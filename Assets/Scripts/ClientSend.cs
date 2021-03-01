using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }


    #region Packets Defintion
    public static void Ping()
    {
        using (Packet _packet = new Packet((int)ClientPackets.ping))
        {
            _packet.Write(Client.instance.clientId); // should be client Id;

            SendUDPData(_packet);
        }        
    }

    public static void MapConnection()
    {
        using (Packet _packet = new Packet((int)ClientPackets.mapConnection))
        {
            _packet.Write(Client.instance.clientId); // should be client Id;

            SendUDPData(_packet);
        }
    }


    //Movement
    public static void Movement(Vector3 transformDirection, float moveSpeed, float vMovement, float deltaTime, float angle)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMoveInput))
        {
            _packet.Write(Client.instance.clientId);
            _packet.Write(transformDirection);
            _packet.Write(moveSpeed);
            _packet.Write(vMovement);
            _packet.Write(deltaTime);
            _packet.Write(angle);
            SendUDPData(_packet);
        }
    }
    #endregion
}
