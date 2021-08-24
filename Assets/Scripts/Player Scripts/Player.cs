using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //gameRole 0 = n/a
    //gameRole 1 = innocent
    //gameRole 2 = traitor
    //gameRole 3 = detective? may go unused
    public enum Role
    {
        Innocent,
        Traitor
    }

    private Role gameRole;

    public static Player instance;

    public float maxHealth = 100f;
    private float health = 100f;
    public bool isAlive = true;
    public int meetingsRemaining;

    public float hitboxDeactivationTimer = 0.2f;

    [HideInInspector]
    public bool isRoundReady = false;

    [SerializeField] //REVOKE THIS PRIVELAGE WHEN APPLICABLE
    private Role gameRoleTester;

    [SerializeField]
    private PlayerShoot playerShoot;
    [SerializeField]
    private PlayerMovement playerMovement;
    [SerializeField]
    private PlayerUI playerUI;

    [SerializeField]
    private GameObject playerBody;
    [SerializeField]
    private GameObject playerOutline;

    //Used to see other ghosts (and yourself I guess)
    public GameObject ghostCamera;
    //Used so ghosts can see lol
    public GameObject ghostViewSquare;

    private float defaultViewRadius = 100f;

    private bool inMenu = false;


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

    // Start is called before the first frame update
    void Start()
    {
        //Subscribe to events here
        SettingsUI.instance.OnSettingsOpen += OnSettingsOpen;
        SettingsUI.instance.OnSettingsClose += OnSettingsClose;

        gameRole = gameRoleTester;
        health = maxHealth;
    }

    public Role GetGameRole()
    {
        return gameRole;
    }

    public void TakeDamage(float _damage)
    {
        health -= _damage;

        if(health < 0)
        {
            health = 0;
        }

        playerUI.TakeDamage(health, maxHealth);

        //if (health <= 0)
        //{
        //    Die();
        //}
    }

    //public void CompleteMinigame(bool isEmergency)
    //{
    //    playerUI.CompleteMinigame(isEmergency);
    //}

    public void Die()
    {
        isAlive = false;

        playerUI.Die();

        SpriteRenderer _playerBodySprite = playerBody.GetComponent<SpriteRenderer>();
        _playerBodySprite.color = new Color(_playerBodySprite.color.r, _playerBodySprite.color.g, _playerBodySprite.color.b, 0.5f);
        playerOutline.SetActive(false);

        
        ghostCamera.SetActive(true);
        ghostViewSquare.SetActive(true);
        playerShoot.DisableShooting();


        //int _layer = LayerMask.NameToLayer("Ghost");
        //gameObject.layer = _layer;
        //playerBody.layer = _layer;
        //playerOutline.layer = _layer;
        StartCoroutine(DelayHitboxDeactivation(hitboxDeactivationTimer));

        ResetWeapons();
    }

    public void ResetWeapons()
    {
        gameObject.GetComponent<WeaponManager>().ResetWeapons();
    }

    //Used to fight bullets seeming to continue through the players that they killed
    private IEnumerator DelayHitboxDeactivation(float _timer)
    {
        float t = 0;

        while (t < _timer)
        {
            t += Time.deltaTime;

            yield return null;
        }

        int _layer = LayerMask.NameToLayer("Ghost");
        gameObject.layer = _layer;
        playerBody.layer = _layer;
        playerOutline.layer = _layer;
    }

    //Used at the end of rounds to bring players back, I'm not implementing undead players
    public void Resurrect()
    {
        //isAlive = true;
        health = maxHealth;
        playerUI.TakeDamage(health, maxHealth);

        SpriteRenderer _playerBodySprite = playerBody.GetComponent<SpriteRenderer>();
        _playerBodySprite.color = new Color(_playerBodySprite.color.r, _playerBodySprite.color.g, _playerBodySprite.color.b, 1f);
        playerOutline.SetActive(true);
        

        ghostCamera.SetActive(false);
        ghostViewSquare.SetActive(false);

        //We don't want shooting in the lobby
        //playerShoot.AllowShooting();


        int _colliderLayer = LayerMask.NameToLayer("Default");
        int _visualLayer = LayerMask.NameToLayer("BehindMask");
        gameObject.layer = _colliderLayer;
        playerBody.layer = _visualLayer;
        playerOutline.layer = _visualLayer;
    }

    public void SetRole(int _role)
    {
        if(_role == 1)
        {
            gameRole = Role.Innocent;
        }
        else if(_role == 2)
        {
            gameRole = Role.Traitor;
        }

        playerUI.SetRole(gameRole);
    }

    public void RemoteDeath(int _causeOfDeath)
    {
        Die();
    }

    //One place for all of these things
    public void SetGameplayVariables(float _playerSpeed, float _viewRadius, int _startingMeetings)
    {
        isAlive = true;

        playerShoot.AllowShooting();
        gameObject.GetComponent<WeaponManager>().SetWeapon(0);
        meetingsRemaining = _startingMeetings;
        

        bool _isTraitor = false;
        if(gameRole == Role.Traitor)
        {
            _isTraitor = true;
        }

        gameObject.GetComponent<PlayerMovement>().SetGameplayVariables(_playerSpeed, _viewRadius, _isTraitor);
    }

    public void AssignTasks(int _numTasks, int _numInnocents)
    {
        bool _isInnocent = true;
        if(gameRole == Role.Traitor)
        {
            _isInnocent = false;
        }

        MinigameManager.instance.StartRound(_numTasks, _isInnocent);

        //Player UI takes _numInnocents and _numTasks so it can create a good task bar at the top
    }

    
    private void OnSettingsOpen(object _sender, EventArgs _e)
    {
        inMenu = true;
    }

    private void OnSettingsClose(object _sender, EventArgs _e)
    {
        inMenu = false;
    }

    private void OnDestroy()
    {
        //Unsubscribe from events here
        SettingsUI.instance.OnSettingsOpen -= OnSettingsOpen;
        SettingsUI.instance.OnSettingsClose -= OnSettingsClose;
    }

    public void StartCutscene()
    {
        playerMovement.StartCutscene();
        playerShoot.StartCutscene();
    }

    public void EndCutscene()
    {
        playerMovement.EndCutscene();
        playerShoot.EndCutscene();
    }

    public void StartMeeting()
    {
        playerMovement.StartMeeting();
        playerShoot.StartMeeting();
    }

    public void EndMeeting()
    {
        playerMovement.EndMeeting();
        playerShoot.EndMeeting();
    }

    public PlayerShoot GetPlayerShoot()
    {
        return playerShoot;
    }

    public void ResetViewRadius()
    {
        gameObject.GetComponent<PlayerMovement>().SetViewRadius(defaultViewRadius);
    }
}
