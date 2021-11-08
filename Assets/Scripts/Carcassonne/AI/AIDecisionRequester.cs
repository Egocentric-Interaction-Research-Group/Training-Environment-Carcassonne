using UnityEngine;
using Carcassonne.State;

/// <summary>
/// The AIDecisionRequester sets up the allowed number of actions for the AI and and requests a decision when needed.
/// </summary>
public class AIDecisionRequester : MonoBehaviour
{
    public CarcassonneAgent ai;
    public float reward = 0; //Used for displaying the reward in the Unity editor.
    private Phase startPhase;


    /// <summary>
    /// Acts on its own or repeatedly requests actions from the actual AI depending the game phase and state.
    /// </summary>
    void FixedUpdate()
    {
        if (ai == null || !ai.wrapper.IsAITurn())
        {
            return;
        }
        Debug.Log("Decision Requesting");
        switch (ai.wrapper.GetGamePhase())
        {
            case Phase.NewTurn: // Picks a new tile automatically
                ai.ResetAttributes();
                ai.wrapper.PickUpTile();
                break;
            case Phase.MeepleDown: //Ends turn automatically and resets AI for next move.
                ai.wrapper.EndTurn();
                break;
            case Phase.GameOver: //ToDo: Add reinforcement based on score
                ai.EndEpisode();
                break;
            default: //Calls for one AI action repeatedly with each FixedUpdate until the phase changes.
                ai.RequestDecision();
                break;
        }

        //ToDo: Add this info to some form of GUI-display instead to visualize many separate agents values concurrently.
        reward = ai.GetCumulativeReward();
        Debug.Log("AI " + ai.GetComponent<Player>().id +" Reward is currently " + reward);
    }
}
