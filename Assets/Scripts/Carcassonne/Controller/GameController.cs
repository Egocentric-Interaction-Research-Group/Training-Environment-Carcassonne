using Carcassonne;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Carcassonne.State;
public class GameController : MonoBehaviour
{
    public enum Phases
    {
        NewTurn,
        TileDrawn,
        TileDown,
        MeepleDrawn,
        MeepleDown,
        GameOver
    }

    public GameState gameState;

    public bool startGame, pcRotate, isManipulating;

    public GameObject trainingTable;

    [FormerlySerializedAs("state")] public Phases phase;

    private bool cityIsFinished;

    private Carcassonne.PointScript.Direction direction;


    public int iTileAimX, iTileAimZ;

    private int NewTileRotation;

    private int tempX;
    private int tempY;

    private int VertexItterator;

    private bool[,] visited;

    private Carcassonne.PlacedTilesScript placedTiles;

    internal Player currentPlayer;

    public Carcassonne.StackScript stackScript;

    public Player PlayerTest
    {
        set => currentPlayer = value;
        get => currentPlayer;
    }

    public Carcassonne.PlacedTilesScript PlacedTiles
    {
        set => placedTiles = value;
        get => placedTiles;
    }

    public Carcassonne.TileControllerScript TileControllerScript
    {
        set => TileControllerScript = value;
        get => TileControllerScript;
    }

    public Carcassonne.PointScript.Direction Direction
    {
        set => direction = value;
        get => direction;
    }

    [SerializeField]
    internal MeepleControllerScript meepleControllerScript;

    [SerializeField]
    internal TileControllerScript tileControllerScript;
    private int firstTurnCounter;

    private void FixedUpdate()
    {
        if (startGame)
        {
            NewGame();
            startGame = false;
        }

        else if (gameState.phase == Phase.GameOver)
        {
            //Säg till agenterna att resetta och starta om en ny runda
        }
    }

    public void NewGame()
    {
        
        placedTiles = GetComponent<PlacedTilesScript>();
        placedTiles.InstansiatePlacedTilesArray();

        trainingTable = GameObject.Find("TrainingTable");

        stackScript = GetComponent<StackScript>().createStackScript();

        stackScript.PopulateTileArray();

        currentPlayer = GetComponent<Player>();

        BaseTileCreation();
        for (int i = 0; i < 2; i++)
        {
            gameState.Players.All.Add(GameObject.Find("AI" + i).GetComponent<Player>());
        }

        PlaceTile(tileControllerScript.currentTile, 85, 85, true);

        currentPlayer = gameState.Players.All[0];

        gameState.phase = Phase.NewTurn;
    }

    private void BaseTileCreation()
    {
        tileControllerScript.currentTile = stackScript.firstTile;
        tileControllerScript.currentTile.transform.parent = trainingTable.transform;
    }

    public bool CityIsFinishedDirection(int x, int y, PointScript.Direction direction)
    {
        meepleControllerScript.MeeplesInCity = new List<MeepleScript>();
        meepleControllerScript.MeeplesInCity.Add(meepleControllerScript.FindMeeple(x, y, TileScript.Geography.City, direction, this));

        cityIsFinished = true;
        visited = new bool[170, 170];
        RecursiveCityIsFinishedDirection(x, y, direction);
        Debug.Log(
            "DIRECTION__________________________CITY IS FINISHED EFTER DIRECTION REKURSIV: ___________________________" +
            cityIsFinished + " X: " + x + " Z: " + y + " MEEPLEINCITY: " + meepleControllerScript.FindMeeple(x, y, TileScript.Geography.City, this));
        return cityIsFinished;
    }

    public bool CityIsFinished(int x, int y)
    {
        meepleControllerScript.MeeplesInCity = new List<MeepleScript>();
        meepleControllerScript.MeeplesInCity.Add(meepleControllerScript.FindMeeple(x, y, TileScript.Geography.City, this));


        cityIsFinished = true;
        visited = new bool[170, 170];
        RecursiveCityIsFinished(x, y);
        Debug.Log("__________________________CITY IS FINISHED EFTER REKURSIV: ___________________________" +
                  cityIsFinished + " X: " + x + " Z: " + y + " MEEPLEINCITY: " + meepleControllerScript.FindMeeple(x, y, TileScript.Geography.City, this));

        return cityIsFinished;
    }

    public void RecursiveCityIsFinishedDirection(int x, int y, PointScript.Direction direction)
    {
        visited[x, y] = true;
        if (direction == PointScript.Direction.NORTH)
            if (placedTiles.getPlacedTiles(x, y).GetComponent<TileScript>().North == TileScript.Geography.City)
            {
                if (placedTiles.getPlacedTiles(x, y + 1) != null)
                {
                    if (!visited[x, y + 1]) RecursiveCityIsFinished(x, y + 1);
                }
                else
                {
                    cityIsFinished = false;
                }
            }

        if (direction == PointScript.Direction.EAST)
            if (placedTiles.getPlacedTiles(x, y).GetComponent<TileScript>().East == TileScript.Geography.City)
            {
                if (placedTiles.getPlacedTiles(x + 1, y) != null)
                {
                    if (!visited[x + 1, y]) RecursiveCityIsFinished(x + 1, y);
                }
                else
                {
                    cityIsFinished = false;
                }
            }

        if (direction == PointScript.Direction.SOUTH)
            if (placedTiles.getPlacedTiles(x, y).GetComponent<TileScript>().South == TileScript.Geography.City)
            {
                if (placedTiles.getPlacedTiles(x, y - 1) != null)
                {
                    if (!visited[x, y - 1]) RecursiveCityIsFinished(x, y - 1);
                }
                else
                {
                    cityIsFinished = false;
                }
            }

        if (direction == PointScript.Direction.WEST)
            if (placedTiles.getPlacedTiles(x, y).GetComponent<TileScript>().West == TileScript.Geography.City)
            {
                if (placedTiles.getPlacedTiles(x - 1, y) != null)
                {
                    if (!visited[x - 1, y]) RecursiveCityIsFinished(x - 1, y);
                }
                else
                {
                    cityIsFinished = false;
                }
            }
    }

    public bool TileCanBePlaced(TileScript script)
    {
        for (var i = 0; i < placedTiles.GetLength(0); i++)
            for (var j = 0; j < placedTiles.GetLength(1); j++)
                if (placedTiles.HasNeighbor(i, j) && placedTiles.getPlacedTiles(i, j) == null)
                    for (var k = 0; k < 4; k++)
                    {
                        if (placedTiles.MatchGeographyOrNull(i - 1, j, PointScript.Direction.EAST, script.West))
                            if (placedTiles.MatchGeographyOrNull(i + 1, j, PointScript.Direction.WEST, script.East))
                                if (placedTiles.MatchGeographyOrNull(i, j - 1, PointScript.Direction.NORTH, script.South))
                                    if (placedTiles.MatchGeographyOrNull(i, j + 1, PointScript.Direction.SOUTH,
                                        script.North))
                                    {
                                        ResetTileRotation();
                                        return true;
                                    }

                        RotateTile();
                    }

        ResetTileRotation();
        return false;
    }

    public void RecursiveCityIsFinished(int x, int y)
    {
        visited[x, y] = true;


        if (placedTiles.getPlacedTiles(x, y) != null)
        {
            if (placedTiles.getPlacedTiles(x, y).GetComponent<TileScript>().North == TileScript.Geography.City)
                if (!placedTiles.CityTileHasGrassOrStreamCenter(x, y))
                {
                    if (placedTiles.getPlacedTiles(x, y + 1) != null)

                    {
                        if (!visited[x, y + 1]) RecursiveCityIsFinished(x, y + 1);
                    }
                    else
                    {
                        cityIsFinished = false;
                    }
                }

            if (placedTiles.getPlacedTiles(x, y).GetComponent<TileScript>().East == TileScript.Geography.City)
                if (!placedTiles.CityTileHasGrassOrStreamCenter(x, y))
                {
                    if (placedTiles.getPlacedTiles(x + 1, y) != null)
                    {
                        if (!visited[x + 1, y]) RecursiveCityIsFinished(x + 1, y);
                    }
                    else
                    {
                        cityIsFinished = false;
                    }
                }

            if (placedTiles.getPlacedTiles(x, y).GetComponent<TileScript>().South == TileScript.Geography.City)
                if (!placedTiles.CityTileHasGrassOrStreamCenter(x, y))
                {
                    if (placedTiles.getPlacedTiles(x, y - 1) != null)
                    {
                        if (!visited[x, y - 1]) RecursiveCityIsFinished(x, y - 1);
                    }
                    else
                    {
                        cityIsFinished = false;
                    }
                }

            if (placedTiles.getPlacedTiles(x, y).GetComponent<TileScript>().West == TileScript.Geography.City)
                if (!placedTiles.CityTileHasGrassOrStreamCenter(x, y))
                {
                    if (placedTiles.getPlacedTiles(x - 1, y) != null)
                    {
                        if (!visited[x - 1, y]) RecursiveCityIsFinished(x - 1, y);
                    }
                    else
                    {
                        cityIsFinished = false;
                    }
                }
        }
    }

    public void PlaceTile(GameObject tile, int x, int z, bool firstTile)
    {
        tempX = x;
        tempY = z;
        tile.GetComponent<TileScript>().vIndex = VertexItterator;

        GetComponent<PointScript>().placeVertex(VertexItterator, placedTiles.GetNeighbors(tempX, tempY),
            placedTiles.getWeights(tempX, tempY), tileControllerScript.currentTile.GetComponent<TileScript>().getCenter(),
            placedTiles.getCenters(tempX, tempY), placedTiles.getDirections(tempX, tempY));

        VertexItterator++;



        placedTiles.PlaceTile(x, z, tile);




        calculatePoints(false, false);
    }

    //Metod för att plocka upp en ny tile
    public void PickupTile()
    {
        if (gameState.phase == Phase.NewTurn)
        {
            stackScript.Pop();
            if (!TileCanBePlaced(gameState.Tiles.Current))
            {
                Debug.Log("Tile not possible to place: discarding and drawing a new one. " + "Tile id: " + tileControllerScript.currentTile.GetComponent<TileScript>().id);
                Destroy(tileControllerScript.currentTile);
                PickupTile();
            }
            else
            {
                ResetTileRotation();
                gameState.phase = Phase.TileDrawn;
            }
        }
    }

    public void ConfirmPlacement()
    {
        if (gameState.phase == Phase.TileDrawn)
        {
            if (placedTiles.TilePlacementIsValid(tileControllerScript.currentTile, iTileAimX, iTileAimZ))
            {
                PlaceTile(tileControllerScript.currentTile, iTileAimX, iTileAimZ, false);

                gameState.phase = Phase.TileDown;
            }
            else if (!placedTiles.TilePlacementIsValid(tileControllerScript.currentTile, iTileAimX, iTileAimZ))
            {
                Debug.Log("Tile cant be placed");
            }
        }
        else if (gameState.phase == Phase.MeepleDrawn)
        {
            if (gameState.Meeples.Current != null)
            {
               
                {
                    if (meepleControllerScript.meepleGeography == TileScript.Geography.City ||
                        meepleControllerScript.meepleGeography == TileScript.Geography.Cloister ||
                        meepleControllerScript.meepleGeography == TileScript.Geography.Road)
                    {
                        meepleControllerScript.PlaceMeeple(gameState.Meeples.Current.gameObject,
                            meepleControllerScript.iMeepleAimX, meepleControllerScript.iMeepleAimZ,
                            Direction, meepleControllerScript.meepleGeography, this);
                    }
                }
               
            }
        }
    }

    public void EndTurn()
    {
        if (gameState.phase == Phase.TileDown || gameState.phase == Phase.MeepleDown)
        {
            calculatePoints(true, false);
            NewTileRotation = 0;
            if (stackScript.isEmpty())
            {
                GameOver();
            }
            else
            {
                if (gameState.Players.All.Count > 1)
                {
                    if (currentPlayer == gameState.Players.All[0])
                        currentPlayer = gameState.Players.All[1];
                    else
                        currentPlayer = gameState.Players.All[0];
                }

                if (firstTurnCounter != 0) firstTurnCounter -= 1;

                gameState.phase = Phase.NewTurn;
            }
        }
    }

    public void calculatePoints(bool RealCheck, bool GameEnd)
    {
        foreach (var p in gameState.Players.All)
            for (var j = 0; j < p.Meeples.Length; j++)
            {
                var meeple = p.Meeples[j].GetComponent<MeepleScript>();

                var tileID = placedTiles.getPlacedTiles(meeple.x, meeple.z).GetComponent<TileScript>().id;
                var finalscore = 0;
                if (meeple.geography == TileScript.Geography.City)
                {
                    //CITY DIRECTION
                    if (placedTiles.getPlacedTiles(meeple.x, meeple.z).GetComponent<TileScript>().getCenter() ==
                        TileScript.Geography.Stream ||
                        placedTiles.getPlacedTiles(meeple.x, meeple.z).GetComponent<TileScript>().getCenter() ==
                        TileScript.Geography.Grass ||
                        placedTiles.getPlacedTiles(meeple.x, meeple.z).GetComponent<TileScript>().getCenter() ==
                        TileScript.Geography.Road ||
                        placedTiles.getPlacedTiles(meeple.x, meeple.z).GetComponent<TileScript>().getCenter() ==
                        TileScript.Geography.Village)
                    {
                        if (CityIsFinishedDirection(meeple.x, meeple.z, meeple.direction))
                        {

                            finalscore = GetComponent<PointScript>()
                                .startDfsDirection(
                                    placedTiles.getPlacedTiles(meeple.x, meeple.z).GetComponent<TileScript>()
                                        .vIndex, meeple.geography, meeple.direction, GameEnd);
                        }

                        //else
                        //{
                        //    GetComponent<PointScript>().startDfsDirection(placedTiles.getPlacedTiles(meeple.x, meeple.z).
                        //        GetComponent<TileScript>().vIndex, meeple.geography, meeple.direction, GameEnd);
                        //}
                        if (GameEnd)
                        {
                            finalscore = GetComponent<PointScript>()
                                .startDfsDirection(
                                    placedTiles.getPlacedTiles(meeple.x, meeple.z).GetComponent<TileScript>()
                                        .vIndex, meeple.geography, meeple.direction, GameEnd);
                        }
                    }
                    else
                    {
                        //CITY NO DIRECTION
                        if (CityIsFinished(meeple.x, meeple.z))
                            finalscore = GetComponent<PointScript>()
                                .startDfs(
                                    placedTiles.getPlacedTiles(meeple.x, meeple.z).GetComponent<TileScript>()
                                        .vIndex, meeple.geography, GameEnd);
                        if (GameEnd)
                        {
                            Debug.Log("GAME END I ELSE");
                            finalscore = GetComponent<PointScript>()
                                .startDfsDirection(
                                    placedTiles.getPlacedTiles(meeple.x, meeple.z).GetComponent<TileScript>()
                                        .vIndex, meeple.geography, meeple.direction, GameEnd);
                        }
                    }
                }
                else
                {
                    ///ROAD
                    if (placedTiles.getPlacedTiles(meeple.x, meeple.z).GetComponent<TileScript>().getCenter() ==
                        TileScript.Geography.Village ||
                        placedTiles.getPlacedTiles(meeple.x, meeple.z).GetComponent<TileScript>().getCenter() ==
                        TileScript.Geography.Grass)
                    {
                        finalscore = GetComponent<PointScript>().startDfsDirection(placedTiles
                            .getPlacedTiles(meeple.x, meeple.z)
                            .GetComponent<TileScript>().vIndex, meeple.geography, meeple.direction, GameEnd);
                        if (GameEnd)
                            finalscore--;
                    }
                    else
                    {
                        finalscore = GetComponent<PointScript>()
                            .startDfs(
                                placedTiles.getPlacedTiles(meeple.x, meeple.z).GetComponent<TileScript>().vIndex,
                                meeple.geography, GameEnd);
                        if (GameEnd)
                            finalscore--;
                    }

                    //CLOISTER
                    if (placedTiles.getPlacedTiles(meeple.x, meeple.z).GetComponent<TileScript>().getCenter() ==
                        TileScript.Geography.Cloister &&
                        meeple.direction == PointScript.Direction.CENTER)
                        finalscore = placedTiles.CheckSurroundedCloister(meeple.x, meeple.z, GameEnd);
                }

                if (finalscore > 0 && RealCheck)
                {
                    p.Score += finalscore;
                }
            }
    }
    public void RotateTile()
    {
        if (gameState.phase == Phase.TileDrawn)
        {
            NewTileRotation++;
            if (NewTileRotation > 3) NewTileRotation = 0;
            gameState.Tiles.Current.Rotate();

            if (pcRotate) tileControllerScript.currentTile.transform.Rotate(0.0f, 90.0f, 0.0f, Space.Self);
        }
    }

    public void ResetTileRotation()
    {
        NewTileRotation = 0;
        gameState.Tiles.Current.rotation = 0;
    }

    private void GameOver()
    {
        calculatePoints(true, true);
        gameState.phase = Phase.GameOver;
    }

    public void ChangeStateToNewTurn()
    {
        gameState.phase = Phase.NewTurn;
    }

}
