using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Carcassonne;
using Carcassonne.State;

public class Player : MonoBehaviour
{

    public int id;
    public int score = 0;

    public List<Meeple> meeples = new List<Meeple>();
    public MeepleState meepleState;

    public void Setup()
    {
        MeepleController meepleController = GameObject.Find("MeepleController").GetComponent<MeepleController>();
        for (int i = 0; i < 7; i++)
        {
            Meeple meeple = meepleController.GetNewInstance();
            meeple.playerId = id;
            meepleState.All.Add(meeple);
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
