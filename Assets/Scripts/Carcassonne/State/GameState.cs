using System;
using UnityEngine;

namespace Carcassonne.State
{
    [CreateAssetMenu(fileName = "GameState", menuName = "ScriptableObjects/GameState")]
    public class GameState : ScriptableObject
    {
        public GameRules Rules;

        /// <summary>
        /// Describes what is happening currently in the game.
        /// </summary>
        public Phase phase;

        public TileState Tiles;
        public MeepleState Meeples;
        public PlayerState Players;

        public GameState()
        {
            Rules = new GameRules();
        }
    }
}