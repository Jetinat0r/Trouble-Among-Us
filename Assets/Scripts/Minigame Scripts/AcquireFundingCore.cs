using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcquireFundingCore : Minigame
{
    public int coins = 10;

    public void CheckWin()
    {
        coins--;

        if (coins == 0)
        {
            CompleteMinigame();
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            CloseMinigame();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Goal"))
        {
            Destroy(collision.gameObject);
            CheckWin();
        }
    }
}
