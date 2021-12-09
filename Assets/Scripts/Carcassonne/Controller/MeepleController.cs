using System.Collections.Generic;
using Carcassonne;
using UnityEngine;

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

    public Meeple FindMeeple(int x, int y, Tile.Geography geography)
    {
        Meeple res = null;

        foreach (Meeple m in meeples.All)
        {;
            if (m.geography == geography && m.x == x && m.z == y) return m;
        }

        return res;
    }

    public Meeple FindMeeple(int x, int y, Tile.Geography geography, Point.Direction direction)
    {
        Meeple res = null;

        foreach (var m in meeples.All)
        {
            if (m.geography == geography && m.x == x && m.z == y && m.direction == direction) return m;
        }

        return res;
    }

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
            meeple.free = false;
            gameController.state.phase = Phase.MeepleDown;
        }
        
    }


    public void FreeMeeple(Meeple meeple)
    {
        meeple.free = true;
        gameController.state.phase = Phase.TileDown;
    }

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
