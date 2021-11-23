using Carcassonne;
using JetBrains.Annotations;

using UnityEngine;

public class TileController : MonoBehaviour
{
    private GameController gameController;
    public Vector3 currentTileEulersOnManip;
    public ParticleSystem drawTileEffect;
    public TileState tiles;

    [CanBeNull]
    public GameObject currentTile
    {
        get
        {   if(tiles.Current == null)
            {
                return null;
            }        
            return tiles.Current.gameObject;
        }
        set
        {   if(value == null)
            {
                tiles.Current = null;
            }
            else
            {
                tiles.Current = value.GetComponent<Tile>();
            }         
        }
    }

    public GameObject drawTile;
    public GameObject tileSpawnPosition;
    public float fTileAimX;
    public float fTileAimZ;

    public TileController(GameController gameController)
    {
        this.gameController = gameController;
    }

}
