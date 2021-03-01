using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using WebSocketSharp;
using Newtonsoft.Json;
using System;

public class ServerSocketManager : MonoBehaviour
{
    public static ServerSocketManager instance;
    private delegate void SocketHandler(string data);
    private static Dictionary<string, SocketHandler> socketsHandler;

    private GameObject loginWindow;
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
    public class EventJSON
    {
        public string type;
        public string payload;
    }

    public void WakeUpManager()
    {
        loginWindow = GameObject.Find("UILogin");        
        socketsHandler = new Dictionary<string, SocketHandler>()
        {
            { "ping", HandlePong},
            {"join_handshake",  HandleHandshake},
            {"req_open_udp_conn", HandleOpenUDPRequest },
            {"got_player_list", HandleGotPlayerList},
            {"on_player_connected", HandleOnPlayerConnected }
        };
    }

    public static void HandleSocketMessage(WebSocket sender, MessageEventArgs e)
    {
        EventJSON eventJSON = JsonConvert.DeserializeObject<EventJSON>(e.Data);        
        socketsHandler[eventJSON.type](eventJSON.payload);
    }


    #region Sender
    void HandlePong(string data)
    {
        EventJSON pongEvent = new EventJSON();
        pongEvent.type = "pong";
        pongEvent.payload = "";
        string ep = JsonConvert.SerializeObject(pongEvent);        
        Client.instance.wsclient.SendData(ep);
    }
    struct RequestJoinPayload
    {
        public string username;
    }
    public void RequestJoin(string username)
    {

        EventJSON reqJoinEvent = new EventJSON
        {
            type = "req_join_game"
        };
        RequestJoinPayload payload = new RequestJoinPayload
        {
            username = username
        };
        reqJoinEvent.payload = JsonConvert.SerializeObject(payload);
        string ep = JsonConvert.SerializeObject(reqJoinEvent);
        Client.instance.wsclient.SendData(ep);
    }

    struct RequestListPlayerPayload
    {
        public int playerId;
    }
    public void RequestListPlayer()
    {
        EventJSON reqListPlayer = new EventJSON
        {
            type = "get_player_list",            
        };
        RequestListPlayerPayload payload = new RequestListPlayerPayload
        {
            playerId = Client.instance.clientId
        };
        reqListPlayer.payload = JsonConvert.SerializeObject(payload);
        string ep = JsonConvert.SerializeObject(reqListPlayer);
        Client.instance.wsclient.SendData(ep);
    }

    #endregion

    #region Handler
    public struct HandleOpenUDPRequestPayload
    {
        public string username;
        public int playerId;        
    }
    public void HandleOpenUDPRequest(string data)
    {
        var payload = JsonConvert.DeserializeObject<HandleOpenUDPRequestPayload>(data);
        Client.instance.clientId = payload.playerId;
        Client.instance.udp.Connect();
        Client.instance.udp.StartListener();
        ThreadManager.ExecuteOnMainThread(() =>
        {
            loginWindow.GetComponent<UILoginWindow>().SetInfoMsg("Connected!");
            loginWindow.GetComponent<UILoginWindow>().Hide();
            PlayerSpawnManager.instance.SpawnPlayer(payload);
            ClientSend.MapConnection();
        });
    }

    public struct PlayerInfoPosition
    {
        public float x;
        public float y;
        public float z;
    }
    public struct PlayerListInfo
    {
        public int playerId;
        public string username;
        public PlayerInfoPosition position;
    }    
    void HandleGotPlayerList(string data)
    {  
        try
        {
            List<PlayerListInfo> payload = JsonConvert.DeserializeObject<List<PlayerListInfo>>(data);            
            ThreadManager.ExecuteOnMainThread(() =>
            {
                foreach (var player in payload)
                {
                    PlayerSpawnManager.instance.SpawnOtherPlayer(player);
                }                
            });
        } catch (Exception _e)  
        {
            Debug.Log(_e.ToString());
        }
        
    }

    void HandleOnPlayerConnected(string data)
    {
        try
        {
            PlayerListInfo payload = JsonConvert.DeserializeObject<PlayerListInfo>(data);
            if(payload.playerId != Client.instance.clientId)
            {
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    Debug.Log("Respawning New Connected Player");
                    PlayerSpawnManager.instance.SpawnOtherPlayer(payload);
                });
            }            
        } catch (Exception _e)
        {
            Debug.Log(_e.ToString());
        }
    }

    void HandleHandshake(string data)
    {
        GameObject.Find("UILogin").GetComponent<UILoginWindow>().SetInfoMsg("Error... Server is full");
    }

    

    #endregion
}
