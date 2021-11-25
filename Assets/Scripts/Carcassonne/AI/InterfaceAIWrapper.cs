using Carcassonne;


public interface InterfaceAIWrapper
{
        //Further methods may be needed to handle the observation of placed tiles.
    public bool IsAITurn();

    public void PickUpTile();

    public int GetCurrentTileId();

    public Phase GetGamePhase();

    public int GetMeeplesLeft();

    public void EndTurn();

    public void DrawMeeple();

    public void RotateTile();

    public void PlaceTile(int x, int z);

    public void PlaceMeeple(float x, float z);

    public void FreeCurrentMeeple();

    public int GetMaxMeeples();

    public int GetMaxTileId();

    public int GetMaxBoardSize();

    /// <summary>
    /// Gets a 2d array of tile ids as floats. Elements of value 0 contain no tile.
    /// </summary>
    public float[,] GetPlacedTiles();

    /// <summary>
    /// Gets a 2d array of tiles. Contains null elements where there are no tiles.
    /// </summary>
    public Tile[,] GetTiles();    
}

