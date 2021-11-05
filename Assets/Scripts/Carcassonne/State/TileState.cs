using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Carcassonne.State
{
    [CreateAssetMenu(fileName = "TileState", menuName = "State/TileState")]
    public class TileState : ScriptableObject
    {
        public List<Tile> Remaining;
        [CanBeNull] public Tile Current;
        public Tile[,] Played;

        private void Awake()
        {
            Remaining = new List<Tile>();
            Current = null;
        }
    }
}