using System.Collections.Generic;
using Carcassonne;
using JetBrains.Annotations;

public class TileState
{
    public List<Tile> Remaining = new List<Tile>();
    [CanBeNull] public Tile Current;
    public Tile[,] Played = new Tile[170,170];
    public float[,] PlayedId = new float[170,170];
}
