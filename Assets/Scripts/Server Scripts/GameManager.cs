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

    //Tells the FixedUpdate funtion to disconnect the player
    private bool shouldDisconnect = false;

    public string mainMenuSceneName = "Main Menu";

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

    public void StartRound()
    {
        //TODO: res everyone, assign tasks, hmm
    }
}
