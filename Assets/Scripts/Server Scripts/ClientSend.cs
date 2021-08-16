using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    public static void WelcomeRecieved()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(GameManager.instance.localPlayerUsername);

            SendTCPData(_packet);
        }
    }

    public static void UDPTestReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.udpTestReceived))
        {
            _packet.Write("Received a UDP packet.");

            SendUDPData(_packet);
        }
    }

    public static void PlayerPosRot(Vector3 _position, Quaternion _rotation)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerPosRot))
        {
            _packet.Write(_position);
            _packet.Write(_rotation);

            SendUDPData(_packet);
        }
    }

    public static void WeaponSwap(int _weapon)
    {
        using (Packet _packet = new Packet((int)ClientPackets.weaponSwap))
        {
            _packet.Write(_weapon);

            SendTCPData(_packet);
        }
    }

    public static void Shoot(Vector3 _position, Quaternion _rotation, float _velocity)
    {
        using (Packet _packet = new Packet((int)ClientPackets.shoot))
        {
            _packet.Write(_position);
            _packet.Write(_rotation);
            _packet.Write(_velocity);

            SendTCPData(_packet);
        }
    }

    public static void ClientEmergencyStartRequest(MinigameStarter _ms)
    {
        using (Packet _packet = new Packet((int)ClientPackets.clientEmergencyStartRequest))
        {
            _packet.Write(_ms.index);
            _packet.Write(_ms.isFirst);

            SendTCPData(_packet);
        }
    }

    public static void ClientCompleteEmergency(MinigameStarter _ms)
    {
        using (Packet _packet = new Packet((int)ClientPackets.clientCompleteEmergency))
        {
            _packet.Write(_ms.index);
            _packet.Write(_ms.isFirst);

            SendTCPData(_packet);
        }
    }

    public static void ClientCompleteTask(MinigameStarter _ms)
    {
        using (Packet _packet = new Packet((int)ClientPackets.clientCompleteTask))
        {
            //Index sent jic
            //_packet.Write(_ms.index);

            SendTCPData(_packet);
        }
    }

    public static void ClientSendVoiceChat(float[] voiceSamples, int samples, int channels, int maxFreq, bool isRadioActive)
    {
        using (Packet _packet = new Packet((int)ClientPackets.clientSendVoiceChat))
        {
            _packet.Write(voiceSamples);
            _packet.Write(samples);
            _packet.Write(channels);
            _packet.Write(maxFreq);
            _packet.Write(isRadioActive);

            SendTCPData(_packet);
        }

        //foreach (float f in voiceSamples)
        //{
        //    Debug.Log(f);
        //}
    }

    #endregion
}
