using Carcassonne;
using UnityEngine;
using Random = System.Random;


/// <summary>
///     The Stack of tiles.
/// </summary>
public class Stack : MonoBehaviour
{
    /// <summary>
    ///     The array of tiles
    /// </summary>
    public Object[] tileArray;

    public GameObject firstTile;

    public TileState tiles;

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public NewTile Pop()
    {
        var rand = new Random();
        var idx = rand.Next(tiles.Remaining.Count);

        tiles.Current = tiles.Remaining[idx];
        tiles.Remaining.Remove(tiles.Current);


        return tiles.Current;
    }

    public bool isEmpty()
    {
        return tiles.Remaining.Count == 0;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public Stack createStackScript()
    {
        return this;
    }

    /// <summary>
    /// Populates the array of tiles. Finds all game objects tagged tile. The Master client sets 
    /// </summary>
    public void PopulateTileArray()
    {
        tileArray = Resources.LoadAll("Tiles", typeof(GameObject));
        foreach (Object obj in tileArray)
        {
            GameObject tile = (GameObject)obj;
            if(tile.GetComponent<Tile>().tileSet == Tile.TileSet.Base && tile != firstTile)
            {
                NewTile nTile = new NewTile();
                nTile.AssignAttributes(tile.GetComponent<Tile>().id);
                tiles.Remaining.Add(nTile);
            }       
        }
    }

}
