using Carcassonne;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Assets.Scripts.Carcassonne.AI;

public class GameController : MonoBehaviour
{
    public bool startGame, pcRotate, isManipulating, cityIsFinished;

    private bool[,] visited;

    public int iTileAimX, iTileAimZ;

    private int NewTileRotation, tempX, tempY, VertexItterator, tileCounter;

    public GameState state = new GameState();

    private CarcassonneVisualization shader;

    private Point.Direction direction;

    public GameObject ai, visualizationBoard;

    public PlacedTiles placedTiles;

    internal Player currentPlayer;

    public Stack stack;

    public Point point;

    [SerializeField]
    public MeepleController meepleController;

    [SerializeField]
    public TileController tileController;

    private int firstTurnCounter;

    private void Start()
    {
        stack.tiles = state.tiles;
        placedTiles.tiles = state.tiles;
        tileController.tiles = state.tiles;
        meepleController.meeples = state.meeples;
        for (int i = 0; i < 1; i++)
        {
            GameObject Agent = Instantiate(ai);
            AIWrapper aIWrapper = Agent.GetComponent<CarcassonneAgent>().wrapper;
            aIWrapper.state = state;
            aIWrapper.controller = this;
            aIWrapper.totalTiles = 85;
            Player player = Agent.GetComponent<Player>();
            Agent.GetComponent<CarcassonneAgent>().wrapper.player = player;
            player.id = i;
            player.meepleState = state.meeples;
            player.Setup();           
            if (i == 0)
            {
                currentPlayer = player;
            }
            state.players.All.Add(player);
        }

        // Initialize the shader visualization in order to set the max array
        // size for upcoming shader data.
        shader = visualizationBoard.GetComponent<CarcassonneVisualization>();
        shader.Init();
    }

    private void FixedUpdate()
    {
        if (startGame)
        {
            state.ResetStates();
            NewGame();
            startGame = false;
        }

        else if (state.phase == Phase.GameOver)
        {
            startGame = true;
        }
    }

    public void NewGame()
    {
        tileCounter = 0;
        VertexItterator = 0;
        stack.PopulateTileArray();
        BaseTileCreation();
        pcRotate = true;
        RotateTile();
        PlaceTile(tileController.currentTile, 85, 85, true);
        //Initialize AI player data dependent on tile array.
        foreach (Player p in state.players.All)
        {
            p.GetComponent<CarcassonneAgent>().wrapper.totalTiles = state.tiles.Remaining.Count + 1;

        }
        state.phase = Phase.NewTurn;
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
        shader.VisualizeBoard(state.tiles.Played, state.meeples.All);
        tileCounter++;
        if(tileCounter != 1)
        {
            Debug.Log("AI Placed tile with id " + tile.GetComponent<Tile>().id + "on " + x + "," + z);
            Debug.Log(" Number of tiles placed: " + tileCounter + " At academy step " + Academy.Instance.StepCount + " of episode " + Academy.Instance.EpisodeCount);
        }      
    }

    public void PlaceMeeple(GameObject meeple)
    {

    }

    //Metod för att plocka upp en ny tile
    public void PickupTile()
    {
        if (state.phase == Phase.NewTurn)
        {
            stack.Pop();
            if (!TileCanBePlaced(state.tiles.Current))
            {
                tileController.currentTile = null;
                PickupTile();
            }
            else
            {
                ResetTileRotation();
                state.phase = Phase.TileDrawn;
            }
        }
    }

    public void ConfirmPlacement()
    {
        if (state.phase == Phase.TileDrawn)
        {
            if (placedTiles.TilePlacementIsValid(tileController.currentTile, iTileAimX, iTileAimZ))
            {
                PlaceTile(tileController.currentTile, iTileAimX, iTileAimZ, false);
                state.phase = Phase.TileDown;
            }
        }
        else if (state.phase == Phase.MeepleDrawn)
        {
            if (state.meeples.Current != null)
            {
                if (meepleController.meepleGeography == Tile.Geography.City ||
                    meepleController.meepleGeography == Tile.Geography.Cloister ||
                    meepleController.meepleGeography == Tile.Geography.Road)
                {
                    meepleController.PlaceMeeple(state.meeples.Current.gameObject,
                        meepleController.iMeepleAimX, meepleController.iMeepleAimZ,
                        direction, meepleController.meepleGeography, this);
                }

                else
                {
                    meepleController.FreeMeeple(state.meeples.Current, this);
                }
            }
        }
    }

    public void EndTurn()
    {
        if (state.phase == Phase.TileDown || state.phase == Phase.MeepleDown)
        {
            calculatePoints(true, false);
            NewTileRotation = 0;
            if (stack.isEmpty())
            {
                GameOver();
            }
            else
            {
                if (state.players.All.Count > 1)
                {
                    if (currentPlayer == state.players.All[0])
                        currentPlayer = state.players.All[1];
                    else
                        currentPlayer = state.players.All[0];
                }

                if (firstTurnCounter != 0) firstTurnCounter -= 1;
                state.phase = Phase.NewTurn;
            }
        }
    }

    public void calculatePoints(bool RealCheck, bool GameEnd)
    {
        foreach (var p in state.players.All)
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
        if (state.phase == Phase.TileDrawn)
        {
            NewTileRotation++;
            if (NewTileRotation > 3) NewTileRotation = 0;
            state.tiles.Current.Rotate();

            if (pcRotate) tileController.currentTile.transform.Rotate(0.0f, 90.0f, 0.0f, Space.Self);
        }
    }

    public void ResetTileRotation()
    {
        NewTileRotation = 0;
        state.tiles.Current.rotation = 0;
    }

    private void GameOver()
    {
        calculatePoints(true, true);
        state.phase = Phase.GameOver;
    }

    public void ChangeStateToNewTurn()
    {
        state.phase = Phase.NewTurn;
    }

}
