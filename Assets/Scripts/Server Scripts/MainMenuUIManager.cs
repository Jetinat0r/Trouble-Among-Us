using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUIManager : MonoBehaviour
{
    public static MainMenuUIManager instance;

    public GameObject mainMenu;
    [SerializeField]
    private TMP_InputField usernameField;
    [SerializeField]
    private TMP_InputField serverIPAdressField;
    [SerializeField]
    private TMP_InputField serverPortField;

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
        //Check if any field is invalid

        if(usernameField.text != "" /*&& serverIPAdressField.text != "" && serverPortField.text != ""*/)
        {
            //TODO: Add functionality for choosing server
            GameManager.instance.ConnectToServer(gameSceneName, usernameField.text);
        }
    }
}
