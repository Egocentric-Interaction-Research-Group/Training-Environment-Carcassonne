using System;

public class GameState
{
    public GameRules Rules;

    /// <summary>
    /// Describes what is happening currently in the game.
    /// </summary>
    public Phase phase;

    public TileState Tiles;
    public MeepleState Meeples;
    public PlayerState Players;

    public GameState()
    {
        Rules = new GameRules();
    }

    public void ResetStates()
    {
        //TileState
        Tiles.Current = null;
        Tiles.Remaining.Clear();
        Array.Clear(Tiles.Played, 0, Tiles.Played.Length);
        Array.Clear(Tiles.PlayedId, 0, Tiles.Played.Length);

        //MeepleState
        Meeples.Current = null;
        Meeples.All.Clear();

        //PlayerState
        Players.All.Clear();
    }
}
