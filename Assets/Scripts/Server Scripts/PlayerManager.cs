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

    [Header("Voice Chat")]
    [SerializeField]
    private AudioSource proxyAudioSource;
    [SerializeField]
    private AudioSource globalAudioSource;

    [HideInInspector]
    public int completedTasks;
    [HideInInspector]
    public int totalTasks;

    private float vcVolume = 1;

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
        nameplate.GetComponent<TMP_Text>().text = username;
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

        if (_causeOfDeath == 0)
        {
            GameObject _playerCorpse = Instantiate(playerCorpsePrefab, transform.position, transform.rotation);
            Color _playerBodySprite = playerBody.GetComponent<SpriteRenderer>().color;
            _playerCorpse.GetComponent<SpriteRenderer>().color = new Color(_playerBodySprite.r, _playerBodySprite.g, _playerBodySprite.b, 1f);
            GameManager.instance.deadBodies.Add(_playerCorpse);

        }
        else if (_causeOfDeath == 1)
        {
            //Spawn outside
            GameObject _playerCorpse = Instantiate(playerCorpsePrefab, GraveyardManager.instance.GetNextGravePosition());
            Color _playerBodySprite = playerBody.GetComponent<SpriteRenderer>().color;
            _playerCorpse.GetComponent<SpriteRenderer>().color = new Color(_playerBodySprite.r, _playerBodySprite.g, _playerBodySprite.b, 1f);
            GameManager.instance.deadBodies.Add(_playerCorpse);
        }

        if (isLocalPlayer)
        {
            gameObject.GetComponent<Player>().RemoteDeath(_causeOfDeath);
            isAlive = false;
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

    public void Resurrect()
    {
        //isAlive = true;

        if (isLocalPlayer)
        {
            GetComponent<Player>().Resurrect();
        }
        else
        {
            SetRole(0);

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
    public void SetGameplayVariables(float _playerSpeed, float _viewRadius, int _startingMeetings)
    {
        isAlive = true;

        if (isLocalPlayer)
        {
            gameObject.GetComponent<Player>().SetGameplayVariables(_playerSpeed, _viewRadius, _startingMeetings);
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

    public Color GetNameplateColor()
    {
        return nameplate.GetComponent<TMP_Text>().color;
    }

    public void SetVCVolume(float _v)
    {
        vcVolume = _v;
    }

    public void PlayVCRecording(float[] voiceSamples, int samples, int channels, int maxFreq, bool isRemoteRadioActive)
    {
        bool isLocalRadioActive = MicrophoneManager.instance.isRadioActive;
        bool isLocalPlayerAlive = GameManager.players[Client.instance.myId].isAlive;

        //IF: Remote player is dead and local isn't, don't bother playing
        if(!isAlive && isLocalPlayerAlive)
        {
            return;
        }


        for (int i = 0; i < voiceSamples.Length; i++)
        {
            voiceSamples[i] = voiceSamples[i] * vcVolume;
        }

        AudioClip voiceClip = AudioClip.Create("Voice", samples, channels, maxFreq, false);
        voiceClip.SetData(voiceSamples, 0);

        //Debug.Log("Bruh");

        //IF: Local player is dead
        if (!isLocalPlayerAlive)
        {
            //IF: Remote player's radio is active OR if they are dead
            if (isRemoteRadioActive || !isAlive)
            {
                //Play on 2D
                globalAudioSource.clip = voiceClip;
                globalAudioSource.Play();
            }
            else
            {
                //Play on 3D
                proxyAudioSource.clip = voiceClip;
                proxyAudioSource.Play();
            }
        }
        else
        {
            //IF: Local player is on their radio AND remote player is on their radio
            if (isLocalRadioActive && isRemoteRadioActive)
            {
                //Play on 2D voice chanel
                globalAudioSource.clip = voiceClip;
                globalAudioSource.Play();
            }
            else
            {
                //Play on 3D voice chanel
                proxyAudioSource.clip = voiceClip;
                proxyAudioSource.Play();
            }
        }
    }
}
