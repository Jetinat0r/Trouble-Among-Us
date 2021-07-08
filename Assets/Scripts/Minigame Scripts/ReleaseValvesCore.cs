using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseValvesCore : Minigame
{
    public int goals = 3;
    private int curCompletedGoals = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            CloseMinigame();
        }
    }

    public void UpdateGoals(int amount)
    {
        curCompletedGoals += amount;

        if(curCompletedGoals == goals)
        {
            CompleteMinigame();
        }
    }
}
