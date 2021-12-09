using Carcassonne;
using UnityEngine;


/// <summary>
/// This script represents a tile in Carcassonne. It contains the geography for said tile and if any part of it
/// is occupied by a meeple
/// </summary>
public class Tile
{

    /// <summary>
    ///     Describes the different set of game tiles (used in different versions of gameplay).
    /// </summary>
    public enum TileSet
    {
        Base,
        River
    }

    /// <summary>
    ///     Geography decides what is contained within each direction. If there is a road going out to the right and the
    ///     rotation is 0 then east will become "road"
    /// </summary>
    public enum Geography
    {
        Cloister,
        Village,
        Road,
        Grass,
        City,
        Stream,
        CityStream,
        RoadStream,
        CityRoad
    }

    /// <summary>
    ///     The ID decides which type of tile this tile is. Refer to the ID graph for exact results.
    /// </summary>
    public int id;

    /// <summary>
    ///     How many times the tile has been rotated. In standard the rotation is 0, and rotated 4 times it returns to 0.
    /// </summary>
    public int rotation;


    /// <summary>
    ///     The vIndex of the tile. Is applied when placed on the board
    /// </summary>
    public int vIndex;

    public bool northOcupied, southOcupied, eastOcupied, westOcupied, centerOcupied; //TODO Fix Spelling

    /// <summary>
    ///     The list of textures. All tile instances have a reference of all the textures so it can assign it to itself
    ///     depending on the tile ID
    /// </summary>
    public Texture[] textures;

    /// <summary>
    ///     These are closely related to the Up, Down, Left and Right geographies. When the tile is rotated the values shift to
    ///     correlate to the new rotation:
    ///     If Up is road, but the rotation is 1 then East gets the value of Up, since it's rotated 90 degrees clockwise. If
    ///     rotation is 0 then North is equal to Up.
    /// </summary>
    public Geography North, South, West, East, Center;

    /// <summary>
    ///     Defines whether the tile is a member of the base set or one of the expansions or alternate tile sets.
    /// </summary>
    public TileSet tileSet = TileSet.Base;

    /// <summary>
    ///     Geography locations set to different local directions.
    /// </summary>
    private Geography Up, Down, Left, Right;

    private bool shield;

    /// <summary>
    ///     Simple getter for the centerGeography
    /// </summary>
    /// <returns>The center geography</returns>
    public Geography getCenter()
    {
        return Center;
    }


    /// <summary>
    /// Returns wether a direction on the tile occupied or not
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool IsOccupied(Point.Direction direction)  //TODO Fix naming (spelling)
    {
        switch (direction)
        {
            case Point.Direction.NORTH:
                return northOcupied;
            case Point.Direction.SOUTH:
                return southOcupied;
            case Point.Direction.EAST:
                return eastOcupied;
            case Point.Direction.WEST:
                return westOcupied;
            default:
                return centerOcupied;
        }
    }

    /// <summary>
    /// Sets a direction to be occupied on the ile
    /// </summary>
    /// <param name="direction"></param>
    public void occupy(Point.Direction direction)
    {
        if (direction == Point.Direction.NORTH) northOcupied = true;
        if (direction == Point.Direction.SOUTH) southOcupied = true;
        if (direction == Point.Direction.EAST) eastOcupied = true;
        if (direction == Point.Direction.WEST) westOcupied = true;
        if (direction == Point.Direction.CENTER) centerOcupied = true;
        if (Center == getGeographyAt(direction) && direction != Point.Direction.CENTER ||
            Center == Geography.City)
        {
            if (getGeographyAt(Point.Direction.NORTH) == getGeographyAt(direction)) northOcupied = true;
            if (getGeographyAt(Point.Direction.EAST) == getGeographyAt(direction)) eastOcupied = true;
            if (getGeographyAt(Point.Direction.SOUTH) == getGeographyAt(direction)) southOcupied = true;
            if (getGeographyAt(Point.Direction.WEST) == getGeographyAt(direction)) westOcupied = true;
        }

        if (Center == Geography.City && getGeographyAt(direction) == Geography.City)
            centerOcupied = true;
        else if (Center == Geography.Road && getGeographyAt(direction) == Geography.Road) centerOcupied = true;
    }

    /// <summary>
    ///     Returns the tile geography at a specific direction.
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public Geography getGeographyAt(Point.Direction direction)
    {
        if (direction == Point.Direction.NORTH) return North;
        if (direction == Point.Direction.SOUTH) return South;
        if (direction == Point.Direction.EAST) return East;
        if (direction == Point.Direction.WEST)
            return West;
        return Center;
    }


    /// <summary>
    ///     Depending on the ID of the tile it recieves different attributes.
    ///     ID's in tiles are not unique and they share them with other tiles who also recieve the same attributes.
    /// </summary>
    /// <param name="id"></param>
    public void AssignAttributes(int id)
    {
        rotation = 0;
        this.id = id;
        if (id == 1 || id == 2 || id == 3 || id == 4 || id == 5 || id == 6 || id == 12 || id == 17 || id == 25 ||
            id == 26 || id == 27 || id == 28) Up = Geography.Grass;
        if (id == 1 || id == 2 || id == 4 || id == 7 || id == 9 || id == 14 || id == 25 || id == 27)
            Right = Geography.Grass;
        if (id == 1 || id == 3 || id == 7 || id == 8 || id == 12 || id == 13 || id == 15 || id == 17 || id == 18 ||
            id == 20 || id == 22 || id == 26) Down = Geography.Grass;
        if (id == 1 || id == 2 || id == 7 || id == 10 || id == 13 || id == 14 || id == 15 || id == 18 || id == 25)
            Left = Geography.Grass;
        if (id == 6 || id == 29 || id == 30) Up = Geography.Road;
        if (id == 3 || id == 5 || id == 6 || id == 8 || id == 10 || id == 11 || id == 30) Right = Geography.Road;
        if (id == 2 || id == 4 || id == 5 || id == 6 || id == 9 || id == 10 || id == 11 || id == 16 || id == 19 ||
            id == 21 || id == 23 || id == 28 || id == 29 || id == 31) Down = Geography.Road;
        if (id == 3 || id == 4 || id == 5 || id == 6 || id == 8 || id == 9 || id == 11 || id == 16 || id == 19)
            Left = Geography.Road;
        if (id == 7 || id == 8 || id == 9 || id == 10 || id == 11 || id == 13 || id == 14 || id == 15 || id == 16 ||
            id == 18 || id == 19 || id == 20 || id == 21 || id == 22 || id == 23 || id == 24 || id == 31 || id == 32 ||
            id == 33) Up = Geography.City;
        if (id == 12 || id == 13 || id == 15 || id == 16 || id == 17 || id == 18 || id == 19 || id == 20 || id == 21 ||
            id == 22 || id == 23 || id == 24 || id == 33) Right = Geography.City;
        if (id == 14 || id == 24 || id == 32) Down = Geography.City;
        if (id == 12 || id == 17 || id == 20 || id == 21 || id == 22 || id == 23 || id == 24) Left = Geography.City;
        if (id == 26 || id == 28 || id == 29 || id == 31 || id == 32) Right = Geography.Stream;
        if (id == 25 || id == 27 || id == 30 || id == 33) Down = Geography.Stream;
        if (id == 26 || id == 27 || id == 28 || id == 29 || id == 30 || id == 31 || id == 33) Left = Geography.Stream;
        if (id == 1 || id == 2 || id == 28) Center = Geography.Cloister;
        if (id == 3 || id == 4 || id == 8 || id == 9 || id == 10 || id == 29 || id == 30) Center = Geography.Road;
        if (id == 5 || id == 6 || id == 11) Center = Geography.Village;
        if (id == 7 || id == 14 || id == 15 || id == 32) Center = Geography.Grass;
        if (id == 12 || id == 13 || id == 17 || id == 18 || id == 20 || id == 21 || id == 22 || id == 23 || id == 24 ||
            id == 31) Center = Geography.City;
        if (id == 33) Center = Geography.CityStream;
        if (id == 16 || id == 19) Center = Geography.CityRoad;
        if (id == 17 || id == 18 || id == 19 || id == 22 || id == 23 || id == 24)
            shield = true;
        else
            shield = false;

        North = Up;
        East = Right;
        South = Down;
        West = Left;
    }


    /// <summary>
    ///     The method used to rotate the tile. In essence it just cycles the rotation between 1 and 3 (and returns to 0 when
    ///     rotated after 3), and switches the north east south west values clockwise.
    /// </summary>
    public void Rotate()
    {
        rotation++;
        if (rotation > 3) rotation = 0;
        var res = North;
        North = West;
        West = South;
        South = East;
        East = res;
    }

}
