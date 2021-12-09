/// <summary>
/// Describes different phases of gameplay.
/// </summary>
public enum Phase
{
    NewTurn, //Current player rotates
    TileDrawn, //Tile is drawn from the stack
    TileDown, //Tile is correctly placed on the board
    MeepleDrawn, //Meeple is drawn from current players inventory
    MeepleDown, //Meeple is correctly placed on current tile
    GameOver //Tile stack is empty, the game has ended
}
