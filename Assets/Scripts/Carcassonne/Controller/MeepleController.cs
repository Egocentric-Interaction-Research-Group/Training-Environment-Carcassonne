using System.Collections.Generic;
using Carcassonne;
using UnityEngine;

/// <summary>
/// Controller script for meeples. It handles everything with 
/// regards to meeple control from drawing to placement
/// </summary>
public class MeepleController : MonoBehaviour
{

    public GameController gameController;
    public Point point;
    [HideInInspector] public List<Meeple> MeeplesInCity;

    public MeepleState meeples;
    public PlayerState players;

    internal int iMeepleAimX;
    internal int iMeepleAimZ;
    public Tile.Geography meepleGeography;
    public Point.Direction meepleDirection;

    /// <summary>
    /// Returns a meeple on x,z and on the specified geography
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="geography"></param>
    /// <returns></returns>
    public Meeple FindMeeple(int x, int y, Tile.Geography geography)
    {
        Meeple res = null;

        foreach (Meeple m in meeples.All)
        {;
            if (m.geography == geography && m.x == x && m.z == y) return m;
        }

        return res;
    }

    /// <summary>
    /// Returns a meeple on x,z and on the specified geography with the given direction
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="geography"></param>
    /// <returns></returns>
    public Meeple FindMeeple(int x, int y, Tile.Geography geography, Point.Direction direction)
    {
        Meeple res = null;

        foreach (var m in meeples.All)
        {
            if (m.geography == geography && m.x == x && m.z == y && m.direction == direction) return m;
        }

        return res;
    }

    /// <summary>
    /// Place a meeple on the tile grid in the specified direction.
    /// The method will check whether a meeple is already occupying said geography either direct or indirect
    /// </summary>
    /// <param name="meeple"></param>
    /// <param name="xs"></param>
    /// <param name="zs"></param>
    /// <param name="meepleDirection"></param>
    /// <param name="meepleGeography"></param>
    public void PlaceMeeple(Meeple meeple, int xs, int zs, Point.Direction meepleDirection,
        Tile.Geography meepleGeography)
    {
        Tile currentTile = gameController.state.tiles.Current;
        Tile.Geography currentCenter = currentTile.getCenter();
        bool res;
        if (currentCenter == Tile.Geography.Village || currentCenter == Tile.Geography.Grass ||
            currentCenter == Tile.Geography.Cloister && meepleDirection != Point.Direction.CENTER)
            res = point
                .testIfMeepleCantBePlacedDirection(currentTile.vIndex, meepleGeography, meepleDirection);
        else if (currentCenter == Tile.Geography.Cloister && meepleDirection == Point.Direction.CENTER)
            res = false;
        else
            res = point.testIfMeepleCantBePlaced(currentTile.vIndex, meepleGeography);

        if (meepleGeography == Tile.Geography.City)
        {
            if (currentCenter == Tile.Geography.City)
                res = gameController.CityIsFinished(xs, zs) || res;
            else
                res = gameController.CityIsFinishedDirection(xs, zs, meepleDirection) || res;
        }

        if (!currentTile.IsOccupied(meepleDirection) && !res)
        {
            currentTile.occupy(meepleDirection);
            if (meepleGeography == Tile.Geography.CityRoad) meepleGeography = Tile.Geography.City;

            meeple.assignAttributes(xs, zs, meepleDirection, meepleGeography);
            gameController.state.phase = Phase.MeepleDown;
        }
        
    }

    /// <summary>
    /// Sets the freedom status of a meeple to free
    /// and moves back the game state from MeepleDrawn to TileDown
    /// </summary>
    /// <param name="meeple"></param>
    public void FreeMeeple(Meeple meeple)
    {
        meeple.free = true;
        gameController.state.phase = Phase.TileDown;
    }

    /// <summary>
    /// If the game is at the point when a meeple can be drawn, the first available (free) meeple the current player has is placed in
    /// current meeple and the game phase moves from TileDown to MeepleDrawn
    /// </summary>
    public void DrawMeeple()
    {
        if (gameController.state.phase == Phase.TileDown)
        {
            foreach (Meeple meeple in gameController.currentPlayer.meeples) //TODO Inefficient. Just want the first free meeple.
            {
                if (meeple.free)
                {
                    meeples.Current = meeple;
                    gameController.state.phase = Phase.MeepleDrawn;
                    break;
                }
            }
        }
    }

}
