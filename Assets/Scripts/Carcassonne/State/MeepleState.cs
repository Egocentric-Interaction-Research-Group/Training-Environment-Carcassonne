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
    [CreateAssetMenu(fileName = "MeepleState", menuName = "States/MeepleState")]
    public class MeepleState : ScriptableObject
    {
        public List<MeepleScript> All= new List<MeepleScript>();
        [CanBeNull] public MeepleScript Current;

        private void OnEnable()
        {
            All.Clear();

            Current = null;
            
        }
    }
}