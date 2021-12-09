using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

/// <summary>
/// This script represent the state of all meeples held by players
/// </summary>
public class MeepleState
{
    public List<Meeple> All = new List<Meeple>(); //List of every meeple both on the board and off
    [CanBeNull] public Meeple Current; //Current active meeple of the current player

    /// <summary>
    /// Retrieves a list of all the meeples that match a player id
    /// </summary>
    /// <param name="id">player id</param>
    /// <returns>List of meeple objects</returns>
    public List<Meeple> MeeplesForPlayer(int id)
    {
        return (from meeple in All where meeple.playerId == id select meeple).ToList();
    }
}
