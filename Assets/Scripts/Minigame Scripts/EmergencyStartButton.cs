using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyStartButton : MonoBehaviour
{
    private bool isInRange = false;
    [SerializeField]
    private GameObject connectedMinigame;
    [SerializeField]
    private MinigameStarter minigameStarter;

    //Use the same thing as the minigames to determine if the player is close enough
    public string localPlayerMinigameCheckerTag;
    [SerializeField]
    private GameObject proximityGlow;

    [SerializeField]
    private SpriteRenderer mainGraphics;

    // Update is called once per frame
    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E) && !connectedMinigame.activeSelf)
        {
            RequestEmergency();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(localPlayerMinigameCheckerTag) && Player.instance.GetGameRole() == Player.Role.Traitor && !connectedMinigame.activeSelf)
        {
            //The checker is one level below the parent object, so this will allow me to access player related scripts
            proximityGlow.SetActive(true);
            isInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        proximityGlow.SetActive(false);
        isInRange = false;
    }

    private void RequestEmergency()
    {
        proximityGlow.SetActive(false);
        ClientSend.ClientEmergencyStartRequest(minigameStarter);
    }
}
