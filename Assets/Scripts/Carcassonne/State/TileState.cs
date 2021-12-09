using System.Collections.Generic;
using JetBrains.Annotations;


/// <summary>
/// This script represent the Carcassonne board state
/// </summary>
public class TileState
{
    public List<Tile> Remaining = new List<Tile>(); //  Remaining tiles not yet drawn by any player
    [CanBeNull] public Tile Current; // Current active tile during Phase.TileDrawn
    public Tile[,] Played = new Tile[40,40]; // Current played tiles on board
    public float[,] PlayedId = new float[40,40]; // Id of every played tile mapped in its own matrix for AI use
}
