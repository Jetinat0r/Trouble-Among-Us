using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public static SettingsUI instance;

    public event EventHandler OnSettingsOpen;
    public event EventHandler OnSettingsClose;

    //Separated so I don't call an inactive script
    public List<GameObject> playerVCSliderObjects;
    public List<SliderAndInputFieldConnector> playerVCConnectors;

    [SerializeField]
    private GameObject microphoneSelectionButtonsParent;
    [SerializeField]
    private GameObject microphoneSelectionButtonPrefab;

    [SerializeField]
    private GameObject serverDisconnectMenuButton;

    private bool isInGame = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying!");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Client.instance.OnServerConnect += OnServerConnect;
        Client.instance.OnServerDisconnect += OnServerDisconnect;
    }

    public void ToggleMenu(GameObject _menu)
    {
        _menu.SetActive(!_menu.activeSelf);

        if (_menu.activeSelf)
        {
            OnSettingsOpen?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            OnSettingsClose?.Invoke(this, EventArgs.Empty);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeVolumeInputFieldValue(SliderAndInputFieldConnector _connector)
    {
        bool isSlider = _connector.DetermineInputDevice();

        if (isSlider)
        {
            _connector.inputField.text = _connector.slider.value.ToString();
            _connector.lastInput = _connector.slider.value;
        }
        else
        {
            float _volumeValue = float.Parse(_connector.inputField.text);

            _volumeValue = Mathf.Clamp(_volumeValue, 0f, 50f);
            _connector.inputField.text = _volumeValue.ToString();

            _connector.slider.value = _volumeValue;
            _connector.lastInput = _connector.slider.value;
        }

        _connector.SetPlayerManagerVolume();
    }

    public void SetupPlayerVolumeSliders()
    {
        foreach(GameObject _go in playerVCSliderObjects)
        {
            _go.SetActive(false);
        }

        int _curActiveSliders = 0;

        for(int i = 0; i < GameManager.players.Count; i++)
        {
            PlayerManager _player = GameManager.players[i + 1];

            //Checks if that player exists, and if that player is NOT the local player
            if(_player != null && !_player.isLocalPlayer)
            {
                playerVCSliderObjects[_curActiveSliders].SetActive(true);

                SliderAndInputFieldConnector _connector = playerVCConnectors[_curActiveSliders];
                _connector.playerName.text = _player.username;
                _connector.playerID = _player.id;
                _connector.slider.value = _connector.lastInput;
                _connector.inputField.text = _connector.lastInput.ToString();

                _curActiveSliders++;
            }
        }
    }

    public void SetupMicrophoneOptions()
    {
        foreach(Transform child in microphoneSelectionButtonsParent.transform)
        {
            Destroy(child.gameObject);
        }

        string[] availableMics = Microphone.devices;

        //Prefab structure
        //Child 0 = text
        //Child 1 = checkbox
            //Child 0 = check mark
        foreach (string mic in availableMics)
        {
            GameObject micButton = Instantiate(microphoneSelectionButtonPrefab, microphoneSelectionButtonsParent.transform);

            micButton.transform.GetChild(0).GetComponent<TMP_Text>().text = mic;
            micButton.GetComponent<Button>().onClick.AddListener(() => SelectMicrophone(micButton));

            if(mic == MicrophoneManager.instance.GetCurrentMicrophone())
            {
                micButton.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    public void SelectMicrophone(GameObject selectedButton)
    {

        //Prefab structure
        //Child 0 = text
        //Child 1 = checkbox
            //Child 0 = check mark
        foreach (Transform child in microphoneSelectionButtonsParent.transform)
        {
            //turn off check buttons
            child.GetChild(1).GetChild(0).gameObject.SetActive(false);
        }

        selectedButton.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);

        MicrophoneManager.instance.SetCurrentMicrophone(selectedButton.transform.GetChild(0).GetComponent<TMP_Text>().text);
    }

    public void OnServerConnect(object sender, EventArgs e)
    {
        isInGame = true;
        serverDisconnectMenuButton.SetActive(true);
    }

    public void OnServerDisconnect(object sender, EventArgs e)
    {
        isInGame = false;
        Debug.Log(2);
        serverDisconnectMenuButton.SetActive(false);
    }

    public void DisconnectFromServer()
    {
        if (isInGame)
        {
            Client.instance.Disconnect();
        }
    }

    private void OnDestroy()
    {
        Client.instance.OnServerConnect -= OnServerConnect;
        Client.instance.OnServerDisconnect -= OnServerDisconnect;
    }
}
