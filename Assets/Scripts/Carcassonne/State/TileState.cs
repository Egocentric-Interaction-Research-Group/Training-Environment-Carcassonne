using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Carcassonne.State
{
    [CreateAssetMenu(fileName = "TileState", menuName = "ScriptableObjects/TileState")]
    public class TileState : ScriptableObject
    {
        public List<Tile> Remaining;
        [CanBeNull] public Tile Current;
        public Tile[,] Played;
        public float[,] PlayedId;

        private void OnEnabled()
        {
            Remaining = new List<Tile>();
            Current = null;
        }
    }
}