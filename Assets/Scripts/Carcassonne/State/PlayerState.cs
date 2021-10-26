using System.Collections.Generic;
using UnityEngine;

namespace Carcassonne.State
{
    [CreateAssetMenu(fileName = "PlayerState", menuName = "States/PlayerState")]
    public class PlayerState : ScriptableObject
    {
        public List<Player> All = new List<Player>();
        public Player Current;
        
        // Derived Properties
        public List<MeepleScript> Meeples => new List<MeepleScript>();
        
        private void OnEnable()
        {
            All.Clear();
            Current = null;
        }
    }
}