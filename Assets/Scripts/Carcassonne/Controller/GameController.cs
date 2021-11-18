using Carcassonne;
using System.Collections.Generic;
using UnityEngine;
using Carcassonne.State;
using Unity.MLAgents;

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

    public bool startGame, pcRotate, isManipulating, cityIsFinished;

    private bool[,] visited;

    public int iTileAimX, iTileAimZ;

    private int NewTileRotation, tempX, tempY, VertexItterator, tileCounter;

    public GameState gameState;

    private CarcassonneVisualization shader;

    private Point.Direction direction;

    public GameObject ai, visualizationBoard;

    public PlacedTiles placedTiles;

    internal Player currentPlayer;

    public Stack stack;

    public Point point;

    public PlacedTiles PlacedTiles
    {
        set => placedTiles = value;
        get => placedTiles;
    }

    public TileController TileController
    {
        set => TileController = value;
        get => TileController;
    }

    public Point.Direction Direction
    {
        set => direction = value;
        get => direction;
    }

    [SerializeField]
    internal MeepleController meepleController;

    [SerializeField]
    internal TileController tileController;
    private int firstTurnCounter;

    private void Start()
    {
        shader = visualizationBoard.GetComponent<CarcassonneVisualization>();
    }

    private void FixedUpdate()
    {
        if (startGame)
        {
            NewGame();
            startGame = false;
        }

        else if (gameState.phase == Phase.GameOver)
        {
            startGame = true;
        }
    }

    public void NewGame()
    {
        tileCounter = 0;     

        placedTiles.InstansiatePlacedTilesArray();
        stack.PopulateTileArray();
        BaseTileCreation();
        PlaceTile(tileController.currentTile, 85, 85, true);
        for (int i = 0; i < 1; i++)
        {          
            Player player = Instantiate(ai).GetComponent<Player>();
            player.id = 0;
            player.meepleState = gameState.Meeples;
            player.Setup();       
            if (i == 0)
            {
                currentPlayer = player;
            }
            gameState.Players.All.Add(player);
        }
        gameState.phase = Phase.NewTurn;
    }

    private void BaseTileCreation()
    {
        tileController.currentTile = stack.firstTile;
    }

    public bool CityIsFinishedDirection(int x, int y, Point.Direction direction)
    {
        meepleController.MeeplesInCity = new List<Meeple>();
        meepleController.MeeplesInCity.Add(meepleController.FindMeeple(x, y, Tile.Geography.City, direction));

        cityIsFinished = true;
        visited = new bool[170, 170];
        RecursiveCityIsFinishedDirection(x, y, direction);
        Debug.Log(
            "DIRECTION__________________________CITY IS FINISHED EFTER DIRECTION REKURSIV: ___________________________" +
            cityIsFinished + " X: " + x + " Z: " + y + " MEEPLEINCITY: " + meepleController.FindMeeple(x, y, Tile.Geography.City));
        return cityIsFinished;
    }

    public bool CityIsFinished(int x, int y)
    {
        meepleController.MeeplesInCity = new List<Meeple>();
        meepleController.MeeplesInCity.Add(meepleController.FindMeeple(x, y, Tile.Geography.City));


        cityIsFinished = true;
        visited = new bool[170, 170];
        RecursiveCityIsFinished(x, y);
        Debug.Log("__________________________CITY IS FINISHED EFTER REKURSIV: ___________________________" +
                  cityIsFinished + " X: " + x + " Z: " + y + " MEEPLEINCITY: " + meepleController.FindMeeple(x, y, Tile.Geography.City));

        return cityIsFinished;
    }

    public void RecursiveCityIsFinishedDirection(int x, int y, Point.Direction direction)
    {
        visited[x, y] = true;
        if (direction == Point.Direction.NORTH)
            if (placedTiles.getPlacedTile(x, y).GetComponent<Tile>().North == Tile.Geography.City)
            {
                if (placedTiles.getPlacedTile(x, y + 1) != null)
                {
                    if (!visited[x, y + 1]) RecursiveCityIsFinished(x, y + 1);
                }
                else
                {
                    cityIsFinished = false;
                }
            }

        if (direction == Point.Direction.EAST)
            if (placedTiles.getPlacedTile(x, y).GetComponent<Tile>().East == Tile.Geography.City)
            {
                if (placedTiles.getPlacedTile(x + 1, y) != null)
                {
                    if (!visited[x + 1, y]) RecursiveCityIsFinished(x + 1, y);
                }
                else
                {
                    cityIsFinished = false;
                }
            }

        if (direction == Point.Direction.SOUTH)
            if (placedTiles.getPlacedTile(x, y).GetComponent<Tile>().South == Tile.Geography.City)
            {
                if (placedTiles.getPlacedTile(x, y - 1) != null)
                {
                    if (!visited[x, y - 1]) RecursiveCityIsFinished(x, y - 1);
                }
                else
                {
                    cityIsFinished = false;
                }
            }

        if (direction == Point.Direction.WEST)
            if (placedTiles.getPlacedTile(x, y).GetComponent<Tile>().West == Tile.Geography.City)
            {
                if (placedTiles.getPlacedTile(x - 1, y) != null)
                {
                    if (!visited[x - 1, y]) RecursiveCityIsFinished(x - 1, y);
                }
                else
                {
                    cityIsFinished = false;
                }
            }
    }

    public bool TileCanBePlaced(Tile script)
    {
        for (var i = 0; i < placedTiles.GetLength(0); i++)
            for (var j = 0; j < placedTiles.GetLength(1); j++)
                if (placedTiles.HasNeighbor(i, j) && placedTiles.getPlacedTile(i, j) == null)
                    for (var k = 0; k < 4; k++)
                    {
                        if (placedTiles.MatchGeographyOrNull(i - 1, j, Point.Direction.EAST, script.West))
                            if (placedTiles.MatchGeographyOrNull(i + 1, j, Point.Direction.WEST, script.East))
                                if (placedTiles.MatchGeographyOrNull(i, j - 1, Point.Direction.NORTH, script.South))
                                    if (placedTiles.MatchGeographyOrNull(i, j + 1, Point.Direction.SOUTH,
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


        if (placedTiles.getPlacedTile(x, y) != null)
        {
            if (placedTiles.getPlacedTile(x, y).GetComponent<Tile>().North == Tile.Geography.City)
                if (!placedTiles.CityTileHasGrassOrStreamCenter(x, y))
                {
                    if (placedTiles.getPlacedTile(x, y + 1) != null)

                    {
                        if (!visited[x, y + 1]) RecursiveCityIsFinished(x, y + 1);
                    }
                    else
                    {
                        cityIsFinished = false;
                    }
                }

            if (placedTiles.getPlacedTile(x, y).GetComponent<Tile>().East == Tile.Geography.City)
                if (!placedTiles.CityTileHasGrassOrStreamCenter(x, y))
                {
                    if (placedTiles.getPlacedTile(x + 1, y) != null)
                    {
                        if (!visited[x + 1, y]) RecursiveCityIsFinished(x + 1, y);
                    }
                    else
                    {
                        cityIsFinished = false;
                    }
                }

            if (placedTiles.getPlacedTile(x, y).GetComponent<Tile>().South == Tile.Geography.City)
                if (!placedTiles.CityTileHasGrassOrStreamCenter(x, y))
                {
                    if (placedTiles.getPlacedTile(x, y - 1) != null)
                    {
                        if (!visited[x, y - 1]) RecursiveCityIsFinished(x, y - 1);
                    }
                    else
                    {
                        cityIsFinished = false;
                    }
                }

            if (placedTiles.getPlacedTile(x, y).GetComponent<Tile>().West == Tile.Geography.City)
                if (!placedTiles.CityTileHasGrassOrStreamCenter(x, y))
                {
                    if (placedTiles.getPlacedTile(x - 1, y) != null)
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
        tile.GetComponent<Tile>().vIndex = VertexItterator;
        point.placeVertex(VertexItterator, placedTiles.GetNeighbors(tempX, tempY),
            placedTiles.getWeights(tempX, tempY), tileController.currentTile.GetComponent<Tile>().getCenter(),
            placedTiles.getCenters(tempX, tempY), placedTiles.getDirections(tempX, tempY));
        VertexItterator++;
        placedTiles.PlaceTile(x, z, tile); 
        calculatePoints(false, false);
        //shader.VisualizeBoard(gameState.Tiles.Played, gameState.Meeples.All);
        tileCounter++;
        if(tileCounter != 1)
        {
            Debug.Log("AI Placed tile on " + x + "," + z);
            Debug.Log(" Number of tiles placed: " + tileCounter + " At step " + Academy.Instance.StepCount + " of episode");
        }      
    }

    public void PlaceMeeple(GameObject meeple)
    {

    }

    //Metod för att plocka upp en ny tile
    public void PickupTile()
    {
        if (gameState.phase == Phase.NewTurn)
        {
            stack.Pop();
            if (!TileCanBePlaced(gameState.Tiles.Current))
            {            
                Destroy(tileController.currentTile);
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
            if (placedTiles.TilePlacementIsValid(tileController.currentTile, iTileAimX, iTileAimZ))
            {
                PlaceTile(tileController.currentTile, iTileAimX, iTileAimZ, false);
                gameState.phase = Phase.TileDown;
            }
        }
        else if (gameState.phase == Phase.MeepleDrawn)
        {
            if (gameState.Meeples.Current != null)
            {
                if (meepleController.meepleGeography == Tile.Geography.City ||
                    meepleController.meepleGeography == Tile.Geography.Cloister ||
                    meepleController.meepleGeography == Tile.Geography.Road)
                {
                    meepleController.PlaceMeeple(gameState.Meeples.Current.gameObject,
                        meepleController.iMeepleAimX, meepleController.iMeepleAimZ,
                        Direction, meepleController.meepleGeography, this);
                }

                else
                {
                    meepleController.FreeMeeple(gameState.Meeples.Current, this);
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
            if (stack.isEmpty())
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
            foreach (Meeple meeple in p.meeples)
            {
                var tileID = placedTiles.getPlacedTile(meeple.x, meeple.z).GetComponent<Tile>().id;
                var finalscore = 0;
                if (meeple.geography == Tile.Geography.City)
                {
                    //CITY DIRECTION
                    if (placedTiles.getPlacedTile(meeple.x, meeple.z).GetComponent<Tile>().getCenter() ==
                        Tile.Geography.Stream ||
                        placedTiles.getPlacedTile(meeple.x, meeple.z).GetComponent<Tile>().getCenter() ==
                        Tile.Geography.Grass ||
                        placedTiles.getPlacedTile(meeple.x, meeple.z).GetComponent<Tile>().getCenter() ==
                        Tile.Geography.Road ||
                        placedTiles.getPlacedTile(meeple.x, meeple.z).GetComponent<Tile>().getCenter() ==
                        Tile.Geography.Village)
                    {
                        if (CityIsFinishedDirection(meeple.x, meeple.z, meeple.direction))
                        {

                            finalscore = point
                                .startDfsDirection(
                                    placedTiles.getPlacedTile(meeple.x, meeple.z).GetComponent<Tile>()
                                        .vIndex, meeple.geography, meeple.direction, GameEnd);
                        }

                        if (GameEnd)
                        {
                            finalscore = point
                                .startDfsDirection(
                                    placedTiles.getPlacedTile(meeple.x, meeple.z).GetComponent<Tile>()
                                        .vIndex, meeple.geography, meeple.direction, GameEnd);
                        }
                    }
                    else
                    {
                        //CITY NO DIRECTION
                        if (CityIsFinished(meeple.x, meeple.z))
                            finalscore = point
                                .startDfs(
                                    placedTiles.getPlacedTile(meeple.x, meeple.z).GetComponent<Tile>()
                                        .vIndex, meeple.geography, GameEnd);
                        if (GameEnd)
                        {
                            finalscore = point
                                .startDfsDirection(
                                    placedTiles.getPlacedTile(meeple.x, meeple.z).GetComponent<Tile>()
                                        .vIndex, meeple.geography, meeple.direction, GameEnd);
                        }
                    }
                }
                else
                {
                    ///ROAD
                    if (placedTiles.getPlacedTile(meeple.x, meeple.z).GetComponent<Tile>().getCenter() ==
                        Tile.Geography.Village ||
                        placedTiles.getPlacedTile(meeple.x, meeple.z).GetComponent<Tile>().getCenter() ==
                        Tile.Geography.Grass)
                    {
                        finalscore = point.startDfsDirection(placedTiles
                            .getPlacedTile(meeple.x, meeple.z)
                            .GetComponent<Tile>().vIndex, meeple.geography, meeple.direction, GameEnd);
                        if (GameEnd)
                            finalscore--;
                    }
                    else
                    {
                        finalscore = point
                            .startDfs(
                                placedTiles.getPlacedTile(meeple.x, meeple.z).GetComponent<Tile>().vIndex,
                                meeple.geography, GameEnd);
                        if (GameEnd)
                            finalscore--;
                    }

                    //CLOISTER
                    if (placedTiles.getPlacedTile(meeple.x, meeple.z).GetComponent<Tile>().getCenter() ==
                        Tile.Geography.Cloister &&
                        meeple.direction == Point.Direction.CENTER)
                        finalscore = placedTiles.CheckSurroundedCloister(meeple.x, meeple.z, GameEnd);
                }

                if (finalscore > 0 && RealCheck)
                {
                    p.score += finalscore;
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

            if (pcRotate) tileController.currentTile.transform.Rotate(0.0f, 90.0f, 0.0f, Space.Self);
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
