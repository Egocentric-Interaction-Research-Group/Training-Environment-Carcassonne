using System;

/// <summary>
/// This script representt the collection of all the state script
/// </summary>
public class GameState
{
    public GameRules rules;
    public Phase phase;
    public TileState tiles;
    public MeepleState meeples;
    public PlayerState players;

    /// <summary>
    /// Subsequent states are created on the basis that a new game state is created
    /// </summary>
    public GameState()
    {
        rules = new GameRules();
        tiles = new TileState();
        meeples = new MeepleState();
        players = new PlayerState();
    }

    /// <summary>
    /// Resets certain states in a manner specific to them
    /// </summary>
    public void ResetStates()
    {
        //TileState
        tiles.Current = null;
        tiles.Remaining.Clear();
        Array.Clear(tiles.Played, 0, tiles.Played.Length);
        Array.Clear(tiles.PlayedId, 0, tiles.Played.Length);

        //MeepleState
        meeples.Current = null;
        meeples.All.Clear();
    }
}
