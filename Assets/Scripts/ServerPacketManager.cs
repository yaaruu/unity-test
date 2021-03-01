using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerPacketManager : MonoBehaviour
{
    public static ServerPacketManager instance;
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }      
    public void ProcessServerPacket(byte[] _data)
    {
        ThreadManager.ExecuteOnMainThread(() =>
        {            
            using (Packet _packet = new Packet(_data))
            {
                int _packetLength = _packet.ReadInt();
                int _packetId = _packet.ReadInt();
                //Debug.Log($"Got Packet {_packetId} length: {_packetLength}");
                packetHandlers[_packetId](_packet);
            }
        });
        
    }
    
    public void WakeUpManager()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int) ServerPackets.pong, GameManager.instance.HandlePong },
            { (int) ServerPackets.simPosition,  GameManager.instance.HandleSimPosition},
            { (int) ServerPackets.predictPosition, GotPredictPosition  }
        };
    }

    public void GotPredictPosition(Packet _packet)
    {
        float clientId = _packet.ReadInt();
        float posX = _packet.ReadFloat();
        float posY = _packet.ReadFloat();
        float posZ = _packet.ReadFloat();
    }

    
}
