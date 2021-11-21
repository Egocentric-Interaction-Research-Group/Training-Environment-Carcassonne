using System.Collections.Generic;
using Carcassonne;
using UnityEngine;

public class MeepleController : MonoBehaviour
{

    [SerializeField]
    internal GameController gameController;
    [HideInInspector] public List<Meeple> MeeplesInCity;
    internal float fMeepleAimX; //TODO Make Private
    internal float fMeepleAimZ; //TODO Make Private

    private int meepleCount = 0;

    public MeepleState meeples;
    public PlayerState players;

    public MeepleController(GameController gameController)
    {
        this.gameController = gameController;
    }

    // Instantiation Stuff
    public GameObject prefab;

    /// <summary>
    /// Instantiate a new Meeple with the chosen prefab and parent object in the hierarchy.
    /// </summary>
    /// <returns>GameObject : An instance of MeepleScript.prefab.</returns>
    public Meeple GetNewInstance()
    {
        meepleCount++;
        GameObject newMeeple = Instantiate(prefab, new Vector3(), Quaternion.identity);
        newMeeple.gameObject.name = $"Meeple {meepleCount}";
        newMeeple.SetActive(false);

        return newMeeple.GetComponent<Meeple>();
    }



    internal int iMeepleAimX;
    internal int iMeepleAimZ;
    public Tile.Geography meepleGeography;

    public Meeple FindMeeple(int x, int y, Tile.Geography geography)
    {
        Meeple res = null;

        foreach (var m in meeples.All)
        {
            var tmp = m;

            if (tmp.geography == geography && tmp.x == x && tmp.z == y) return tmp;
        }

        return res;
    }

    public Meeple FindMeeple(int x, int y, Tile.Geography geography, Point.Direction direction)
    {
        Meeple res = null;

        foreach (var m in meeples.All)
        {
            var tmp = m;

            if (tmp.geography == geography && tmp.x == x && tmp.z == y && tmp.direction == direction) return tmp;
        }

        return res;
    }

    public void PlaceMeeple(GameObject meeple, int xs, int zs, Point.Direction direction,
        Tile.Geography meepleGeography, GameController gameController)
    {
        var currentTile = gameController.TileController.currentTile.GetComponent<Tile>();
        var currentCenter = currentTile.getCenter();
        bool res;
        if (currentCenter == Tile.Geography.Village || currentCenter == Tile.Geography.Grass ||
            currentCenter == Tile.Geography.Cloister && direction != Point.Direction.CENTER)
            res = GetComponent<Point>()
                .testIfMeepleCantBePlacedDirection(currentTile.vIndex, meepleGeography, direction);
        else if (currentCenter == Tile.Geography.Cloister && direction == Point.Direction.CENTER)
            res = false;
        else
            res = GetComponent<Point>().testIfMeepleCantBePlaced(currentTile.vIndex, meepleGeography);

        if (meepleGeography == Tile.Geography.City)
        {
            if (currentCenter == Tile.Geography.City)
                res = gameController.CityIsFinished(xs, zs) || res;
            else
                res = gameController.CityIsFinishedDirection(xs, zs, direction) || res;
        }

        if (!currentTile.IsOccupied(direction) && !res)
        {

            currentTile.occupy(direction);
            if (meepleGeography == Tile.Geography.CityRoad) meepleGeography = Tile.Geography.City;

            meeple.GetComponent<Meeple>().assignAttributes(xs, zs, direction, meepleGeography);


            gameController.gameState.phase = Phase.MeepleDown;
        }
    }


    public void FreeMeeple(Meeple meeple, GameController gameController)
    {
        meeple.GetComponent<Meeple>().free = true;
        gameController.gameState.phase = Phase.TileDown;
    }

    public void DrawMeeple()
    {
        if (gameController.gameState.phase == Phase.TileDown)
        {
            foreach (Meeple meeple in gameController.currentPlayer.meeples) //TODO Inefficient. Just want the first free meeple.
            {
                GameObject meepleGameObject = meeple.gameObject;
                if (meeple.free)
                {
                    meeples.Current = meepleGameObject.GetComponent<Meeple>();
                    gameController.gameState.phase = Phase.MeepleDrawn;
                    break;
                }
            }
        }
    }

}
