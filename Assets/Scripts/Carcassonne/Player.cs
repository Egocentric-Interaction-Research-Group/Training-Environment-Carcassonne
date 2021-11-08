using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Carcassonne;
using Carcassonne.State;

public class Player : MonoBehaviour
{

    private readonly int id;
    private List<Meeple> meeples = new List<Meeple>();
    private int score;

    public MeepleState meepleState;

    public Player(int id)
    {
        this.id = id;
        Score = 0;

        Setup();
    }

    public List<Meeple> Meeples { get => meeples; set => meeples = value; }

    public int Id => id;

    public int Score { get => score; set => score = value; }

    private void Setup()
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
