using static Carcassonne.Point;
using UnityEngine;
/// <summary>
///  The AIWrapper acts as a middle-man between the AIPlayer-class and the data it needs and actions it can perform. It separates the AI logic from the code implementation. Its specific purpose is to 
///  allow the exact same AIPlayer-class to be used in the real environment and the training environment. This means the AIWrapper class will look different in both these project, as the code running
///  the game differs in the two implementations.
///  Version 1.0
/// </summary>
public class AIWrapper : InterfaceAIWrapper
{
    public GameController controller;
    public GameState state;
    public Player player;
    public int totalTiles;
    public float previousScore;

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
        controller.ConfirmTilePlacement();
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
        previousScore = 0;
        controller.startGame = true;
    }

    public int GetMaxMeeples()
    {
        return player.meeples.Count;
    }

    public int GetMaxTileId()
    {
        return 24;
    }

    public float[,] GetPlacedTiles()
    {
        return state.tiles.PlayedId;
    }

    public object[,] GetTiles()
    {
        return state.tiles.Played;
    }
        
    public int GetNumberOfPlacedTiles()
    {
        return totalTiles - state.tiles.Remaining.Count;
    }

    public int GetTotalTiles() 
    {
        return totalTiles;
    }

    public int GetMinX()
    {
        return controller.minX;
    }

    public int GetMaxX()
    {
        return controller.maxX;
    }

    public int GetMinZ()
    {
        return controller.minZ;
    }

    public int GetMaxZ()
    {
        return controller.maxZ;
    }

    public float GetScore()
    {
        return (float)player.score;
    }

    public float GetScoreChange()
    {
        // if ((float)player.score != previousScore)
        // {
        //     Debug.Log("Player " + player.id + " score changed from " + previousScore + "p to " + player.score + "p");
        // }
        float scoreChange = (float)player.score - previousScore;
        previousScore = (float)player.score;
        return scoreChange;
    }
}
