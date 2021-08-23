using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log("Message from server: " + _msg);

        Client.instance.myId = _myId;
        ClientSend.WelcomeRecieved();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void UDPTest(Packet _packet)
    {
        string _msg = _packet.ReadString();

        Debug.Log("Received packet via UDP. Contains message: " + _msg);
        ClientSend.UDPTestReceived();
    }

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();

        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        int _curWeapon = _packet.ReadInt();
        Color _color = _packet.ReadColor();

        Vector3 _spawnLocation = _packet.ReadVector3();

        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation, _curWeapon, _color, _spawnLocation);
    }

    public static void PlayerMovement(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        if (Client.instance.isConnected)
        {
            GameManager.players[_id].PlayerMovement(_position, _rotation);
        }
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _role = _packet.ReadInt();

        if (Client.instance.isConnected)
        {
            if(_role == 0)
            {
                GameManager.players[_id].RemoteDeath(-1);
            }
            else if(GameManager.players[_id].isAlive)
            {
                GameManager.players[_id].RemoteDeath(0);
            }

            Destroy(GameManager.players[_id].gameObject);
            GameManager.players.Remove(_id);
        }
    }

    public static void RemoteDisconnect(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _msg = _packet.ReadString();

        Debug.Log("Player id: " + _id +" forcefully disconnected from server. Reason: " + _msg);

        Client.instance.Disconnect();
    }

    public static void SetRole(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _role = _packet.ReadInt();

        GameManager.players[_id].SetRole(_role);
    }

    public static void RemoteWeaponSwap(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _weapon = _packet.ReadInt();

        GameManager.players[_id].RemoteWeaponSwap(_weapon);
    }

    public static void SpawnBullet(Packet _packet)
    {
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        float _velocity = _packet.ReadFloat();
        Vector3 _shotPosOffset = _packet.ReadVector3();

        GameManager.instance.SpawnBullet(_position, _rotation, _velocity, _shotPosOffset);
    }

    public static void DamagePlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        float _amount = _packet.ReadFloat();

        GameManager.players[_id].TakeDamage(_amount);
    }

    public static void RemoteDeath(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _causeOfDeath = _packet.ReadInt();

        GameManager.players[_id].RemoteDeath(_causeOfDeath);
    }

    public static void StartRound(Packet _packet)
    {
        int _numActivePlayer = _packet.ReadInt();
        int _numInnocents = _packet.ReadInt();
        int[] _roleArray = _packet.ReadIntArray();
        int _tasksPerPlayer = _packet.ReadInt();
        float _playerSpeed = _packet.ReadFloat();
        float _visionRadius = _packet.ReadFloat();

        //Since this contains a role for every playing player, we can assign tasks here

        //Put each active player's id into an array
        int[] _activePlayerIds = new int[GameManager.players.Count];
        int _aPIdPos = 0;
        foreach (KeyValuePair<int, PlayerManager> _player in GameManager.players)
        {
            //Check num cur players, [each key.player != null]
            if (_player.Value != null)
            {
                _activePlayerIds[_aPIdPos] = _player.Value.id;
                _aPIdPos++;
            }
        }

        //Sort the ids (really this easy huh)
        Array.Sort(_activePlayerIds);

        PlayerManager _localPlayer = null;

        for (int i = 0; i < _activePlayerIds.Length; i++)
        {
            PlayerManager _player = GameManager.players[_activePlayerIds[i]];
            if (_player != null)
            {
                if (_player.isLocalPlayer)
                {
                    _localPlayer = _player;
                }

                //Kinda dumb implementation (SetRole could be called in SetGameplayVariables), just want to assure things go right and threading doesn't hurt me
                _player.SetRole(_roleArray[i]);
                _player.AssignTasks(_tasksPerPlayer, _numInnocents);
                _player.SetGameplayVariables(_playerSpeed, _visionRadius);
                _player.SetNameplateColor(false);
            }
        }

        //Sets nameplates to correct colors
        //Must be called after the previous thing bc I don't actually know the id of the local player
        if(_localPlayer.gameRole == 1)
        {
            for (int i = 0; i < _activePlayerIds.Length; i++)
            {
                PlayerManager _player = GameManager.players[_activePlayerIds[i]];

                if (_player != null)
                {
                    _player.SetNameplateColor(true);
                }
            }
        }

        GameManager.instance.StartRound();
    }

    public static void AssignEmergency(Packet _packet)
    {
        int _emergencyID = _packet.ReadInt();

        MinigameManager.instance.AssignGlobalMinigame(MinigameManager.instance.globalMinigameStarters[_emergencyID]);
    }

    public static void RemoteCompleteEmergency(Packet _packet)
    {
        int _emergencyID = _packet.ReadInt();

        MinigameManager.instance.GlobalMinigameCompleted(MinigameManager.instance.globalMinigameStarters[_emergencyID]);
    }

    public static void RemoteTeleport(Packet _packet)
    {
        int _playerID = _packet.ReadInt();
        Vector3 _targetPos = _packet.ReadVector3();

        GameManager.players[_playerID].RemoteTeleport(_targetPos);
    }

    public static void RemoteCompleteTask(Packet _packet)
    {
        int _playerID = _packet.ReadInt();

        GameManager.players[_playerID].RemoteCompleteTask();
    }

    public static void RemoteSendVoiceChat(Packet _packet)
    {
        int _playerID = _packet.ReadInt();

        float[] voiceSamples = _packet.ReadFloatArray();
        int samples = _packet.ReadInt();
        int channels = _packet.ReadInt();
        int maxFreq = _packet.ReadInt();
        bool isRadioActive = _packet.ReadBool();

        PlayerManager _player = GameManager.players[_playerID];

        if (_player != null)
        {
            _player.PlayVCRecording(voiceSamples, samples, channels, maxFreq, isRadioActive);
        }
    }

    public static void RemoteStartEmergencyMeeting(Packet _packet)
    {
        int _playerID = _packet.ReadInt();
        float _timer = _packet.ReadFloat();

        if(Client.instance.myId == _playerID)
        {
            GameManager.players[_playerID].gameObject.GetComponent<Player>().meetingsRemaining--;
        }

        GameManager.instance.StartEmergencyMeeting(_playerID, _timer);
    }

    public static void RemoteSendMeetingVotes(Packet _packet)
    {
        int[] toPlayerIDs = _packet.ReadIntArray();
        Color[] _fromPlayers = _packet.ReadColorArray();
        float _endEmergencyMeetingTimer = _packet.ReadFloat();

        GameManager.instance.EndEmergencyMeeting(toPlayerIDs, _fromPlayers, _endEmergencyMeetingTimer);
    }
}
