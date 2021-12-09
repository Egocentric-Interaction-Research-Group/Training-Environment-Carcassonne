using Carcassonne;
using UnityEngine;

/// <summary>
/// Script encapsulating information about tiles that have been played on the board.
/// It also handles the adding of tiles on the board
/// </summary>
public class PlacedTiles : MonoBehaviour
{

    public TileState tiles;

    /// <summary>
    /// Place a tile on x,z in the tile grid
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="tile"></param>
    public void PlaceTile(int x, int z, Tile tile)
    {
        tiles.Played[x, z] = tile;
        tiles.PlayedId[x, z] = tile.id;
    }

    /// <summary>
    /// Returns the tile on x,z in the tile grid
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public Tile getPlacedTile(int x, int z)
    {
        Tile t = tiles.Played[x, z];
        if (t is null)
        {
            return null;
        }

        return t;
    }

    /// <summary>
    /// Returns the length or size of the tile grid. Dimension specifies which axis, 0 = x, 1 = z
    /// </summary>
    /// <param name="dimension"></param>
    /// <returns></returns>
    public int GetLength(int dimension)
    {
        return tiles.Played.GetLength(dimension);
    }

    /// <summary>
    /// Checks whether a point in the tile grid has neighbouring tiles
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public bool HasNeighbor(int x, int z)
    {
        if (x + 1 < tiles.Played.GetLength(0))
            if (tiles.Played[x + 1, z] != null)
                return true;
        if (x - 1 >= 0)
            if (tiles.Played[x - 1, z] != null)
                return true;
        if (z + 1 < tiles.Played.GetLength(1))
            if (tiles.Played[x, z + 1] != null)
                return true;
        if (z - 1 >= 0)
            if (tiles.Played[x, z - 1] != null)
                return true;
        return false;
    }

    /// <summary>
    /// Checks if a tile has a matching geography in a direction.
    /// Returns true if there is no neighbouring tile (null) or if a neighbouring tile's
    /// geography match in the corresponding direction
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="dir"></param>
    /// <param name="geography"></param>
    /// <returns></returns>
    public bool MatchGeographyOrNull(int x, int y, Point.Direction dir, Tile.Geography geography)
    {
        if (tiles.Played[x, y] == null)
            return true;
        if (tiles.Played[x, y].getGeographyAt(dir) == geography)
            return true;
        return false;
    }

    /// <summary>
    /// Checks whether a tile has a city in it's center
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool CityTileHasCityCenter(int x, int y)
    {
        return tiles.Played[x, y].getCenter() == Tile.Geography.City ||
               tiles.Played[x, y].getCenter() == Tile.Geography.CityRoad;
    }

    /// <summary>
    /// Checks whether a tile has grass or a stream(UNUSED) in it's center
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool CityTileHasGrassOrStreamCenter(int x, int y)
    {
        return tiles.Played[x, y].getCenter() == Tile.Geography.Grass ||
               tiles.Played[x, y].getCenter() == Tile.Geography.Stream;
    }

    /// <summary>
    /// Retrieves the neighbours of a coordinate on the tile grid
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int[] GetNeighbors(int x, int y)
    {
        var Neighbors = new int[4];
        var itt = 0;


        if (tiles.Played[x + 1, y] != null)
        {
            Neighbors[itt] = tiles.Played[x + 1, y].vIndex;
            itt++;
        }

        if (tiles.Played[x - 1, y] != null)
        {
            Neighbors[itt] = tiles.Played[x - 1, y].vIndex;
            itt++;
        }

        if (tiles.Played[x, y + 1] != null)
        {
            Neighbors[itt] = tiles.Played[x, y + 1].vIndex;
            itt++;
        }

        if (tiles.Played[x, y - 1] != null) Neighbors[itt] = tiles.Played[x, y - 1].vIndex;
        return Neighbors;
    }

    /// <summary>
    /// Weight means geography in this case. Code has probably been semi-copied from an online algorithm
    /// and not been properly updated for this project.
    /// The method will return the geography of a coordinate on the tile grid 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Tile.Geography[] getWeights(int x, int y)
    {
        var weights = new Tile.Geography[4];
        var itt = 0;
        if (tiles.Played[x + 1, y] != null)
        {
            weights[itt] = tiles.Played[x + 1, y].West;
            itt++;
        }

        if (tiles.Played[x - 1, y] != null)
        {
            weights[itt] = tiles.Played[x - 1, y].East;
            itt++;
        }

        if (tiles.Played[x, y + 1] != null)
        {
            weights[itt] = tiles.Played[x, y + 1].South;
            itt++;
        }

        if (tiles.Played[x, y - 1] != null) weights[itt] = tiles.Played[x, y - 1].North;
        return weights;
    }

    /// <summary>
    /// Returns the geography in the center of coordinate on the tile grid
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Tile.Geography[] getCenters(int x, int y)
    {
        var centers = new Tile.Geography[4];
        var itt = 0;
        if (tiles.Played[x + 1, y] != null)
        {
            centers[itt] = tiles.Played[x + 1, y].getCenter();
            itt++;
        }

        if (tiles.Played[x - 1, y] != null)
        {
            centers[itt] = tiles.Played[x - 1, y].getCenter();
            itt++;
        }

        if (tiles.Played[x, y + 1] != null)
        {
            centers[itt] = tiles.Played[x, y + 1].getCenter();
            itt++;
        }

        if (tiles.Played[x, y - 1] != null) centers[itt] = tiles.Played[x, y - 1].getCenter();
        return centers;
    }

    /// <summary>
    /// Get usuable directions of coordinates in the tile grid. If the coordinates contains
    /// a tile and that tile has a direction for example in the direction 'North' then that direction will be placed in
    /// array to be returned by the method
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Point.Direction[] getDirections(int x, int y)
    {
        var directions = new Point.Direction[4];
        var itt = 0;
        if (tiles.Played[x + 1, y] != null)
        {
            directions[itt] = Point.Direction.EAST;
            itt++;
        }

        if (tiles.Played[x - 1, y] != null)
        {
            directions[itt] = Point.Direction.WEST;
            itt++;
        }

        if (tiles.Played[x, y + 1] != null)
        {
            directions[itt] = Point.Direction.NORTH;
            itt++;
        }

        if (tiles.Played[x, y - 1] != null) directions[itt] = Point.Direction.SOUTH;
        return directions;
    }

    /// <summary>
    /// Checks whether a cloister tile is surrounded by other tiles
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="endTurn"></param>
    /// <returns></returns>
    public int CheckSurroundedCloister(int x, int z, bool endTurn)
    {
        var pts = 1;
        if (tiles.Played[x - 1, z - 1] != null) pts++;
        if (tiles.Played[x - 1, z] != null) pts++;
        if (tiles.Played[x - 1, z + 1] != null) pts++;
        if (tiles.Played[x, z - 1] != null) pts++;
        if (tiles.Played[x, z + 1] != null) pts++;
        if (tiles.Played[x + 1, z - 1] != null) pts++;
        if (tiles.Played[x + 1, z] != null) pts++;
        if (tiles.Played[x + 1, z + 1] != null) pts++;
        if (pts == 9 || endTurn)
            return pts;
        return 0;
    }

    /// <summary>
    /// Checks whether neighbouring tile interfers with tile placement, meaning if the current tile matches up
    /// with neighbouring tiles
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool CheckNeighborsIfTileCanBePlaced(Tile tile, int x, int y)
    {
        var isNotAlone2 = false;

        if (tiles.Played[x - 1, y] != null)
        {
            isNotAlone2 = true;
            if (tile.West == tiles.Played[x - 1, y].East) return false;
        }

        if (tiles.Played[x + 1, y] != null)
        {
            isNotAlone2 = true;
            if (tile.East == tiles.Played[x + 1, y].West) return false;
        }

        if (tiles.Played[x, y - 1] != null)
        {
            isNotAlone2 = true;
            if (tile.South == tiles.Played[x, y - 1].North) return false;
        }

        if (tiles.Played[x, y + 1] != null)
        {
            isNotAlone2 = true;
            if (tile.North == tiles.Played[x, y + 1].South) return false;
        }

        return isNotAlone2;
    }

    /// <summary>
    /// Checks if a tile is placed within the confinement of the tile grid
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public bool TilePlacementIsValid(Tile tile, int x, int z)
    {
        if (x < 0 || x >= tiles.Played.GetLength(0) || z < 0 || z >= tiles.Played.GetLength(1) || tiles.Played[x, z] != null)
            return false;
     
        var isNotAlone = false;
        if (x > 0 && tiles.Played[x - 1, z] != null)
        {
            isNotAlone = true;
            if (tile.West != tiles.Played[x - 1, z].East) return false;
        }

        if (x + 1 < tiles.Played.GetLength(0) && tiles.Played[x + 1, z] != null)
        {
            isNotAlone = true;
            if (tile.East != tiles.Played[x + 1, z].West) return false;
        }

        if (z > 0 && tiles.Played[x, z - 1] != null)
        {
            isNotAlone = true;
            if (tile.South != tiles.Played[x, z - 1].North) return false;
        }

        if (z + 1 < tiles.Played.GetLength(1) && tiles.Played[x, z + 1] != null)
        {
            isNotAlone = true;
            if (tile.North != tiles.Played[x, z + 1].South) return false;
        }

        return isNotAlone;
    }
}
