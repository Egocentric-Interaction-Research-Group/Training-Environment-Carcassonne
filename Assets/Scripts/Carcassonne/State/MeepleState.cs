using System.Collections.Generic;
using System.Linq;
using Carcassonne;
using JetBrains.Annotations;

public class MeepleState
{
    public List<Meeple> All = new List<Meeple>();
    [CanBeNull] public Meeple Current;
    public List<Meeple> MeeplesForPlayer(int id)
    {
        return (from meeple in All where meeple.playerId == id select meeple).ToList();
    }
}
