using System.Collections.Generic;
using Carcassonne;
using UnityEngine;
using static Carcassonne.Tile;

public class MeepleController : MonoBehaviour
{

    public GameController gameController;
    public Point point;
    [HideInInspector] public List<Meeple> MeeplesInCity;

    public MeepleState meeples;
    public PlayerState players;






    internal int iMeepleAimX;
    internal int iMeepleAimZ;
    public NewTile.Geography meepleGeography;

    public Meeple FindMeeple(int x, int y, NewTile.Geography geography)
    {
        Meeple res = null;

        foreach (Meeple m in meeples.All)
        {;
            if (m.geography == geography && m.x == x && m.z == y) return m;
        }

        return res;
    }

    public Meeple FindMeeple(int x, int y, NewTile.Geography geography, Point.Direction direction)
    {
        Meeple res = null;

        foreach (var m in meeples.All)
        {
            if (m.geography == geography && m.x == x && m.z == y && m.direction == direction) return m;
        }

        return res;
    }

    public void PlaceMeeple(Meeple meeple, int xs, int zs, Point.Direction direction,
        NewTile.Geography meepleGeography)
    {
        NewTile currentTile = gameController.state.tiles.Current;
        NewTile.Geography currentCenter = currentTile.getCenter();
        bool res;
        if (currentCenter == NewTile.Geography.Village || currentCenter == NewTile.Geography.Grass ||
            currentCenter == NewTile.Geography.Cloister && direction != Point.Direction.CENTER)
            res = point
                .testIfMeepleCantBePlacedDirection(currentTile.vIndex, meepleGeography, direction);
        else if (currentCenter == NewTile.Geography.Cloister && direction == Point.Direction.CENTER)
            res = false;
        else
            res = point.testIfMeepleCantBePlaced(currentTile.vIndex, meepleGeography);

        if (meepleGeography == NewTile.Geography.City)
        {
            if (currentCenter == NewTile.Geography.City)
                res = gameController.CityIsFinished(xs, zs) || res;
            else
                res = gameController.CityIsFinishedDirection(xs, zs, direction) || res;
        }

        if (!currentTile.IsOccupied(direction) && !res)
        {

            currentTile.occupy(direction);
            if (meepleGeography == NewTile.Geography.CityRoad) meepleGeography = NewTile.Geography.City;

            meeple.assignAttributes(xs, zs, direction, meepleGeography);
            gameController.state.phase = Phase.MeepleDown;
        }
        else
        {
            FreeMeeple(meeple);
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
