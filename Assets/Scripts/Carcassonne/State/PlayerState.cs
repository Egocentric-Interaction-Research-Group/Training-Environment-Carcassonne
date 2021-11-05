using System.Collections.Generic;
using UnityEngine;

namespace Carcassonne.State
{
    [CreateAssetMenu(fileName = "PlayerState", menuName = "State/PlayerState")]
    public class PlayerState : ScriptableObject
    {
        public List<Player> All = new List<Player>();
        public Player Current;
        
        // Derived Properties
        public List<Meeple> Meeples => new List<Meeple>();
        
        private void OnEnable()
        {
            All.Clear();
            Current = null;
        }
    }
}