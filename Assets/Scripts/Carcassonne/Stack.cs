﻿using System.Collections.Generic;
using System.Linq;
using Carcassonne.State;
using JetBrains.Annotations;

using UnityEngine;
using Random = System.Random;

// TODO: Why not use an actual Stack object? Or two lists?

// Game State requires a count of Tiles.Remaining tiles, and a list of Tiles.Remaining tiles. Possible search by criteria?

namespace Carcassonne
{
    /// <summary>
    ///     The Stack of tiles.
    /// </summary>
    public class StackScript : MonoBehaviour
    {
        /// <summary>
        ///     A reference to the prefab Tile, to be used later.
        /// </summary>
        public GameObject tile;

        public Transform basePositionTransform;

        public int[] randomIndexArray;

        /// <summary>
        ///     The array of tiles
        /// </summary>
        public List<GameObject> tileArray;

        public GameObject firstTile;
        
        public TileState tiles;
        
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public GameObject Pop()
        {
            var rand = new Random();
            var idx = rand.Next(tiles.Remaining.Count);

            tiles.Current = tiles.Remaining[idx];
            tiles.Remaining.Remove(tiles.Current);

            return tiles.Current.gameObject;
        }

        public bool isEmpty()
        {
            return tiles.Remaining.Count == 0;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public StackScript createStackScript()
        {
            //setAll();
            return this;
        }
        
        /// <summary>
        /// Populates the array of tiles. Finds all game objects tagged tile. The Master client sets 
        /// </summary>
        public void PopulateTileArray()
        {
            //randomIndex = new int[84];
            tileArray = new List<GameObject>(GameObject.FindGameObjectsWithTag("Tile"));
            // Filter out tiles not in set. TODO: This should reference the game rules and pick relevant sets.
            tileArray = tileArray.Where(t => t.GetComponent<TileScript>().tileSet == TileScript.TileSet.Base && t != firstTile ).ToList();
            
            // This probably indicates that I've coded something incorrectly.
            tiles.Remaining.Clear(); // Remove all remaining tiles from old games so that they do not persist.

            foreach (var t in tileArray)
            {
                tiles.Remaining.Add(t.GetComponent<TileScript>());
            }
            
            Debug.Log($"Tile array is populated. {tiles.Remaining.Count} items remain in the stack.");

        }

    }
}