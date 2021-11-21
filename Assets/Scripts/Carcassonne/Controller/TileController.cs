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
        {
            if (tiles.Current is null)
                return null;
            return tiles.Current.gameObject;
        }
        set => tiles.Current = value.GetComponent<Tile>();
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
