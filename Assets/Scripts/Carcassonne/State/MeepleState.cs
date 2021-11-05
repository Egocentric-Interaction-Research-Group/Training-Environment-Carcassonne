using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Carcassonne.State
{
    /// <summary>
    /// MeepleState hold all of the information about the position, availability, and ownership of meeples.
    /// Player meeple list derive from this information store.
    /// </summary>
    [CreateAssetMenu(fileName = "MeepleState", menuName = "State/MeepleState")]
    public class MeepleState : ScriptableObject
    {
        public List<Meeple> All= new List<Meeple>();
        [CanBeNull] public Meeple Current;

        private void OnEnable()
        {
            All.Clear();

            Current = null;
            
        }

        public List<Meeple> MeeplesForPlayer(int id)
        {
            return (from meeple in All where meeple.playerId == id select meeple).ToList();
        }
    }
}