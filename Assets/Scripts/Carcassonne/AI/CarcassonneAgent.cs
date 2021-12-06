using Carcassonne;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using Assets.Scripts.Carcassonne.AI;
using static Carcassonne.Point;


/// <summary>
/// The AI for the player. An AI user contains both a regular PlayerScript and this AI script to observe and take actions.
/// </summary>

public class CarcassonneAgent : Agent
{
    public enum ObservationApproach
    {
        [Tooltip("Observation size: 917")]
        [InspectorName("Tile Ids")] 
        TileIds,
        
        [Tooltip("Observation size: 1817")]
        Packed
    }

    //Enum Observations
    private Direction meepleDirection = Direction.SELF;

    // Observation approach
    public ObservationApproach observationApproach = ObservationApproach.TileIds;
    private Action<VectorSensor> AddTileObservations;

    //AI Specific
    public AIWrapper wrapper;
    private const int maxBranchSize = 6;
    public int x = 15, z = 15, rot = 0;

    /// <summary>
    /// Initial setup which gets the scripts needed to AI calls and observations, called only once when the agent is enabled.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        wrapper = new AIWrapper();
        // Setup delegate for tile observation approach.
        switch (observationApproach)
        {
            case ObservationApproach.TileIds:
                AddTileObservations = AddTileIdObservations;
                break;

            case ObservationApproach.Packed:
                AddTileObservations = AddPackedTileObservations;
                break;

                // Note: There should only ever be one tile observations function in use, hence '=', and not '+='.
        }
    }

    /// <summary>
    /// Perform actions based on a vector of numbers. Which actions are made depend on the current game phase.
    /// </summary>
    /// <param name="actionBuffers">The struct of actions to take</param>
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        switch (wrapper.GetGamePhase())
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

                //Punishment for rotating more than needed, i.e. returning back to default rotation state.
                //AddReward(-0.01f); 
            }
        }
        else if (actionBuffers.DiscreteActions[0] == 5f) //Place tile
        {
            //Rotates the tile the amount of times AI has chosen (0-3).
            for (int i = 0; i <= rot; i++)
            {
                wrapper.RotateTile();
            }

            //Values are loaded into GameController that are used in the ConfirmPlacementRPC call.
            wrapper.PlaceTile(x, z);

            if (wrapper.GetGamePhase() == Phase.TileDown) //If the placement was successful, the phase changes to TileDown.
            {
                AddReward(0.5f);
            }         
        }

        //After choice checks to determine if AI is Out of Bounds. Bounds a defined by the minimum and maximum coordinates in each axis for tiles placed
        if (x < wrapper.GetMinX() - 1 || x > wrapper.GetMaxX() + 1 || z < wrapper.GetMinZ() - 1 || z > wrapper.GetMaxZ() + 1)
        {
            //Outside table area, reset values and add significant punishment.
            ResetAttributes();
            AddReward(-0.05f);
        }
    }

    /// <summary>
    /// Places the meeple on one of the 5 places available on the tile (Uses the tile to find the positions).
    /// </summary>
    /// <param name="actionBuffers"></param>
    private void MeepleDrawnAction(ActionBuffers actionBuffers)
    {
        AddReward(-0.01f); //Each call gets a negative reward to avoid getting stuck just moving the meeple around in this stage.
        if (actionBuffers.DiscreteActions[0] == 0f)
        {
            meepleDirection = Direction.NORTH;
        }
        else if (actionBuffers.DiscreteActions[0] == 1f)
        {
            meepleDirection = Direction.SOUTH;
        }
        else if (actionBuffers.DiscreteActions[0] == 2f)
        {
            meepleDirection = Direction.WEST;
        }
        else if (actionBuffers.DiscreteActions[0] == 3f)
        {
            meepleDirection = Direction.EAST;
        }
        else if (actionBuffers.DiscreteActions[0] == 4f)
        {
            meepleDirection = Direction.CENTER;
        }
        else if (actionBuffers.DiscreteActions[0] == 5f)
        {
            if (meepleDirection != Direction.SELF) //Checks so that a placement choice has been made since meeple was drawn.
            {
                wrapper.PlaceMeeple(meepleDirection);  //Either confirms and places the meeple if possible, or returns meeple and goes back to phase TileDown.
            }

            if (wrapper.GetGamePhase() == Phase.MeepleDown) //If meeple is placed.
            {
                AddReward(0.1f); //Rewards successfully placing a meeple
            }
            else if (wrapper.GetGamePhase() == Phase.TileDown) //If meeple gets returned.
            {
                AddReward(-0.1f); //Punishes returning a meeple & going back a phase (note: no punishment for never drawing a meeple).
            }
            else //Workaround for a bug where you can draw an unconfirmable meeple and never be able to change phase.
            {
                wrapper.FreeCurrentMeeple();
            }
        }
    }

    private void AddTileIdObservations(VectorSensor sensor)
    {
        NewTile[,] playedTiles = wrapper.GetTiles();
        foreach (NewTile t in playedTiles)
        {
            if (t == null)
            {
                sensor.AddObservation(0.0f);
                continue;
            }

            float obs = t.id + t.rotation * 100; // Note that tile ids must not exceed 99.
            sensor.AddObservation(obs);  
        }
    }

    private void AddPackedTileObservations(VectorSensor sensor)
    {
        NewTile[,] tiles = wrapper.GetTiles();

        for (int row = 0; row < tiles.GetLength(0); row++)
        {
            for (int col = 0; col < tiles.GetLength(1); col++)
            {
                NewTile tile = tiles[col, row];
                int tileData = 0x0;
                int meepleData = 0x0;
                
                if (tile == null)
                {
                    tileData = unchecked((int)0xFFFFFFFF); // No data = -1.
                    sensor.AddObservation(tileData);
                    sensor.AddObservation(meepleData);
                    continue;
                }

                const int bitMask4  = 0xF; // 4-bit mask.
                const int bitMask3  = 0x7; // 3-bit mask.
                int meeplePlayerId  = -1;   // TODO: implement when there is a way to access the meeple from a tile.
                int meepleDirection = 0;   // TODO: see above.

                tileData |= ((int)tile.Center & bitMask4);
                tileData |= (((int)tile.East  & bitMask4) << 4);
                tileData |= (((int)tile.North & bitMask4) << 8);
                tileData |= (((int)tile.West  & bitMask4) << 12);
                tileData |= (((int)tile.South & bitMask4) << 16);

                if (meeplePlayerId >= 0) // If there IS a meeple placed on this tile.
                {
                    meepleData |= (meeplePlayerId        & bitMask3) << 20; // Insert 3-bit player id for meeple. Should be between 0-7.
                    meepleData |= ((meepleDirection + 1) & bitMask3) << 23; // Insert 3-bit value for meeple direction.
                }

                // In total, 26 of 32 bits are used to store geography data and meeple placement,
                // and the id of its owner.

                // Normalize by maximum, which is 20 bits set (1,048,575).
                float normalizedTileData = tileData / (float)(0xFFFFF);

                // Normalize by maximum, which is 6 bits set (63).
                float normalizedMeepleData = meepleData / (float)(0x3F);

                sensor.AddObservation(normalizedTileData);
                sensor.AddObservation(normalizedMeepleData);
            }
        }

    }


    /// <summary>
    /// When a new episode begins, reset the agent and area
    /// </summary>
    public override void OnEpisodeBegin()
    {
        //This occurs every X steps (Max Steps). It only serves to reset tile position if AI is stuck, and for AI to process current learning
        ResetAttributes();
        if(wrapper.state.phase != Phase.GameOver)
        {
            wrapper.Reset();
        }
    }

    /// <summary>
    /// Collect all observations, normalized.
    /// </summary>
    /// <param name="sensor">The vector sensor to add observations to</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(MeeplesLeft / meeplesMax); Dos not work as meeples don't seem to be implemented at all at the moment
        sensor.AddObservation(wrapper.GetCurrentTileId() / wrapper.GetMaxTileId());
        sensor.AddObservation(rot / 3f);
        sensor.AddObservation(x / wrapper.GetMaxBoardSize());
        sensor.AddObservation(z / wrapper.GetMaxBoardSize());
        sensor.AddObservation(wrapper.GetNumberOfPlacedTiles() / wrapper.GetTotalTiles());

        //One-Hot observations of enums (can be done with less code, but this is more readable)
        int MAX_PHASES = Enum.GetValues(typeof(Phase)).Length;
        int MAX_DIRECTIONS = Enum.GetValues(typeof(Direction)).Length;

        sensor.AddOneHotObservation((int)wrapper.GetGamePhase(), MAX_PHASES);
        sensor.AddOneHotObservation((int)meepleDirection, MAX_DIRECTIONS);


        // Call the tile observation method that was assigned at initialization,
        // using the editor-exposed 'observationApproach' field.
        AddTileObservations?.Invoke(sensor);
    }

    /// <summary>
    /// Masks certain inputs so they cannot be used. Amount of viable inputs depends on the game phase.
    /// </summary>
    /// <param name="actionMask">The actions (related to ActionBuffer actioons) to disable or enable</param>
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        int allowedActions = 0;
        switch (wrapper.GetGamePhase())
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
        x = 15;
        z = 15;
        rot = 0;
        meepleDirection = Direction.SELF;
    }
}
