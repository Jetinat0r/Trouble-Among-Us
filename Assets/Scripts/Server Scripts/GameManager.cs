using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    public List<GameObject> deadBodies = new List<GameObject>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    public GameObject bulletPrefab;

    public GameObject emergencyMeetingPrefab;

    //Tells the FixedUpdate funtion to disconnect the player
    private bool shouldDisconnect = false;

    public string mainMenuSceneName = "Main Menu";

    [Header("Cutscene Scriptable Objects")]

    [SerializeField]
    private RoundChangeScreen emergencyMeetingIntroCutscene;
    [SerializeField]
    private RoundChangeScreen goodRoundStartCutscene;
    [SerializeField]
    private RoundChangeScreen evilRoundStartCutscene;
    [SerializeField]
    private RoundChangeScreen goodRoundEndCutscene;
    [SerializeField]
    private RoundChangeScreen evilRoundEndCutscene;

    [HideInInspector]
    public string localPlayerUsername;

    private void Awake()
    {
        if(instance == null)
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

    private void FixedUpdate()
    {
        if (shouldDisconnect)
        {
            shouldDisconnect = false;
            Disconnect();
        }
    }

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation, int _curWeapon, Color _color, Vector3 _spawnLocation)
    {
        GameObject _player;
        if(_id == Client.instance.myId)
        {
            _player = Instantiate(localPlayerPrefab, _position, Quaternion.identity);
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, Quaternion.identity);
        }

        PlayerManager _playerManager = _player.GetComponent<PlayerManager>();

        _playerManager.id = _id;
        _playerManager.username = _username;
        _playerManager.curWeapon = _curWeapon;
        _playerManager.playerColor = _color;

        _playerManager.SetSpawnState();
        _playerManager.RemoteTeleport(_spawnLocation);

        players.Add(_id, _playerManager);
    }

    public void SpawnBullet(Vector3 _position, Quaternion _rotation, float _velocity, Vector3 _shotPosOffset)
    {
        GameObject _curBullet = Instantiate(bulletPrefab, _position, _rotation);

        Bullet _bulletScript = _curBullet.GetComponent<Bullet>();

        _bulletScript.velocity = _velocity;
        _bulletScript.shotPosOffset = _shotPosOffset;
    }

    public void ConnectToServer(string _gameSceneName, string _username)
    {
        SceneManager.LoadScene(_gameSceneName);

        localPlayerUsername = _username;

        Client.instance.ConnectToServer();
    }

    //Here to avoid threading issues
    public void EnableDisconnect()
    {
        shouldDisconnect = true;
    }

    private void Disconnect()
    {
        players = new Dictionary<int, PlayerManager>();
        deadBodies = new List<GameObject>();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    //Really should have done more here :P
    public void StartRound()
    {
        GraveyardManager.instance.ResetGraveyard();
        Player.instance.isRoundReady = false;

        ReadyUpButton.instance.RoundStart();

        if(Player.instance.GetGameRole() == Player.Role.Innocent)
        {
            goodRoundStartCutscene.StartCutscene(this);
        }
        else if (Player.instance.GetGameRole() == Player.Role.Traitor)
        {
            evilRoundStartCutscene.StartCutscene(this);
        }
        else
        {
            Debug.LogWarning("Role not set!");
        }
    }

    #region Emergency Meeting Stuff
    private EmergencyMeetingManager tempEmergencyMeetingManagerScript;
    private int tempMeetingPlayerID = -1;
    private float tempMeetingTimer = -1;

    public void StartEmergencyMeeting(int _playerID, float _timer)
    {
        MinigameManager.instance.RemoteCloseMinigames();

        emergencyMeetingIntroCutscene.OnCutsceneEnd += EndMeetingIntro;

        tempMeetingPlayerID = _playerID;
        tempMeetingTimer = _timer;

        emergencyMeetingIntroCutscene.StartCutscene(this);
    }

    private void EndMeetingIntro(object sender, EventArgs _e)
    {
        emergencyMeetingIntroCutscene.OnCutsceneEnd -= EndMeetingIntro;

        SetupMeeting(tempMeetingPlayerID, tempMeetingTimer);
        tempMeetingPlayerID = -1;
        tempMeetingTimer = -1;
    }

    private void SetupMeeting(int _playerID, float _timer)
    {
        GameObject meeting = Instantiate(emergencyMeetingPrefab);
        meeting.GetComponent<EmergencyMeetingManager>().StartMeeting(_playerID, _timer);
        tempEmergencyMeetingManagerScript = meeting.GetComponent<EmergencyMeetingManager>();
    }

    public void EndEmergencyMeeting(int[] targetPlayers, Color[] fromPlayers, float endingTimer)
    {
        tempEmergencyMeetingManagerScript.DisplayVotes(targetPlayers, fromPlayers);
        tempEmergencyMeetingManagerScript.EndMeeting(endingTimer);
    }
    #endregion

    public void EndRound(int victoryType)
    {
        //0: Innocent Win
        //1: Traitor Win

        //Kill the player and rez them using the cutscene's delegates

        Debug.Log("VT: " + victoryType);
        RoundChangeScreen victoryScreen;
        if(victoryType == 0)
        {
            victoryScreen = goodRoundEndCutscene;
        }
        else if(victoryType == 1)
        {
            victoryScreen = evilRoundEndCutscene;
        }
        else
        {
            Debug.LogWarning("Unassigned Victory!");
            victoryScreen = goodRoundEndCutscene;
        }

        victoryScreen.preActionDelegate += new RoundChangeScreen.PreActionDelegate(PreVictoryScreen);
        victoryScreen.postActionDelegate += new RoundChangeScreen.PostActionDelegate(PostVictoryScreen);

        victoryScreen.StartCutscene(this);
        StartCoroutine(VictoryScreenUnsub(victoryScreen.cutsceneTimer, victoryScreen));

        foreach(GameObject corpse in deadBodies)
        {
            Destroy(corpse);
        }
    }

    private IEnumerator VictoryScreenUnsub(float waitTime, RoundChangeScreen cutscene)
    {
        yield return new WaitForSeconds(waitTime + 0.01f);
    }

    private void PreVictoryScreen()
    {
        Player.instance.Die();
    }

    private void PostVictoryScreen()
    {
        Player.instance.Resurrect();

        //Resurrect everyone else
        foreach (KeyValuePair<int, PlayerManager> pair in GameManager.players)
        {
            if (pair.Value != null && !pair.Value.isLocalPlayer)
            {
                pair.Value.Resurrect();
            }
        }

        Player.instance.ResetViewRadius();
    }
}
