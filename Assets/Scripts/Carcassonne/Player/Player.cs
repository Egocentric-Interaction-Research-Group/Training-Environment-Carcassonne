using System.Collections.Generic;
using UnityEngine;
using Carcassonne;

/// <summary>
/// A player in the Carcassonne game
/// </summary>
public class Player : MonoBehaviour
{

    public int id;
    public int score = 0;

    public List<Meeple> meeples = new List<Meeple>();
    public MeepleState meepleState;

    /// <summary>
    /// Setup the data neccessary for the player. Clear previous meeples and create a new set of meeples that
    /// are also added to the meeple state
    /// </summary>
    public void Setup()
    {
        meeples.Clear();
        for (int i = 0; i < 7; i++)
        {
            Meeple meeple = new Meeple();
            meeple.playerId = id;
            meeple.free = true;
            meepleState.All.Add(meeple);
            meeples.Add(meeple);
        }
    }

    /// <summary>
    /// Returns how many free to use meeples the player currently has
    /// </summary>
    /// <returns></returns>
    public int AmountOfFreeMeeples()
    {
        int count = 0;
        foreach (Meeple meeple in meeples)
        {
            if (meeple.free) { count++; }
        }
        return count;
    }
}
