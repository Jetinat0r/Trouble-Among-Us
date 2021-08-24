using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EmergencyButtonCore : Minigame
{
    //private NetworkManager networkManager;

    [SerializeField]
    private TextMeshPro text;

    // Start is called before the first frame update
    void Start()
    {
        //networkManager = player.GetComponent<Player>().GetNetworkManager();
        text.SetText(player.GetComponent<Player>().meetingsRemaining + " Meetings Remaining");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            CloseMinigame();
        }
    }

    public void ButtonPress()
    {
        if (player.GetComponent<Player>().meetingsRemaining > 0 && Player.instance.isAlive)
        {
            //networkManager.StartEmergencyMeeting();
            ClientSend.ClientStartEmergencyMeeting();
            //player.GetComponent<Player>().meetingsRemaining--;
            //CloseMinigame();
        }
    }
}
