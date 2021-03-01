using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int frameRate = 144;

    public Transform localPlayer;

    public List<Transform> playerList;
    public float lastPong;
    private void Awake()
    {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 144;
        StartCoroutine(changeFramerate());
        InvokeRepeating(nameof(SendPing), 2f, 1f);
        lastPong = Time.deltaTime;        
    }

    public void SendPing()
    {
        if (Client.instance.udp.isConnected)
        {
            ClientSend.Ping();
        }
        
    }

    IEnumerator changeFramerate()
    {
        yield return new WaitForSeconds(2);
        Application.targetFrameRate = frameRate;
    }

    public void HandlePong(Packet _packet)
    {
        float pongDiff = Time.time - lastPong;        
        lastPong = Time.time;        
    }

    public void SetLocalPlayer(Transform player)
    {
        localPlayer = player;
    }

    public void AddPlayer(Transform player)
    {
        playerList.Add(player);
    }

    public void HandleSimPosition(Packet _packet) // handle simulated position;
    {
        int clientId = _packet.ReadInt();
        float x = _packet.ReadFloat();
        float y = _packet.ReadFloat();
        float z = _packet.ReadFloat();
        float vMovement = _packet.ReadFloat();
        float angle = _packet.ReadFloat();
        Vector3 _moveDirection = new Vector3(x, y, z);        
        if(Client.instance.clientId == clientId)
        {
            localPlayer.GetComponent<ActorController>().Move(_moveDirection, vMovement, angle);
        } else
        {
            Transform playerpool = GameObject.Find("PlayerPool").transform;
                        
            foreach(Transform actor in playerpool)
            {                
                ActorController actorctl = actor.GetComponent<ActorController>();                
                if (actorctl.playerId == clientId)
                {                    
                    actorctl.Move(_moveDirection, vMovement, angle);
                }
            }
        }
        
    }
}
