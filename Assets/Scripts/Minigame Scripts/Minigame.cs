using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviour
{
    [System.NonSerialized]
    public GameObject player;
    //isPersistent MUST BE THE SAME FOR EACH MINIGAME + STARTER
    public bool isPersistent = false;
    public bool isEmergency;

    [System.NonSerialized]
    public MinigameStarter minigameStarter;

    public void CloseMinigame()
    {
        if (isPersistent)
        {
            player.GetComponent<PlayerMovement>().AllowMovement();
            player.GetComponent<PlayerShoot>().AllowShooting();
            minigameStarter.CloseMinigame();
            gameObject.SetActive(false);
        }
        else
        {
            player.GetComponent<PlayerMovement>().AllowMovement();
            player.GetComponent<PlayerShoot>().AllowShooting();
            minigameStarter.CloseMinigame();
            Destroy(gameObject);
        }
    }

    public void SetUp(GameObject _player, MinigameStarter _minigameStarter)
    {
        player = _player;
        minigameStarter = _minigameStarter;
    }

    public void CompleteMinigame()
    {
        player.GetComponent<PlayerMovement>().AllowMovement();
        player.GetComponent<PlayerShoot>().AllowShooting();

        //Makes the text appear
        //player.GetComponent<Player>().CompleteMinigame(isEmergency);

        minigameStarter.MinigameCompleted();
        Destroy(gameObject);
    }
}
