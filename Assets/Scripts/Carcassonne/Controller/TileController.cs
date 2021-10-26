using Carcassonne.State;
using JetBrains.Annotations;

using UnityEngine;

namespace Carcassonne
{
    public class TileControllerScript : MonoBehaviour
    {
        private GameController gameControllerScript;
        public Vector3 currentTileEulersOnManip;
        public ParticleSystem drawTileEffect;
        public TileState tiles;
        
        [CanBeNull]
        public GameObject currentTile
        {
            get
            {
                if (tiles.Current is null)
                    return null;
                return tiles.Current.gameObject;
            }
            set => tiles.Current = value.GetComponent<TileScript>();
        }

        public GameObject drawTile;
        public GameObject tileSpawnPosition;
        public float fTileAimX;
        public float fTileAimZ;

        public TileControllerScript(GameController gameControllerScript)
        {
            this.gameControllerScript = gameControllerScript;
        }

    }
}