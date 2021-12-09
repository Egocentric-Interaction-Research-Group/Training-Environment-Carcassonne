using Carcassonne;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameController handles all the game logic and the actual game loop
/// </summary>
public class GameController : MonoBehaviour
{
    public bool startGame, cityIsFinished;

    private bool[,] visited;

    public int iTileAimX, iTileAimZ, minX, minZ, maxX, maxZ;

    private int NewTileRotation, tempX, tempY, VertexItterator;

    public GameState state = new GameState();

    private CarcassonneVisualization shader;

    public GameObject AI, visualizationBoard;

    public PlacedTiles placedTiles;

    internal Player currentPlayer;

    public Stack stack;

    public Point point;

    public MeepleController meepleController;

    private int firstTurnCounter;


    /// <summary>
    /// MonoBehavior method that will create the necessary data before the game starts
    /// Some objects such as anything that is related to the ML Agent has to be set at this point so that it can be used
    /// at the first FixedUpdate call
    /// </summary>
    private void Start()
    {
        stack.tiles = state.tiles;
        placedTiles.tiles = state.tiles;
        meepleController.meeples = state.meeples;
        for (int i = 0; i < 1; i++) //Creates all the players
        {
            GameObject Agent = Instantiate(AI);
            AIWrapper aIWrapper = Agent.GetComponent<CarcassonneAgent>().wrapper;
            aIWrapper.state = state;
            aIWrapper.controller = this;
            aIWrapper.totalTiles = 72;
            Player player = Agent.GetComponent<Player>();
            Agent.GetComponent<CarcassonneAgent>().wrapper.player = player;
            player.id = i;
            player.meepleState = state.meeples;
            player.Setup();
            if (i == 0) //First created player will be player that starts in the first round
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

    /// <summary>
    /// MonoBehavior method where the game loop is run.   
    /// </summary>
    private void FixedUpdate()
    {
        if (startGame)
        {
            state.ResetStates(); // Before a new game is launched, all state data will be reset
            NewGame();
            startGame = false;
        }

        else if (state.phase == Phase.GameOver)
        {
            startGame = true;
        }
    }

    /// <summary>
    /// Setup for a new game session
    /// </summary>
    public void NewGame()
    {
        VertexItterator = 0;
        stack.PopulateTileArray();
        //Initialize AI player data dependent on tile array.       
        foreach (Player p in state.players.All)
        {
            p.Setup();
            p.GetComponent<CarcassonneAgent>().wrapper.totalTiles = state.tiles.Remaining.Count + 1;

        }
        BaseTileCreation();
        RotateTile();
        minX = 20;
        minZ = 20;
        maxX = 20;
        maxZ = 20;
        PlaceTile(state.tiles.Current, 20, 20); // The first tile is always automatically set in the middle of the board
        state.phase = Phase.NewTurn;
    }

    /// <summary>
    /// Create the base to be used as starting tile in a new game session
    /// </summary>
    private void BaseTileCreation()
    {
        state.tiles.Current = stack.GetBaseTile();
    }

    /// <summary>
    /// Starts a recursive search to check whether a city has been finished starting from x,z in the tile grid with a specified direction
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool CityIsFinishedDirection(int x, int y, Point.Direction direction)
    {
        meepleController.MeeplesInCity = new List<Meeple>();
        meepleController.MeeplesInCity.Add(meepleController.FindMeeple(x, y, Tile.Geography.City, direction));
        cityIsFinished = true;
        visited = new bool[40, 40];
        RecursiveCityIsFinishedDirection(x, y, direction);
        return cityIsFinished;
    }

    /// <summary>
    /// Checks if a city has been finished on x,z in the tile grid
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool CityIsFinished(int x, int y)
    {
        meepleController.MeeplesInCity = new List<Meeple>();
        meepleController.MeeplesInCity.Add(meepleController.FindMeeple(x, y, Tile.Geography.City));
        cityIsFinished = true;
        visited = new bool[40, 40];
        RecursiveCityIsFinished(x, y);
        return cityIsFinished;
    }

    /// <summary>
    /// The recursive search to check if tiles are connected through a city geography in the specified direction
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="direction"></param>
    public void RecursiveCityIsFinishedDirection(int x, int y, Point.Direction direction)
    {
        visited[x, y] = true;
        if (direction == Point.Direction.NORTH)
            if (placedTiles.getPlacedTile(x, y).North == Tile.Geography.City)
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
            if (placedTiles.getPlacedTile(x, y).East == Tile.Geography.City)
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
            if (placedTiles.getPlacedTile(x, y).South == Tile.Geography.City)
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
            if (placedTiles.getPlacedTile(x, y).West == Tile.Geography.City)
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


    /// <summary>
    /// The recursive search to check if tiles are connected through a city geography
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="direction"></param>
    public void RecursiveCityIsFinished(int x, int y)
    {
        visited[x, y] = true;


        if (placedTiles.getPlacedTile(x, y) != null)
        {
            if (placedTiles.getPlacedTile(x, y).North == Tile.Geography.City)
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

            if (placedTiles.getPlacedTile(x, y).East == Tile.Geography.City)
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

            if (placedTiles.getPlacedTile(x, y).South == Tile.Geography.City)
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

            if (placedTiles.getPlacedTile(x, y).West == Tile.Geography.City)
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

    /// <summary>
    /// Checks if there is any valid tileplacement for a tile in the current tile grid
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool TileCanBePlaced(Tile tile)
    {
        for (var i = 0; i < placedTiles.GetLength(0); i++)
            for (var j = 0; j < placedTiles.GetLength(1); j++)
                if (placedTiles.HasNeighbor(i, j) && placedTiles.getPlacedTile(i, j) == null)
                    for (var k = 0; k < 4; k++)
                    {
                        if (placedTiles.MatchGeographyOrNull(i - 1, j, Point.Direction.EAST, tile.West))
                            if (placedTiles.MatchGeographyOrNull(i + 1, j, Point.Direction.WEST, tile.East))
                                if (placedTiles.MatchGeographyOrNull(i, j - 1, Point.Direction.NORTH, tile.South))
                                    if (placedTiles.MatchGeographyOrNull(i, j + 1, Point.Direction.SOUTH,
                                        tile.North))
                                    {
                                        ResetTileRotation();
                                        return true;
                                    }

                        RotateTile();
                    }

        ResetTileRotation();
        return false;
    }

    /// <summary>
    /// Place a tile on x,z coordinates in the tile grid.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="firstTile"></param>

    public void PlaceTile(Tile tile, int x, int z)
    {
        tempX = x;
        tempY = z;
        UpdateAIBoundary(x, z);
        tile.vIndex = VertexItterator;
        point.placeVertex(VertexItterator, placedTiles.GetNeighbors(tempX, tempY),
            placedTiles.getWeights(tempX, tempY), state.tiles.Current.getCenter(),
            placedTiles.getCenters(tempX, tempY), placedTiles.getDirections(tempX, tempY));
        VertexItterator++;
        placedTiles.PlaceTile(x, z, tile);
        calculatePoints(false, false);
    }

    /// <summary>
    /// Update current active tile with new tile from the tile stack. If the tile cannot be placed, pop a new one.
    /// </summary>
    public void PickupTile()
    {
        if (state.phase == Phase.NewTurn)
        {
            stack.Pop();
            if (!TileCanBePlaced(state.tiles.Current))
            {
                state.tiles.Current = null;
                PickupTile();
            }
            else
            {
                ResetTileRotation();
                state.phase = Phase.TileDrawn;
            }
        }
    }

    /// <summary>
    /// Confirms that a tile should be placed based on the current iTileAimX and iTileAimZ coordinates that are set
    /// in the Game Controller
    /// </summary>

    public void ConfirmTilePlacement()
    {
        if (state.phase == Phase.TileDrawn)
        {
            if (placedTiles.TilePlacementIsValid(state.tiles.Current, iTileAimX, iTileAimZ))
            {
                PlaceTile(state.tiles.Current, iTileAimX, iTileAimZ);
                state.phase = Phase.TileDown;
                shader.VisualizeBoard(state.tiles.Played, state.meeples.All);
            }
        }
        shader.VisualizeBoard(state.tiles.Played, state.meeples.All);
    }

    /// <summary>
    /// Confirms that a meeple should be placed based on the specified meeple direction on the current tile
    /// </summary>
    public void ConfirmMeeplePlacement(Point.Direction meepleDirection)
    {
        if (state.phase == Phase.MeepleDrawn)
        {
            if (state.meeples.Current != null)
            {
                if (meepleController.meepleGeography == Tile.Geography.City ||
                   meepleController.meepleGeography == Tile.Geography.Cloister ||
                   meepleController.meepleGeography == Tile.Geography.Road)
                {
                    meepleController.PlaceMeeple(state.meeples.Current,
                        meepleController.iMeepleAimX, meepleController.iMeepleAimZ,
                        meepleDirection, meepleController.meepleGeography);
                }

                else
                {
                    meepleController.FreeMeeple(state.meeples.Current);
                }
            }
        }
    }

    /// <summary>
    /// End the current players turn. Calculate any points acquired by placement of tile and/or meeple and move
    /// from phase TileDown or MeepleDown to either NewTurn or if there are no more tiles that can be drawn, end the game through
    /// GameOver()
    /// </summary>
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

    /// <summary>
    /// Calculate points for players based on board state. If the game is over, points will still be awarded.
    /// </summary>
    /// <param name="RealCheck"></param>
    /// <param name="GameEnd"></param>
    public void calculatePoints(bool RealCheck, bool GameEnd)
    {
        foreach (var p in state.players.All)
            foreach (Meeple meeple in p.meeples)
            {
                if (!meeple.free)
                {
                    var finalscore = 0;
                    if (meeple.geography == Tile.Geography.City)
                    {
                        //CITY DIRECTION
                        if (placedTiles.getPlacedTile(meeple.x, meeple.z).getCenter() ==
                            Tile.Geography.Stream ||
                            placedTiles.getPlacedTile(meeple.x, meeple.z).getCenter() ==
                            Tile.Geography.Grass ||
                            placedTiles.getPlacedTile(meeple.x, meeple.z).getCenter() ==
                            Tile.Geography.Road ||
                            placedTiles.getPlacedTile(meeple.x, meeple.z).getCenter() ==
                            Tile.Geography.Village)
                        {
                            if (CityIsFinishedDirection(meeple.x, meeple.z, meeple.direction))
                            {

                                finalscore = point
                                    .startDfsDirection(
                                        placedTiles.getPlacedTile(meeple.x, meeple.z)
                                            .vIndex, meeple.geography, meeple.direction, GameEnd);
                            }

                            if (GameEnd)
                            {
                                finalscore = point
                                    .startDfsDirection(
                                        placedTiles.getPlacedTile(meeple.x, meeple.z)
                                            .vIndex, meeple.geography, meeple.direction, GameEnd);
                            }
                        }
                        else
                        {
                            //CITY NO DIRECTION
                            if (CityIsFinished(meeple.x, meeple.z))
                                finalscore = point
                                    .startDfs(
                                        placedTiles.getPlacedTile(meeple.x, meeple.z)
                                            .vIndex, meeple.geography, GameEnd);
                            if (GameEnd)
                            {
                                finalscore = point
                                    .startDfsDirection(
                                        placedTiles.getPlacedTile(meeple.x, meeple.z)
                                            .vIndex, meeple.geography, meeple.direction, GameEnd);
                            }
                        }
                    }
                    else
                    {
                        ///ROAD
                        if (placedTiles.getPlacedTile(meeple.x, meeple.z).getCenter() ==
                            Tile.Geography.Village ||
                            placedTiles.getPlacedTile(meeple.x, meeple.z).getCenter() ==
                            Tile.Geography.Grass)
                        {
                            finalscore = point.startDfsDirection(placedTiles
                                .getPlacedTile(meeple.x, meeple.z)
                                .vIndex, meeple.geography, meeple.direction, GameEnd);
                            if (GameEnd)
                                finalscore--;
                        }
                        else
                        {
                            finalscore = point
                                .startDfs(
                                    placedTiles.getPlacedTile(meeple.x, meeple.z).vIndex,
                                    meeple.geography, GameEnd);
                            if (GameEnd)
                                finalscore--;
                        }

                        //CLOISTER
                        if (placedTiles.getPlacedTile(meeple.x, meeple.z).getCenter() ==
                            Tile.Geography.Cloister &&
                            meeple.direction == Point.Direction.CENTER)
                            finalscore = placedTiles.CheckSurroundedCloister(meeple.x, meeple.z, GameEnd);
                    }

                    //TODO: Meeples should be free even if they did not reward any point as long as a geography is finished
                    if (finalscore > 0 && RealCheck)
                    {
                        meeple.free = true;
                        p.score += finalscore;
                    }
                }
            }
    }

    /// <summary>
    /// Rotate current tile 90 degrees
    /// </summary>
    public void RotateTile()
    {
        if (state.phase == Phase.TileDrawn)
        {
            NewTileRotation++;
            if (NewTileRotation > 3) NewTileRotation = 0;
            state.tiles.Current.Rotate();
        }
    }

    /// <summary>
    /// Change current tile rotation back to 0
    /// </summary>

    public void ResetTileRotation()
    {
        NewTileRotation = 0;
        state.tiles.Current.rotation = 0;
    }

    /// <summary>
    /// The game is over and final points are calculated. Phase is moved to GameOver
    /// </summary>
    private void GameOver()
    {
        calculatePoints(true, true);
        state.phase = Phase.GameOver;
    }

    /// <summary>
    /// Update the boundaries that the AI can place tiles within
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void UpdateAIBoundary(int x, int z)
    {
        if (x < minX)
        {
            minX = x;
        }
        if (z < minZ)
        {
            minZ = z;
        }
        if (x > maxX)
        {
            maxX = x;
        }
        if (z > maxZ)
        {
            maxZ = z;
        }
    }
}
