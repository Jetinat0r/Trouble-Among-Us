using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUIManager : MonoBehaviour
{
    public static MainMenuUIManager instance;

    public GameObject mainMenu;
    public TMP_InputField usernameField;

    public string gameSceneName = "Russian_Lab";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying!");
            Destroy(gameObject);
        }
    }

    public void ConnectToServer()
    {
        //mainMenu.SetActive(false);
        //usernameField.interactable = false;

        GameManager.instance.ConnectToServer(gameSceneName, usernameField.text);
        //Client.instance.ConnectToServer();
        //SceneManager.LoadScene(gameSceneName);
    }
}
