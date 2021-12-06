using static Carcassonne.Point;

public class AIWrapper : InterfaceAIWrapper
{
    public GameController controller;
    public GameState state;
    public Player player;
    public int totalTiles;

    public bool IsAITurn()
    {
        return player.id == controller.currentPlayer.id;
    }

    public int GetMaxBoardSize()
    {
        return state.tiles.Played.GetLength(0);
    }

    public void PickUpTile()
    {
        controller.PickupTile();
    }

    public int GetCurrentTileId()
    {
        return state.tiles.Current.id;
    }

    public Phase GetGamePhase()
    {
        return state.phase;
    }

    public int GetMeeplesLeft()
    {
        return player.AmountOfFreeMeeples();
    }

    public void EndTurn()
    {
        controller.EndTurn();
    }

    public void DrawMeeple()
    {
        controller.meepleController.DrawMeeple();
    }

    public void RotateTile()
    {
        controller.RotateTile();
    }

    public void PlaceTile(int x, int z)
    {
        controller.iTileAimX = x;
        controller.iTileAimZ = z;
        controller.meepleController.iMeepleAimX = x;
        controller.meepleController.iMeepleAimZ = z;
        controller.ConfirmPlacement();
    }

    public void PlaceMeeple(Direction meepleDirection)
    {
        controller.meepleController.meepleGeography = state.tiles.Current.getGeographyAt(meepleDirection);
        controller.meepleController.meepleDirection = meepleDirection;
        controller.ConfirmMeeplePlacement(meepleDirection);

    }

    public void FreeCurrentMeeple()
    {
        controller.meepleController.FreeMeeple(state.meeples.Current);
    }

    public void Reset()
    {
        controller.state.phase = Phase.GameOver;
    }

    public int GetMaxMeeples()
    {
        return player.meeples.Count;
    }

    public int GetMaxTileId()
    {
        return 33; //I have no clue how to get this in a more error safe manner at the moment.
    }

    public float[,] GetPlacedTiles()
    {
        return state.tiles.PlayedId;
    }

    public NewTile[,] GetTiles()
    {
        return state.tiles.Played;
    }
        
    public int GetNumberOfPlacedTiles()
    {
        return totalTiles - state.tiles.Remaining.Count;
    }

    public int GetTotalTiles() {
        return totalTiles;
    }
}
