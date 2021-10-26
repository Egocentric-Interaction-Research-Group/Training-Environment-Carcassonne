using System;
using System.Collections.Generic;
using Carcassonne.State;

using UnityEngine;

namespace Carcassonne
{
    public class MeepleControllerScript : MonoBehaviour
    {
    
        [SerializeField]
        internal GameController gameControllerScript;
        [HideInInspector] public List<MeepleScript> MeeplesInCity;
        internal float fMeepleAimX; //TODO Make Private
        internal float fMeepleAimZ; //TODO Make Private

        private int meepleCount = 0;

        public MeepleState meeples;
        public PlayerState players;

        public MeepleControllerScript(GameController gameControllerScript)
        {
            this.gameControllerScript = gameControllerScript;
        }
        
        // Instantiation Stuff
        public GameObject prefab;
        public GameObject parent;
        
        /// <summary>
        /// Instantiate a new Meeple with the chosen prefab and parent object in the hierarchy.
        /// </summary>
        /// <returns>GameObject : An instance of MeepleScript.prefab.</returns>
        public MeepleScript GetNewInstance()
        {
            // return Instantiate(prefab, meepleSpawnPosition.transform.position, Quaternion.identity, GameObject.Find("Table").transform).GetComponent<MeepleScript>();
            meepleCount++;
            GameObject newMeeple = Instantiate(prefab, parent.transform.position, Quaternion.identity);//, GameObject.Find("Table").transform);
            newMeeple.gameObject.transform.parent = parent.transform;
            newMeeple.gameObject.name = $"Meeple {meepleCount}";
            newMeeple.SetActive(false);

            return newMeeple.GetComponent<MeepleScript>();
        }

    
        public ParticleSystem drawMeepleEffect;
        [HideInInspector] public GameObject meepleMesh;
        [HideInInspector] public GameObject MeeplePrefab;
        public GameObject meepleSpawnPosition;
        internal int iMeepleAimX;
        internal int iMeepleAimZ;
        public TileScript.Geography meepleGeography;
        public RaycastHit meepleHitTileDirection;

        public MeepleScript FindMeeple(int x, int y, TileScript.Geography geography, GameController gameControllerScript)
        {
            MeepleScript res = null;

            foreach (var m in meeples.All)
            {
                var tmp = m;

                if (tmp.geography == geography && tmp.x == x && tmp.z == y) return tmp;
            }

            return res;
        }

        public MeepleScript FindMeeple(int x, int y, TileScript.Geography geography, PointScript.Direction direction, GameController gameControllerScript)
        {
            MeepleScript res = null;

            foreach (var m in meeples.All)
            {
                var tmp = m;

                if (tmp.geography == geography && tmp.x == x && tmp.z == y && tmp.direction == direction) return tmp;
            }

            return res;
        }

        public void PlaceMeeple(GameObject meeple, int xs, int zs, PointScript.Direction direction,
            TileScript.Geography meepleGeography, GameController gameControllerScript)
        {
            var currentTileScript = gameControllerScript.TileControllerScript.currentTile.GetComponent<TileScript>();
            var currentCenter = currentTileScript.getCenter();
            bool res;
            if (currentCenter == TileScript.Geography.Village || currentCenter == TileScript.Geography.Grass ||
                currentCenter == TileScript.Geography.Cloister && direction != PointScript.Direction.CENTER)
                res = GetComponent<PointScript>()
                    .testIfMeepleCantBePlacedDirection(currentTileScript.vIndex, meepleGeography, direction);
            else if (currentCenter == TileScript.Geography.Cloister && direction == PointScript.Direction.CENTER)
                res = false;
            else
                res = GetComponent<PointScript>().testIfMeepleCantBePlaced(currentTileScript.vIndex, meepleGeography);

            if (meepleGeography == TileScript.Geography.City)
            {
                if (currentCenter == TileScript.Geography.City)
                    res = gameControllerScript.CityIsFinished(xs, zs) || res;
                else
                    res = gameControllerScript.CityIsFinishedDirection(xs, zs, direction) || res;
            }

            if (!currentTileScript.IsOccupied(direction) && !res)
            {

                currentTileScript.occupy(direction);
                if (meepleGeography == TileScript.Geography.CityRoad) meepleGeography = TileScript.Geography.City;

                meeple.GetComponent<MeepleScript>().assignAttributes(xs, zs, direction, meepleGeography);


                gameControllerScript.gameState.phase = Phase.MeepleDown;
            }
        }
    }
}