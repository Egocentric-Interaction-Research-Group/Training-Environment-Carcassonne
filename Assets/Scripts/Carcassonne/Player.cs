using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Carcassonne;
using Carcassonne.State;

public class Player : MonoBehaviour
{

    private readonly int id;
    private GameObject[] meeples = new GameObject[7];
    private int score;

    public MeepleState meepleState;

    public Player(int id)
    {
        this.id = id;

        Score = 0;

        Setup();
    }

    public GameObject[] Meeples { get => meeples; set => meeples = value; }

    public int Id => id;

    public int Score { get => score; set => score = value; }

    private void Setup()
    {
        var meepleControllerScript = GameObject.Find("GameController").GetComponent<MeepleControllerScript>();
        for (var i = 0; i < meeples.Length; i++)
        {
            var meeple = meepleControllerScript.GetNewInstance();
            meepleState.All.Add(meeple);
        }
    }
}
