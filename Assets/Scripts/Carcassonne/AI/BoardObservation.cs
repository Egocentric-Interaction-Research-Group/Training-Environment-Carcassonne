//------------------------------------------------------------------------------------------------//
// Author:   Kasper Skott
// Created:  2021-12-06
// Modified: 2021-12-09
//------------------------------------------------------------------------------------------------//

using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;

/// <summary>
/// Represents the different approaches when it comes to observing 
/// the Carcassonne game board.
/// </summary>
public enum ObservationApproach
{
    [InspectorName("Tile IDs")]
    [Tooltip("Observation size: 3218\nFor each tile, observe the tile ID and rotation as one " +
                "observation, and meeple data as another observation.")]
    TileIds,

    [InspectorName("Packed IDs")]
    [Tooltip("Observation size: 1618\nFor each tile, observe the tile ID, rotation, and " +
                "meeple data as one packed observation.")]
    PackedIDs,

    [Tooltip("Observation size: 3218\nFor each tile, pack all tile geographies explicitly, " +
                "into one observation (instead of using tile IDs), and then meeple data " +
                "as another observation.")]
    Packed
}

/// <summary>
/// Encapsulates different methods/approaches of having an AI 
/// observe a Carcassonne game board.
/// </summary>
public static class BoardObservation
{
    /// <summary>
    /// This builds a dictionary of tile coordinates as keys, and 
    /// meeple data (packed into a single int) as values.
    /// 
    /// Note that this is a shameless workaround for not being able to access the
    /// meeple from the tile it occupies. If you could get the player id of the
    /// meeple placed, via the tile, then this method wouldn't needed. 
    /// Please make it happen, and end this suffering.
    /// </summary>
    /// <returns>A new dictionary of meeples that may be accessed using tile coordinates.</returns>
    public static Dictionary<Vector2Int, int> BuildMeepleMap(AIWrapper wrapper)
    {
        const int bitMask3 = 0x7; // 3-bit mask.

        List<Meeple> allMeeps = wrapper.state.meeples.All;
        Dictionary<Vector2Int, int> mappedMeeps = new Dictionary<Vector2Int, int>(allMeeps.Count);
        foreach (Meeple meep in allMeeps)
        {
            if (meep.free)
                continue;

            int meepleData = 0x0;
            meepleData |= meep.playerId & bitMask3;              // Insert 3-bit player id for meeple. Must be between 0-7.
            meepleData |= ((int)meep.direction & bitMask3) << 3; // Insert 3-bit value for meeple direction.

            mappedMeeps[new Vector2Int(meep.x, meep.z)] = meepleData;
        }

        return mappedMeeps;
    }

    /// <summary>
    /// Adds observations to the given sensor based on the given game state.
    /// This method corresponds to <see cref="ObservationApproach.TileIds"/>.
    /// 
    /// Tile ID and rotation is packed into one observation.
    /// Meeple direction on the tile, and the player ID of its owner are
    /// packed into another observation.
    /// In total, 2 observations are made per tile.
    /// </summary>
    /// <param name="wrapper">Used to access the state of the game.</param>
    /// <param name="sensor">The observations are added to this sensor.</param>
    public static void AddTileIdObservations(AIWrapper wrapper, VectorSensor sensor)
    {
        Dictionary<Vector2Int, int> meepleMap = BuildMeepleMap(wrapper);
        Tile[,] tiles = (Tile[,])wrapper.GetTiles();

        for (int row = 0; row < tiles.GetLength(0); row++)
        {
            for (int col = 0; col < tiles.GetLength(1); col++)
            {
                Tile tile = tiles[col, row];
                if (tile == null)
                {
                    sensor.AddObservation(0.0f);
                    sensor.AddObservation(-1.0f);
                    continue;
                }

                float obs = tile.id + tile.rotation * 100; // Note that tile ids must not exceed 99.
                sensor.AddObservation(obs);

                // Add meeple data as a seperate observation.
                int meepleData = 0x00;
                if (meepleMap.TryGetValue(new Vector2Int(col, row), out meepleData))
                {
                    // Normalize by maximum, which is 6 bits set (63).
                    float normalizedMeepleData = meepleData / (float)(0x3F);
                    sensor.AddObservation(normalizedMeepleData);
                }
                else // If there was no meeple placed on this tile.
                {
                    sensor.AddObservation(-1.0f);
                }
            }
        }
    }

    /// <summary>
    /// Adds observations to the given sensor based on the given game state.
    /// This method corresponds to <see cref="ObservationApproach.PackedIDs"/>.
    /// 
    /// Tile ID and rotation is packed together with meeple direction on the 
    /// tile, and the player ID of its owner, all into one observation.
    /// In total, 1 observations is made per tile.
    /// </summary>
    /// <param name="wrapper">Used to access the state of the game.</param>
    /// <param name="sensor">The observations are added to this sensor.</param>
    public static void AddPackedTileIdObservations(AIWrapper wrapper, VectorSensor sensor)
    {
        Dictionary<Vector2Int, int> meepleMap = BuildMeepleMap(wrapper);
        Tile[,] tiles = (Tile[,])wrapper.GetTiles();

        for (int row = 0; row < tiles.GetLength(0); row++)
        {
            for (int col = 0; col < tiles.GetLength(1); col++)
            {
                Tile tile = tiles[col, row];
                if (tile == null)
                {
                    sensor.AddObservation(0.0f);
                    continue;
                }

                int meepleData = 0x0;
                if (!meepleMap.TryGetValue(new Vector2Int(col, row), out meepleData))
                {
                    meepleData = 0x1 << 6;
                }

                const int bitMask7 = 0x7F; // 7-bit mask.
                const int bitMask6 = 0x3F; // 6-bit mask.
                const int bitMask2 = 0x03; // 2-bit mask.

                int packedData = 0x0;
                packedData |= (tile.id & bitMask6);             // Tile id       = 6 bits
                packedData |= (tile.rotation & bitMask2) << 6;  // Tile rotation = 2 bits
                packedData |= (meepleData & bitMask7) << 8;     // Meeple data   = 7 bits

                // If there is no meeple, the last 7 bits are '0b100 0000'.

                float obs = packedData / (float)0x7FFF;         // Normalize using maximum (15 bits).
                sensor.AddObservation(obs);
            }
        }
    }

    /// <summary>
    /// Adds observations to the given sensor based on the given game state.
    /// This method corresponds to <see cref="ObservationApproach.Packed"/>.
    /// 
    /// Instead of using tile's ID and rotation, the internal geographies of 
    /// the tile are explicitly packed into one observation. This removes the 
    /// need for observing tile rotation, as it is implied by the geographies.
    /// Meeple direction on the tile, and the player ID of its owner are
    /// packed into another observation.
    /// In total, 2 observations are made per tile.
    /// </summary>
    /// <param name="wrapper">Used to access the state of the game.</param>
    /// <param name="sensor">The observations are added to this sensor.</param>
    public static void AddPackedTileObservations(AIWrapper wrapper, VectorSensor sensor)
    {
        Dictionary<Vector2Int, int> meepleMap = BuildMeepleMap(wrapper);
        Tile[,] tiles = (Tile[,])wrapper.GetTiles();

        for (int row = 0; row < tiles.GetLength(0); row++)
        {
            for (int col = 0; col < tiles.GetLength(1); col++)
            {
                Tile tile = tiles[col, row];
                int tileData = 0x0;
                int meepleData = 0x0;

                if (tile == null)
                {
                    sensor.AddObservation(-1.0f); // Tile
                    sensor.AddObservation(-1.0f); // Meeple
                    continue;
                }

                const int bitMask4 = 0xF; // 4-bit mask.

                tileData |= ((int)tile.Center & bitMask4);
                tileData |= (((int)tile.East & bitMask4) << 4);
                tileData |= (((int)tile.North & bitMask4) << 8);
                tileData |= (((int)tile.West & bitMask4) << 12);
                tileData |= (((int)tile.South & bitMask4) << 16);

                // Load the meeple data into "meepleData" if there is a meeple on this tile.
                if (!meepleMap.TryGetValue(new Vector2Int(col, row), out meepleData))
                {
                    // If there was no meeple, set only the 7th bit (bit 6),
                    // in order to indicate 'no meeple'.
                    meepleData = 0x1 << 6;
                }

                // Normalize by maximum, which is 20 bits set (1,048,575).
                float normalizedTileData = tileData / (float)(0xFFFFF);

                // Normalize by maximum, which is 7 bits set (127).
                float normalizedMeepleData = meepleData / (float)0x7F;

                sensor.AddObservation(normalizedTileData);
                sensor.AddObservation(normalizedMeepleData);
            }
        }
    }
}

