using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager instance;

    public Transform playerPrefab;
    public Transform playerCamerPrefab;
    public Transform NameTagPrefab;

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

    public void SpawnPlayer(ServerSocketManager.HandleOpenUDPRequestPayload payload)
    {
        Debug.Log("Spawning Player");
        Transform pool = GameObject.Find("PlayerPool").GetComponent<Transform>();
        Transform newPlayer = Instantiate(playerPrefab, pool);
        Transform nametag = Instantiate(NameTagPrefab);
        newPlayer.transform.parent = pool;
        nametag.transform.SetParent(GameObject.Find("NameTagPool").GetComponent<Transform>());
        nametag.gameObject.GetComponent<Text>().text = payload.username;

        Transform camTarget = newPlayer.GetChild(0);
        Transform vcam = Instantiate(playerCamerPrefab);
        CinemachineFreeLook cam = vcam.GetComponent<CinemachineFreeLook>();
        cam.m_Follow = camTarget;
        cam.m_LookAt = camTarget;

        newPlayer.GetComponent<ThirdPersonCameraControl>().freeLookCam = cam;
        ActorController actor = newPlayer.GetComponentInChildren<ActorController>();
        actor.controllable = true;
        actor.playerName = payload.username;
        actor.playerId = payload.playerId;
        nametag.GetComponent<ClampNameTag>().toFollow = newPlayer.GetChild(1);
        GameManager.instance.SetLocalPlayer(newPlayer);
        ServerSocketManager.instance.RequestListPlayer();
    }
        
    internal void SpawnOtherPlayer(ServerSocketManager.PlayerListInfo player)
    {
        if(Client.instance.clientId != player.playerId) { 
            Transform pool = GameObject.Find("PlayerPool").GetComponent<Transform>();
            Transform newPlayer = Instantiate(playerPrefab);
            Transform nametag = Instantiate(NameTagPrefab);
            ServerSocketManager.PlayerInfoPosition position = player.position;
            Vector3 newPos = new Vector3(position.x, position.y + 2f, position.z);
            newPlayer.transform.SetParent(pool);
            newPlayer.transform.position = newPos;
            nametag.transform.SetParent(GameObject.Find("NameTagPool").GetComponent<Transform>());
            nametag.gameObject.GetComponent<Text>().text = player.username;
            ActorController actor = newPlayer.GetComponentInChildren<ActorController>();
            actor.playerName = player.username;
            actor.playerId = player.playerId;
            nametag.GetComponent<ClampNameTag>().toFollow = newPlayer.GetChild(1);
        }
    }
}
