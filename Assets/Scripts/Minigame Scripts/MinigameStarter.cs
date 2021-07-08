using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameStarter : MonoBehaviour
{
    [SerializeField]
    private GameObject minigamePrefab;
    private GameObject spawnedMinigame;
    public MinigameManager minigameManager;
    public string minigameName;

    //isFirst TELLS MINIGAME MANAGER THIS IS VALID FOR STARTING
    public bool isFirst;
    //isFinal TELLS MINIGAME MANAGER IF THIS WAS THE LAST IN ITS SEQUENCE
    public bool isFinal;
    //index IS INDEX IN MINIGAME MANAGER
    public int index;
    //isPersistent MUST BE THE SAME FOR EACH MINIGAME + STARTER
    public bool isPersistent;
    private bool isSpawned = false;
    public bool isEmergency;

    public string localPlayerMinigameCheckerTag;
    private bool isInRange = false;
    private bool isOpen = false;

    public string incompleteText;
    public string completeText;

    //target is the main player object
    private GameObject target;

    [SerializeField]
    private GameObject minigameGlow;

    // Update is called once per frame
    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E) && !isOpen)
        {
            OpenMinigame();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(localPlayerMinigameCheckerTag))
        {
            //The checker is one level below the parent object, so this will allow me to access player related scripts
            target = collision.transform.parent.gameObject;
            minigameGlow.SetActive(true);
            isInRange = true;
        }
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.CompareTag(localPlayerMinigameCheckerTag))
    //    {
    //        //The checker is one level below the parent object, so this will allow me to access player related scripts
    //        target = collision.transform.parent.gameObject;
    //    }
    //}

    private void OnTriggerExit2D(Collider2D collision)
    {
        minigameGlow.SetActive(false);
        isInRange = false;
    }

    private void OpenMinigame()
    {
        isOpen = true;

        if (isPersistent)
        {
            if (!isSpawned)
            {
                target.GetComponent<PlayerMovement>().StopMovement();
                target.GetComponent<PlayerShoot>().DisableShooting();
                spawnedMinigame = Instantiate(minigamePrefab, target.transform.position, target.transform.rotation);
                spawnedMinigame.GetComponent<Minigame>().SetUp(target, this);

                isSpawned = true;
            }
            else
            {
                target.GetComponent<PlayerMovement>().StopMovement();
                target.GetComponent<PlayerShoot>().DisableShooting();
                spawnedMinigame.SetActive(true);
                spawnedMinigame.transform.position = target.transform.position;
            }
        }
        else
        {
            target.GetComponent<PlayerMovement>().StopMovement();
            target.GetComponent<PlayerShoot>().DisableShooting();
            spawnedMinigame = Instantiate(minigamePrefab, target.transform.position, target.transform.rotation);
            spawnedMinigame.GetComponent<Minigame>().SetUp(target, this);
        }
    }

    public void CloseMinigame()
    {
        isOpen = false;
    }

    public void RemoteCloseMinigame()
    {
        if (isOpen)
        {
            spawnedMinigame.GetComponent<Minigame>().CloseMinigame();
        }
    }

    public void MinigameCompleted()
    {
        if (!isEmergency)
        {
            isSpawned = false;
            minigameGlow.SetActive(false);
            minigameManager.MinigameCompleted(index, isFinal);
            isOpen = false;
            gameObject.SetActive(false);
        }
        else
        {
            isSpawned = false;
            minigameGlow.SetActive(false);
            minigameManager.GlobalMinigameCompleted(index);
            isOpen = false;
            gameObject.SetActive(false);
        }
    }

    public void ClearMinigame()
    {
        if (isSpawned)
        {
            Destroy(spawnedMinigame);
            isSpawned = false;
        }

        if (isOpen)
        {
            RemoteCloseMinigame();
        }

        minigameGlow.SetActive(false);

        gameObject.SetActive(false);
    }
}
