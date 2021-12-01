using System.Collections.Generic;
using UnityEngine;
using Carcassonne;

public class Player : MonoBehaviour
{

    public int id;
    public int score = 0;

    public List<Meeple> meeples = new List<Meeple>();
    public MeepleState meepleState;

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
