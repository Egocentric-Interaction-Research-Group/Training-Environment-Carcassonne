using Carcassonne.State;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using Assets.Scripts.Carcassonne.AI;
using UnityEngine.SceneManagement;

/// <summary>
/// The AI for the player. An AI user contains both a regular PlayerScript and this AI script to observe and take actions.
/// </summary>

public class CarcassonneAgent : Agent
{
    //Observations from real game (use getter properties, don't call these directly)
    public int meeplesLeft;
    public Phase phase;
    public int id;
    public TileState tiles;

    //AI Specific
    public AIWrapper wrapper;
    private const int maxBranchSize = 6;
    public int x = 85, z = 85, y = 1, rot = 0;
    public float meepleX, meepleZ;

    //Monitoring
    private string placement = "";

    public Phase Phase
    {
        get
        {
            return wrapper.GetGamePhase();
        }
    }

    public int MeeplesLeft
    {
        get
        {
            return wrapper.GetMeeplesLeft();
        }
    }

    public int BoardGridSize
    {
        get
        {
            return wrapper.GetBoardSize();
        }
    }

    public int Id
    {
        get
        {
            return wrapper.GetCurrentTileId();
        }
    }

    /// <summary>
    /// Initial setup which gets the scripts needed to AI calls and observations, called only once when the agent is enabled.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        wrapper = new AIWrapper(GetComponent<Player>());
    }


    /// <summary>
    /// Perform actions based on a vector of numbers. Which actions are made depend on the current game phase.
    /// </summary>
    /// <param name="actionBuffers">The struct of actions to take</param>
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        switch (Phase)
        {
            case Phase.TileDrawn:
                TileDrawnAction(actionBuffers);
                break;
            case Phase.TileDown:
                if (actionBuffers.DiscreteActions[0] == 0f)
                {
                    wrapper.DrawMeeple(); //Take meeple
                }
                else
                {
                    wrapper.EndTurn(); //End turn without taking meeple
                }
                break;
            case Phase.MeepleDrawn:
                MeepleDrawnAction(actionBuffers);
                break;
        }
    }

    private void TileDrawnAction(ActionBuffers actionBuffers)
    {        
        AddReward(-0.001f); //Each call to this method comes with a very minor penalty to promote performing quick actions.
        if (actionBuffers.DiscreteActions[0] == 0f)
        {
            z += 1; //Up
        }
        else if (actionBuffers.DiscreteActions[0] == 1f)
        {
            z -= 1; //Down
        }
        else if (actionBuffers.DiscreteActions[0] == 2f)
        {
            x -= 1; //Left
        }
        else if (actionBuffers.DiscreteActions[0] == 3f)
        {
            x += 1; //Right
        }
        else if (actionBuffers.DiscreteActions[0] == 4f)
        {
            //Each step in rot represents a 90 degree rotation of the tile.
            rot++;
            if (rot == 4)
            {
                rot = 0;
            }
        }
        else if (actionBuffers.DiscreteActions[0] == 5f)
        {
            //After choice checks.
            if (x < 0 || x >= BoardGridSize || z < 0 || z >= BoardGridSize)
            {
                //Outside table area, reset values and add significant punishment.
                ResetAttributes();
                AddReward(-1f);
                Debug.Log("AI got -1 reward for placing tile incorrectly");
            }
            else
            {
                //Rotates the tile the amount of times AI has chosen (0-3).
                for (int i = 0; i < rot; i++)
                {
                    wrapper.RotateTile();
                }

                //Values are loaded into GameController that are used in the ConfirmPlacementRPC call.
                wrapper.PlaceTile(x, z);

                if (Phase == Phase.TileDown) //If the placement was successful, the phase changes to TileDown.
                {                
                    AddReward(1f);
                    Debug.Log("AI got 1 reward for placing tile correctly");
                }
            }      
        }
    }

    /// <summary>
    /// Places the meeple on one of the 5 places available on the tile (Uses the tile to find the positions).
    /// </summary>
    /// <param name="actionBuffers"></param>
    private void MeepleDrawnAction(ActionBuffers actionBuffers)
    {
        AddReward(-0.1f); //Each call (each change of position) gets a negative reward to avoid getting stuck in this stage.
        if (actionBuffers.DiscreteActions[0] == 0f)
        {
            placement = "North";
            meepleX = 0.000f;
            meepleZ = 0.011f;
        }
        else if (actionBuffers.DiscreteActions[0] == 1f)
        {
            placement = "South";
            meepleX = 0.000f;
            meepleZ = -0.011f;
        }
        else if (actionBuffers.DiscreteActions[0] == 2f)
        {
            placement = "West";
            meepleX = -0.011f;
            meepleZ = 0.000f;
        }
        else if (actionBuffers.DiscreteActions[0] == 3f)
        {
            placement = "East";
            meepleX = 0.011f;
            meepleZ = 0.000f;
        }
        else if (actionBuffers.DiscreteActions[0] == 4f)
        {
            placement = "Center";
            meepleX = 0.000f;
            meepleZ = 0.000f;
        }
        else if (actionBuffers.DiscreteActions[0] == 5f)
        {
            if (!String.IsNullOrEmpty(placement)) //Checks so that a choice has been made since meeple was drawn.
            {
                wrapper.PlaceMeeple(meepleX, meepleZ);  //Either confirms and places the meeple if possible, or returns meeple and goes back to phase TileDown.
            }


            if (Phase == Phase.MeepleDown) //If meeple is placed.
            {
                AddReward(1f); //Rewards successfully placing a meeple
            }
            else if (Phase == Phase.TileDown) //If meeple gets returned.
            {
                AddReward(-1f); //Punishes returning a meeple & going back a phase (note: no punishment for never drawing a meeple).
            }
            else //Workaround for a bug where you can draw an unconfirmable meeple and never be able to change phase.
            {
                wrapper.FreeCurrentMeeple();
            }
        }
    }


    /// <summary>
    /// Read inputs from the keyboard and convert them to a list of actions.
    /// This is called only when the player wants to control the agent and has set
    /// Behavior Type to "Heuristic Only" in the Behavior Parameters inspector.
    /// </summary>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //Not implemented.
    }


    /// <summary>
    /// When a new episode begins, reset the agent and area
    /// </summary>
    public override void OnEpisodeBegin()
    {
        //This occurs every X steps (Max Steps). It only serves to reset tile position if AI is stuck, and for AI to process current learning
        ResetAttributes();
        wrapper.Reset();
        Debug.Log("New episode");
    }


    /// <summary>
    /// Collect all non-Raycast observations
    /// </summary>
    /// <param name="sensor">The vector sensor to add observations to</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        //ToDo: All these should be Normalized for optimal learning. Read up on Normalization


        sensor.AddObservation((int)Phase); //This one might need to be changed, read up on One-Hot observation
        sensor.AddObservation(MeeplesLeft);
        sensor.AddObservation(Id);
        sensor.AddObservation(rot);
        sensor.AddObservation(x);
        sensor.AddObservation(z);
        sensor.AddObservation(meepleX);
        sensor.AddObservation(meepleZ);
        
        for(int i = 0; i < tiles.PlayedId.GetLength(0); i++)
        {
            for(int j = 0; j < tiles.PlayedId.GetLength(1); j++)
            {
                sensor.AddObservation(tiles.PlayedId[i, j]);
            }
        } 
    }

    /// <summary>
    /// Masks certain inputs so they cannot be used. Amount of viable inputs depends on the game phase.
    /// </summary>
    /// <param name="actionMask">The actions (related to ActionBuffer actioons) to disable or enable</param>
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        int allowedActions = 0;
        switch (Phase)
        {
            case Phase.TileDrawn:
                //AI can choose to step one tile place in either of the 4 directions (-X, X, -Z, Z), rotate 90 degrees, or confirm place.
                allowedActions = 6;
                break;
            case Phase.TileDown:
                //AI can choose to take or not take a meeple.
                allowedActions = 2;
                break;
            case Phase.MeepleDrawn:
                //AI can choose to place a drawn meeple in 5 different places (N, S, W, E, C) or confirm/deny current placement.
                allowedActions = 6;
                break;
        }

        //Disables all actions of branch 0, index i (on that branch) for any i larger than the allowed actions.
        for (int i = allowedActions; i < maxBranchSize; i++)
        {
            actionMask.SetActionEnabled(0, i, false); //The rest are enabled by default, as it resets to all enabled after a decision.
        }
    }

    /// <summary>
    /// Resets tile position and placement (meeple position) to base position before next action.
    /// </summary>
    internal void ResetAttributes()
    {
        x = 85;
        z = 85;
        y = 1;
        rot = 0;
        placement = "";      
    }
}