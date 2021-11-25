using System.Collections.Generic;
using Carcassonne;

public class PlayerState
{
    public List<Player> All = new List<Player>();

    // Derived Properties
    public List<Meeple> Meeples => new List<Meeple>();

}
