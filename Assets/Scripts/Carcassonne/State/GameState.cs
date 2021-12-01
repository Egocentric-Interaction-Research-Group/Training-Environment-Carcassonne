using System;

public class GameState
{
    public GameRules rules;
    public Phase phase;
    public TileState tiles;
    public MeepleState meeples;
    public PlayerState players;

    public GameState()
    {
        rules = new GameRules();
        tiles = new TileState();
        meeples = new MeepleState();
        players = new PlayerState();
    }

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
