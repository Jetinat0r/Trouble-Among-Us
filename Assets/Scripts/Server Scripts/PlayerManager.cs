using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    //Needs manually set in inspector
    public bool isLocalPlayer = false;

    public int id;
    public string username;

    public bool isAlive;

    public float hitboxDeactivationTimer = 0.2f;

    //0: null (Pseudo-Innocent, used to not count people joining mid-round)
    //1: Innocent
    //2: Traitor
    public int gameRole = 0;

    //0: None
    //1: Pistol
    //2: Shotgun
    public int curWeapon = 0;

    public GameObject weaponPivot;

    public GameObject playerCorpsePrefab;

    [Header("Weapons")]
    public GameObject pistolGraphics;
    public GameObject shotgunGraphics;

    [Header("Graphics")]
    public GameObject playerBody;
    public GameObject playerOutline;

    public Color playerColor;

    [Header("Nameplate")]
    [SerializeField]
    private Color innocentNameplateColor;
    [SerializeField]
    private Color traitorNameplateColor;
    [SerializeField]
    private GameObject nameplate;

    [HideInInspector]
    public int completedTasks;
    [HideInInspector]
    public int totalTasks;


    public void PlayerMovement(Vector3 _position, Quaternion _rotation)
    {
        transform.position = _position;
        weaponPivot.transform.rotation = _rotation;
    }

    public void SetRole(int _role)
    {
        gameRole = _role;

        if (isLocalPlayer)
        {
            gameObject.GetComponent<Player>().SetRole(_role);
        }
    }

    public void RemoteWeaponSwap(int _weapon)
    {
        curWeapon = _weapon;

        if (!isLocalPlayer)
        {
            //Call some graphical changes
            switch (curWeapon)
            {
                case 0:
                    pistolGraphics.SetActive(false);
                    shotgunGraphics.SetActive(false);
                    break;
                case 1:
                    pistolGraphics.SetActive(true);
                    shotgunGraphics.SetActive(false);
                    break;
                case 2:
                    pistolGraphics.SetActive(false);
                    shotgunGraphics.SetActive(true);
                    break;
            }
        }
        else
        {
            gameObject.GetComponent<WeaponManager>().SetWeapon(_weapon);
        }
        
    }

    public void SetSpawnState()
    {
        SetRole(gameRole);
        RemoteWeaponSwap(curWeapon);

        playerBody.GetComponent<SpriteRenderer>().color = playerColor;
    }

    public void TakeDamage(float _amount)
    {
        if (isLocalPlayer)
        {
            gameObject.GetComponent<Player>().TakeDamage(_amount);
        }
    }

    public void RemoteDeath(int _causeOfDeath)
    {
        //Causes Of Death:
        //-1: Dc before round start
        //0: Being Shot
        //1: Being Ejected

        if(_causeOfDeath == 0)
        {
            GameObject _playerCorpse = Instantiate(playerCorpsePrefab, transform.position, transform.rotation);
            Color _playerBodySprite = playerBody.GetComponent<SpriteRenderer>().color;
            _playerCorpse.GetComponent<SpriteRenderer>().color = new Color(_playerBodySprite.r, _playerBodySprite.g, _playerBodySprite.b, 1f);
            GameManager.instance.deadBodies.Add(_playerCorpse);

        }
        else
        {
            //Spawn outside
        }

        if (isLocalPlayer)
        {
            gameObject.GetComponent<Player>().RemoteDeath(_causeOfDeath);
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        isAlive = false;

        SpriteRenderer _playerBodySprite = playerBody.GetComponent<SpriteRenderer>();
        _playerBodySprite.color = new Color(_playerBodySprite.color.r, _playerBodySprite.color.g, _playerBodySprite.color.b, 0.5f);
        playerOutline.SetActive(false);

        //int _layer = LayerMask.NameToLayer("Ghost");
        //gameObject.layer = _layer;
        //playerBody.layer = _layer;
        //playerOutline.layer = _layer;
        StartCoroutine(DelayHitboxDeactivation(hitboxDeactivationTimer));

        RemoteWeaponSwap(0);
    }

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
        nameplate.layer = _layer;
    }

    private void Resurrect()
    {
        //isAlive = true;

        SpriteRenderer _playerBodySprite = playerBody.GetComponent<SpriteRenderer>();
        _playerBodySprite.color = new Color(_playerBodySprite.color.r, _playerBodySprite.color.g, _playerBodySprite.color.b, 1f);
        playerOutline.SetActive(true);

        int _colliderLayer = LayerMask.NameToLayer("Default");
        int _visualLayer = LayerMask.NameToLayer("BehindMask");
        gameObject.layer = _colliderLayer;
        playerBody.layer = _visualLayer;
        playerOutline.layer = _visualLayer;
        nameplate.layer = _visualLayer;
    }

    public void AssignTasks(int _numTasks, int _numInnocents)
    {
        if (gameRole == 1)
        {
            completedTasks = 0;
            totalTasks = _numTasks;
        }
        else
        {
            completedTasks = -1;
            totalTasks = -1;
        }

        if (isLocalPlayer)
        {
            //TODO: Do more (use _numInnocents for length of task bar)
            gameObject.GetComponent<Player>().AssignTasks(_numTasks, _numInnocents);
        }
    }

    //Sets:
    // - Movement Speed
    // - viewRadius
    public void SetGameplayVariables(float _playerSpeed, float _viewRadius)
    {
        isAlive = true;

        if (isLocalPlayer)
        {
            gameObject.GetComponent<Player>().SetGameplayVariables(_playerSpeed, _viewRadius);
        }
    }

    //TODO: Implement
    public void RemoteCompleteTask()
    {
        //Receive completed task
        //Only receive final tasks

        //Updates completed out of not

        completedTasks++;

    }

    public void RemoteTeleport(Vector3 _targetPos)
    {
        transform.position = _targetPos;

        if (isLocalPlayer)
        {
            Camera.main.transform.position = _targetPos;
        }
    }

    public void SetNameplateColor(bool _override)
    {
        if (_override)
        {
            nameplate.GetComponent<TextMeshPro>().color = innocentNameplateColor;
        }
        else
        {
            if(gameRole == 1)
            {
                nameplate.GetComponent<TextMeshPro>().color = innocentNameplateColor;
            }
            else
            {
                nameplate.GetComponent<TextMeshPro>().color = traitorNameplateColor;
            }
        }
    }
}
