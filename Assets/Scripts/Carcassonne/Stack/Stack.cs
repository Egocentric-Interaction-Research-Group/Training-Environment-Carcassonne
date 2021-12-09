using UnityEngine;
using Random = System.Random;


/// <summary>
///     The Stack of tiles.
/// </summary>
public class Stack : MonoBehaviour
{
    public TileState tiles;

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public Tile Pop()
    {
        var rand = new Random();
        var idx = rand.Next(tiles.Remaining.Count);

        tiles.Current = tiles.Remaining[idx];
        tiles.Remaining.Remove(tiles.Current);


        return tiles.Current;
    }

    /// <summary>
    /// The base tile, meaning starting tile is created at the beginning of a new game and
    /// is never a part of the remaining tiles at any point
    /// </summary>
    /// <returns></returns>
    public Tile GetBaseTile()
    {
        Tile tile = new Tile();
        tile.AssignAttributes(8);
        return tile;
    }

    /// <summary>
    /// Check if the stack is empty
    /// </summary>
    /// <returns>true if remaining tiles are 0</returns>
    public bool isEmpty()
    {
        return tiles.Remaining.Count == 0;
    }

    /// <summary>
    /// Populates the array of tiles. Larger switch case to tie a fixed amount to each tile id.
    /// The fixed amount is acquired from the actual physical Carcassonne board game
    /// </summary>
    public void PopulateTileArray()
    {
        for(int i = 1; i < 25; i++)
        {
           switch(i)
            {
                case 1:
                    CreateTiles(i, 4);
                    break;
                case 2:
                    CreateTiles(i, 2);
                    break;
                case 3:
                    CreateTiles(i, 8);
                    break;
                case 4:
                    CreateTiles(i, 9);
                    break;
                case 5:
                    CreateTiles(i, 4);
                    break;
                case 6:
                    CreateTiles(i, 1);
                    break;
                case 7:
                    CreateTiles(i, 5);
                    break;
                case 8:
                    CreateTiles(i, 3); //Id 8 represents also the base tile. One copy of this tile is created in GetBaseTile()
                    break;
                case 9:
                    CreateTiles(i, 3);
                    break;
                case 10:
                    CreateTiles(i, 3);
                    break;
                case 11:
                    CreateTiles(i, 3);
                    break;
                case 12:
                    CreateTiles(i, 1);
                    break;
                case 13:
                    CreateTiles(i, 3);
                    break;
                case 14:
                    CreateTiles(i, 3);
                    break;
                case 15:
                    CreateTiles(i, 2);
                    break;
                case 16:
                    CreateTiles(i, 3);
                    break;
                case 17:
                    CreateTiles(i, 2);
                    break;
                case 18:
                    CreateTiles(i, 2);
                    break;
                case 19:
                    CreateTiles(i, 2);
                    break;
                case 20:
                    CreateTiles(i, 3);
                    break;
                case 21:
                    CreateTiles(i, 1);
                    break;
                case 22:
                    CreateTiles(i, 1);
                    break;
                case 23:
                    CreateTiles(i, 2);
                    break;
                case 24:
                    CreateTiles(i, 1);
                    break;
            } 
            
        }
    }

    /// <summary>
    /// Create x amount of tiles based on a tile id. 
    /// Each unique geography is generated from the tile id and assigned to a tile
    /// </summary>
    /// <param name="tileId"></param>
    /// <param name="amount"></param>
    internal void CreateTiles(int tileId, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            Tile tile = new Tile();
            tile.AssignAttributes(tileId);
            tiles.Remaining.Add(tile);
        }
    }

}
