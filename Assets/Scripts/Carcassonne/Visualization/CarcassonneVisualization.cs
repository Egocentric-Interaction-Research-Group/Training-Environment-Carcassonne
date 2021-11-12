//------------------------------------------------------------------------------------------------//
// Author:  Kasper Skott
// Created: 2021-10-22
//------------------------------------------------------------------------------------------------//

using System;
using UnityEngine;
using System.Collections.Generic;

using Geography = Carcassonne.Tile.Geography;
using Direction = Carcassonne.Point.Direction;

namespace Carcassonne
{
    /// <summary>
    /// This script is to be used in combination with the shader "CarcassoneVisualization".
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class CarcassonneVisualization : MonoBehaviour
    {
        private const int MAX_BOARD_DIMENSION = 10; // The maximum nbr of tiles in each axis. This is limited by the shader.

        private Material m_mat;

        void Awake()
        {
            m_mat = GetComponent<Renderer>().material;

            int boardSize = MAX_BOARD_DIMENSION * MAX_BOARD_DIMENSION * 3 * 3;
            float[] tilesInit = new float[boardSize];
            for (int i = 0; i < boardSize; i++)
                tilesInit[i] = -1.0f;

            m_mat.SetFloatArray("_TileGeography", tilesInit);
            m_mat.SetFloatArray("_MeeplePlacement", tilesInit);

            // For testing purposes.
            UpdateWithTestData();
        }

        /// <summary>
        /// Use this method to send tile and meeple data to the shader.
        /// Tiles and Meeples will be culled accordingly.
        /// 
        /// If you're already sending a subsection of all tile data, consider
        /// using UpdateMaterial instead. However, you will have to know the
        /// current offset of your tiles into the original tile array of all tiles.
        /// </summary>
        /// <param name="allTiles">The entire board of tiles. May very well include null elements.</param>
        /// <param name="allMeeples">All meeples in the game, even if they're not placed.</param>
        public void VisualizeBoard(Tile[,] allTiles, IReadOnlyList<Meeple> allMeeples)
        {
            // Get boundaries of the played tiles so as to only bother with existing tiles.
            (Vector2Int size, Vector2Int offset) = GetPlayedTileBounds(allTiles);

            // Slice allTiles using the boundaries, resulting in an array, trimmed of all
            // excessive null Tile instances.
            Tile[,] playedTiles = Get2DSubSection(allTiles, size, offset);

            // Update the material instance to display the only the bounds of played tiles,
            // and provide all meeples (free or not). Meeples are automatically placed according
            // to the given offset.
            UpdateMaterial(playedTiles, offset, allMeeples);
        }

        /// <summary>
        /// Gets the boundaries of existing tiles in the given array.
        /// The offset is the left-most and upper-most existing tiles.
        /// The size how many tiles the boundary spans in each direction
        /// from that offset.
        /// </summary>
        /// <param name="allTiles">A 2d array of null and/or valid Tile instances.</param>
        /// <returns>size (width, height), and offset (x, y).</returns>
        public static (Vector2Int, Vector2Int) GetPlayedTileBounds(Tile[,] allTiles)
        {
            Vector2Int dims = new Vector2Int(allTiles.GetLength(0), allTiles.GetLength(1));

            int minRow = int.MaxValue;
            int minCol = int.MaxValue;
            int maxRow = int.MinValue;
            int maxCol = int.MinValue;
            for (int row = 0; row < dims.y; row++)
            {
                for (int col = 0; col < dims.x; col++)
                {
                    if (allTiles[col, row] == null) // Valid tile
                        continue;

                    if (minRow == int.MaxValue) // Has not yet found upper-most
                        minRow = row;

                    if (col < minCol)
                        minCol = col;

                    if (row > maxRow)
                        maxRow = row + 1;

                    if (col > maxCol)
                        maxCol = col + 1;
                }
            }

            Vector2Int size = new Vector2Int(maxCol - minCol, maxRow - minRow);
            Vector2Int offset = new Vector2Int(minCol, minRow);
            return (size, offset);
        }

        /// <summary>
        /// Gets a subsection of the specified 2-dimensional array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr2d">The array to slice from.</param>
        /// <param name="size">The size of the subsection.</param>
        /// <param name="offset">Where in arr2d to begin slicing.</param>
        /// <returns>A new 2D array of the specified size.</returns>
        public static T[,] Get2DSubSection<T>(T[,] arr2d, Vector2Int size, Vector2Int offset)
        {
            T[,] section = new T[size.x, size.y];
            for (int y = 0; y < size.y; y++)
                for (int x = 0; x < size.x; x++)
                    section[x, y] = arr2d[x + offset.x, y + offset.y];

            return section;
        }

        /// <summary>
        /// Updates the material to display the given section of tiles.
        /// </summary>
        /// <param name="updateTiles">The tiles to be shown.</param>
        /// <param name="updateOffset">The tile-space offset of the currently shown tiles 
        ///     into the entire tile array.</param>
        /// <param name="allMeeples">A list of all meeples.</param>
        public void UpdateMaterial(
            Tile[,] updateTiles,
            Vector2Int updateOffset,
            IReadOnlyList<Meeple> allMeeples)
        {
            Vector2Int updateDim = new Vector2Int(
                updateTiles.GetLength(0), updateTiles.GetLength(1));

            // Clamp the dimensions of the tile data to be sent to the shader.
            if (updateDim.x > MAX_BOARD_DIMENSION ||
                updateDim.y > MAX_BOARD_DIMENSION)
            {
                updateTiles = Get2DSubSection(updateTiles, 
                    new Vector2Int(MAX_BOARD_DIMENSION, MAX_BOARD_DIMENSION), 
                    new Vector2Int(0, 0));

                updateDim.x = updateTiles.GetLength(0);
                updateDim.y = updateTiles.GetLength(1);
            }

            // Make sure to always display 1:1 column-row ratio.
            Vector2Int displayDim = updateDim;
            if (displayDim.x < displayDim.y)
                displayDim.x = displayDim.y;
            else
                displayDim.y = displayDim.x;

            m_mat.SetInt("_DisplayColumns", displayDim.x);
            m_mat.SetInt("_DisplayRows", displayDim.y);

            // Create a dictionary with key (col, row) and value array (playerId+1 for each direction).
            // This maps a certain location with the player that occupies it.
            Dictionary<(int, int), int[]> playerMeeples = new Dictionary<(int, int), int[]>();
            foreach (Meeple m in allMeeples)
            {
                if (m.free)
                    continue;

                (int, int) loc = (m.x - updateOffset.x, m.z - updateOffset.y);

                // Cull meeples that are outside the dimensions shown.
                if (loc.Item1 < 0 || loc.Item1 >= displayDim.x ||
                    loc.Item2 < 0 || loc.Item2 >= displayDim.y)
                    continue;

                int[] playersAtDirections;
                if (!playerMeeples.TryGetValue(loc, out playersAtDirections))
                {
                    playersAtDirections = new int[5];
                    for (int i = 0; i < 5; i++)
                        playersAtDirections[i] = 0;
                }

                if (m.direction == Direction.CENTER)      playersAtDirections[0] = m.playerId + 1;
                else if (m.direction == Direction.EAST)   playersAtDirections[1] = m.playerId + 1;
                else if (m.direction == Direction.NORTH)  playersAtDirections[2] = m.playerId + 1;
                else if (m.direction == Direction.WEST)   playersAtDirections[3] = m.playerId + 1;
                else if (m.direction == Direction.SOUTH)  playersAtDirections[4] = m.playerId + 1;

                playerMeeples[loc] = playersAtDirections;
            }

            // Prepare a float array of sub-tile data to send to the shader. The data represents
            //   the geography and player that occupies that sub-tile with a meeple. All using
            //   a single float value.
            // Note: Needs to send a *float* array to the shader because shaders seem to use floats
            //   internally anyway, and there is no interface for sending int arrays.
            float[] tiles = new float[displayDim.x * displayDim.y];
            float[] meeples = new float[displayDim.x * displayDim.y];
            for (int row = 0; row < displayDim.y; row++)
            {
                for (int col = 0; col < displayDim.x; col++)
                {
                    int idx = col + row * displayDim.x;

                    // Discard if the tile isn't in updateTile.
                    if (col >= updateDim.x || row >= updateDim.y)
                    {
                        tiles[idx] = -1.0f;
                        meeples[idx] = 0.0f;
                        continue;
                    }

                    // Only set values for existing Tiles.
                    if (updateTiles.GetValue(col, row) is Tile t)
                    {
                        // Combine all 5 tile geographies into a single float.
                        float tileGeography = (float)t.Center;
                        tileGeography += (float)t.East  * 10;
                        tileGeography += (float)t.North * 100;
                        tileGeography += (float)t.West  * 1000;
                        tileGeography += (float)t.South * 10000;

                        int[] playersIds;
                        if (!playerMeeples.TryGetValue((col ,row), out playersIds))
                        {
                            playersIds = new int[] { 0, 0, 0, 0, 0};
                        }

                        // Combine all meeple placement on this tile into a single float.
                        float tileMeeple = playersIds[0];
                        tileMeeple += playersIds[1] * 10;
                        tileMeeple += playersIds[2] * 100;
                        tileMeeple += playersIds[3] * 1000;
                        tileMeeple += playersIds[4] * 10000;

                        tiles[idx]   = tileGeography;
                        meeples[idx] = tileMeeple;
                    }
                    else // Invalid or non-existent Tile.
                    {
                        tiles[idx] = -1.0f;
                        meeples[idx] = 0.0f;
                    }
                }
            }

            if (tiles.Length > 0)
            {
                m_mat.SetFloatArray("_TileGeography", tiles);
                m_mat.SetFloatArray("_MeeplePlacement", meeples);
            }
        }

        //---- FOR TESTING -----------------------------------------------------------------------//

        /// <summary>
        /// Updates the material with test data.
        /// This is just for testing things quickly without a real data set.
        /// </summary>
        private void UpdateWithTestData()
        {
            // Fills a new Tile with the given Geography.
            Func<Geography, Tile>
            CreateTile = (geo) =>
            {
                Tile tile = new Tile();
                tile.East = geo;
                tile.North = geo;
                tile.West = geo;
                tile.South = geo;
                tile.Center = geo;
                return tile;
            };

            int showColumns = 5;
            int showRows = 5;

            // Create and fill every tile with grass by default.
            Tile[,] tiles = new Tile[showColumns, showRows];
            for (int row = 0; row < showRows; row++)
                for (int col = 0; col < showColumns; col++)
                    tiles[col, row] = CreateTile(Geography.Grass);

            tiles[1, 0] = null;
            tiles[2, 0] = null;
            tiles[3, 0] = null;
            tiles[4, 0] = null;
            tiles[4, 1] = null;
            tiles[4, 3] = null;
            tiles[4, 4] = null;
            tiles[0, 4] = null;
            tiles[3, 4] = null;
            tiles[0, 3] = null;

            tiles[0, 0].Center = Geography.Road;
            tiles[0, 0].East = Geography.Road;
            tiles[0, 0].South = Geography.Road;

            tiles[0, 1].North = Geography.Road;
            tiles[0, 1].Center = Geography.Village;
            tiles[0, 1].South = Geography.Road;

            tiles[0, 2].West = Geography.City;
            tiles[0, 2].North = Geography.Road;
            tiles[0, 2].Center = Geography.Road;
            tiles[0, 2].East = Geography.Road;

            tiles[1, 2].West = Geography.Road;
            tiles[1, 2].Center = Geography.Road;
            tiles[1, 2].East = Geography.Road;

            tiles[2, 1].North = Geography.Road;
            tiles[2, 1].Center = Geography.Road;
            tiles[2, 1].South = Geography.Road;

            tiles[2, 2].West = Geography.Road;
            tiles[2, 2].North = Geography.Road;
            tiles[2, 2].South = Geography.Road;
            tiles[2, 2].East = Geography.Road;
            tiles[2, 2].Center = Geography.Village;

            tiles[2, 3].North = Geography.Road;
            tiles[2, 3].Center = Geography.Road;
            tiles[2, 3].South = Geography.Road;

            tiles[2, 4].North = Geography.Road;
            tiles[2, 4].Center = Geography.Village;

            tiles[3, 1].Center = Geography.Cloister;

            tiles[3, 2].West = Geography.Road;
            tiles[3, 2].Center = Geography.City;
            tiles[3, 2].East = Geography.City;

            tiles[4, 2].West = Geography.City;
            tiles[4, 2].Center = Geography.City;
            tiles[4, 2].South = Geography.City;
            tiles[4, 2].East = Geography.City;

            List<Meeple> meeples = new List<Meeple>(); // Should actually use something like MeepleState.All.
            meeples.Add(new Meeple());
            meeples[0].assignAttributes(3, 1, Direction.CENTER, Geography.Grass);
            meeples[0].free = false;
            meeples[0].playerId = 0;

            meeples.Add(new Meeple());
            meeples[1].assignAttributes(3, 2, Direction.CENTER, Geography.City);
            meeples[1].free = false;
            meeples[1].playerId = 1;

            UpdateMaterial(tiles, new Vector2Int(0, 0), meeples);
        }
    }
}