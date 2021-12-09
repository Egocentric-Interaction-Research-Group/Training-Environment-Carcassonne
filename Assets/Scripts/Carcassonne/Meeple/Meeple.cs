using Carcassonne;

/// <summary>
/// Represents meeples in the Carcassone game
/// Every meeple is placed in a geography and a direction.
/// They are also tied to their owner (Player), by id. 
/// Meeple use is based around meeple being free or not (see boolean)
/// </summary>
public class Meeple
{
    public Point.Direction direction;
    public Tile.Geography geography;

    public int playerId;

    public bool free;

    public int x = 0, z = 0;

    /// <summary>
    /// Assigns attributes to a meeple. Free is always set to false when a meeple has been placed on a geography
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="direction"></param>
    /// <param name="geography"></param>
    public void assignAttributes(int x, int z, Point.Direction direction, Tile.Geography geography)
    {
        this.direction = direction;
        this.geography = geography;

        this.x = x;
        this.z = z;
        free = false;
    }
}

