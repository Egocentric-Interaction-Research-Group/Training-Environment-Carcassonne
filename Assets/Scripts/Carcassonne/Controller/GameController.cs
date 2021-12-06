using Carcassonne;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Carcassonne.AI;

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


    private void Start()
    {
        stack.tiles = state.tiles;
        placedTiles.tiles = state.tiles;
        meepleController.meeples = state.meeples;
        for (int i = 0; i < 1; i++)
        {
            GameObject Agent = Instantiate(AI);
            AIWrapper aIWrapper = Agent.GetComponent<CarcassonneAgent>().wrapper;
            aIWrapper.state = state;
            aIWrapper.controller = this;
            aIWrapper.totalTiles = 74;
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
        minX = 15;
        minZ  = 15;
        maxX = 15;
        maxZ = 15;
        PlaceTile(state.tiles.Current, 15, 15, true);
        state.phase = Phase.NewTurn;
    }

    private void BaseTileCreation()
    {
        NewTile baseTile = new NewTile();
        baseTile.AssignAttributes(stack.firstTile.GetComponent<Tile>().id);
        state.tiles.Current = baseTile;
    }

    public bool CityIsFinishedDirection(int x, int y, Point.Direction direction)
    {
        meepleController.MeeplesInCity = new List<Meeple>();
        meepleController.MeeplesInCity.Add(meepleController.FindMeeple(x, y, NewTile.Geography.City, direction));
        cityIsFinished = true;
        visited = new bool[30, 30];
        RecursiveCityIsFinishedDirection(x, y, direction);     
        return cityIsFinished;
    }

    public bool CityIsFinished(int x, int y)
    {
        meepleController.MeeplesInCity = new List<Meeple>();
        meepleController.MeeplesInCity.Add(meepleController.FindMeeple(x, y, NewTile.Geography.City));
        cityIsFinished = true;
        visited = new bool[30,30];
        RecursiveCityIsFinished(x, y);   
        return cityIsFinished;
    }

    public void RecursiveCityIsFinishedDirection(int x, int y, Point.Direction direction)
    {
        visited[x, y] = true;
        if (direction == Point.Direction.NORTH)
            if (placedTiles.getPlacedTile(x, y).North == NewTile.Geography.City)
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
            if (placedTiles.getPlacedTile(x, y).East == NewTile.Geography.City)
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
            if (placedTiles.getPlacedTile(x, y).South == NewTile.Geography.City)
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
            if (placedTiles.getPlacedTile(x, y).West == NewTile.Geography.City)
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

    public bool TileCanBePlaced(NewTile tile)
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

    public void RecursiveCityIsFinished(int x, int y)
    {
        visited[x, y] = true;


        if (placedTiles.getPlacedTile(x, y) != null)
        {
            if (placedTiles.getPlacedTile(x, y).North == NewTile.Geography.City)
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

            if (placedTiles.getPlacedTile(x, y).East == NewTile.Geography.City)
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

            if (placedTiles.getPlacedTile(x, y).South == NewTile.Geography.City)
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

            if (placedTiles.getPlacedTile(x, y).West == NewTile.Geography.City)
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

    public void PlaceTile(NewTile tile, int x, int z, bool firstTile)
    {
        tempX = x;
        tempY = z;
        UpdateAIBoundary(x,z);
        tile.vIndex = VertexItterator;
        point.placeVertex(VertexItterator, placedTiles.GetNeighbors(tempX, tempY),
            placedTiles.getWeights(tempX, tempY), state.tiles.Current.getCenter(),
            placedTiles.getCenters(tempX, tempY), placedTiles.getDirections(tempX, tempY));
        VertexItterator++;
        placedTiles.PlaceTile(x, z, tile); 
        calculatePoints(false, false);        
    }

    //Metod för att plocka upp en ny tile
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

    public void ConfirmPlacement()
    {
        if (state.phase == Phase.TileDrawn)
        {
            if (placedTiles.TilePlacementIsValid(state.tiles.Current, iTileAimX, iTileAimZ))
            {
                PlaceTile(state.tiles.Current, iTileAimX, iTileAimZ, false);
                state.phase = Phase.TileDown;
                shader.VisualizeBoard(state.tiles.Played, state.meeples.All);
            }
        }
        shader.VisualizeBoard(state.tiles.Played, state.meeples.All);
    }

    public void ConfirmMeeplePlacement(Point.Direction meepleDirection)
    {
        if(state.phase == Phase.MeepleDrawn)
        {
            if(state.meeples.Current != null)
            {
                if (meepleController.meepleGeography == NewTile.Geography.City ||
                   meepleController.meepleGeography == NewTile.Geography.Cloister ||
                   meepleController.meepleGeography == NewTile.Geography.Road)
                {
                    meepleController.PlaceMeeple(state.meeples.Current,
                        meepleController.iMeepleAimX, meepleController.iMeepleAimZ,
                        meepleDirection, meepleController.meepleGeography);

                    foreach (Meeple m in state.meeples.All)
                    {
                        if (!m.free)
                        {
                        }
                    }
                }

                else
                {
                    meepleController.FreeMeeple(state.meeples.Current);
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
                if(!meeple.free) {
                    var finalscore = 0;
                    if (meeple.geography == NewTile.Geography.City)
                    {
                        //CITY DIRECTION
                        if (placedTiles.getPlacedTile(meeple.x, meeple.z).getCenter() ==
                            NewTile.Geography.Stream ||
                            placedTiles.getPlacedTile(meeple.x, meeple.z).getCenter() ==
                            NewTile.Geography.Grass ||
                            placedTiles.getPlacedTile(meeple.x, meeple.z).getCenter() ==
                            NewTile.Geography.Road ||
                            placedTiles.getPlacedTile(meeple.x, meeple.z).getCenter() ==
                            NewTile.Geography.Village)
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
                            NewTile.Geography.Village ||
                            placedTiles.getPlacedTile(meeple.x, meeple.z).getCenter() ==
                            NewTile.Geography.Grass)
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
                            NewTile.Geography.Cloister &&
                            meeple.direction == Point.Direction.CENTER)
                            finalscore = placedTiles.CheckSurroundedCloister(meeple.x, meeple.z, GameEnd);
                    }

                    if (finalscore > 0 && RealCheck)
                    {
                        p.score += finalscore;
                    }
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

    public void UpdateAIBoundary(int x, int z)
    {
        if(x < minX)
        {
            minX = x;
        }
        if(z < minZ)
        {
            minZ = z;
        }
        if(x > maxX)
        {
            maxX = x;
        }
        if(z > maxZ)
        {
            maxZ = z;
        }
    }
}
