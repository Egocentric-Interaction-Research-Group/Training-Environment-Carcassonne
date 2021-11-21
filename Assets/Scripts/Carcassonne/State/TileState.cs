using System.Collections.Generic;
using Carcassonne;
using JetBrains.Annotations;

public class TileState
{
    public List<Tile> Remaining = new List<Tile>();
    [CanBeNull] public Tile Current;
    public Tile[,] Played;
    public float[,] PlayedId;
}
