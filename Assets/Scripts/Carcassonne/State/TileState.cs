using System.Collections.Generic;
using JetBrains.Annotations;

public class TileState
{
    public List<NewTile> Remaining = new List<NewTile>();
    [CanBeNull] public NewTile Current;
    public NewTile[,] Played = new NewTile[30,30];
    public float[,] PlayedId = new float[30,30];
}
