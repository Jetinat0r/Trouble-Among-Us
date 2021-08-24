using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyUpButton : MonoBehaviour
{
    public static ReadyUpButton instance;

    [SerializeField]
    private Color activeColor;
    [SerializeField]
    private Color inactiveColor;

    private bool isInRange = false;

    //Use the same thing as the minigames to determine if the player is close enough
    public string localPlayerMinigameCheckerTag;
    [SerializeField]
    private GameObject proximityGlow;

    [SerializeField]
    private SpriteRenderer mainGraphics;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying!");
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            ReadyUp();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(localPlayerMinigameCheckerTag))
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

    private void ReadyUp()
    {
        if (Player.instance.isRoundReady)
        {
            mainGraphics.color = inactiveColor;
        }
        else
        {
            mainGraphics.color = activeColor;
        }

        Player.instance.isRoundReady = !Player.instance.isRoundReady;

        ClientSend.ClientReadyUp();
    }

    public void RoundStart()
    {

    }
}
