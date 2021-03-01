using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Text;
using WebSocketSharp;
public class Client : MonoBehaviour
{
    public static Client instance;

    public string ip = "127.0.0.1";
    public int port = 47808;
    public int WSPort = 8471;
    public int localPort = 47807;
    public int clientId = 0;

    public UDP udp;

    public WSClient wsclient;
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
    void Start()
    {
        udp = new UDP();
        /*udp.Connect(0);
        udp.StartListener();*/

        wsclient = new WSClient();
        wsclient.Connect();
        //wsclient.StartListening();

        ServerPacketManager.instance.WakeUpManager();
        ServerSocketManager.instance.WakeUpManager();
    }


    private void Disconnect()
    {
        if (instance.udp.isConnected)
        {
            udp.Disconnect();            
        }
        if (instance.wsclient.isConnected)
        {
            wsclient.Disconnect();
        }
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }


    public class UDP
    {
        public UdpClient _socketClient;
        public IPEndPoint endPoint;

        public bool isConnected;
        public bool isListening;
        private int _listenPort;

        public delegate void PacketReceived(byte[] _packet);
        public event PacketReceived GotPackets;

        Thread m_ListeningThread;
        public void Connect()
        {            
            try
            {
                _socketClient = new UdpClient(instance.localPort);
                Debug.Log($"Connecting to UDP...:{instance.port}");
                _socketClient.Connect(instance.ip, instance.port);
                _listenPort = instance.localPort;
                isConnected = true;
            }
            catch (Exception _e)
            {
                Debug.Log(_e.ToString());
                instance.localPort += 1;
                Debug.Log($"Trying another port {instance.localPort}");
                Connect();
            }
        }
        public void StartListener()
        {
            // Register Packet Listener Here
            GotPackets += ServerPacketManager.instance.ProcessServerPacket;
            m_ListeningThread = new Thread(ListenForUDPPackets);
            isListening = true;
            m_ListeningThread.IsBackground = true;
            m_ListeningThread.Start();
        }

        public void StopListener()
        {
            isListening = false;
        }

        /*
         * UDP Listener for Data Packets then Pass Packet to Event Handler.
         */
        public void ListenForUDPPackets()
        {
            if (_socketClient != null)
            {
                endPoint = new IPEndPoint(IPAddress.Any, _listenPort);

                try
                {
                    Debug.Log($"Starting Listener for UDP Packets in :{_listenPort}");
                    while (this.isListening)
                    {
                        // TODO Implement UDP Packet Handling
                        byte[] bytes = _socketClient.Receive(ref endPoint);
                        GotPackets?.Invoke(bytes);                        
                        
                    }
                } catch (Exception _e)
                {
                    Debug.Log(_e.ToString());
                }

            }
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (_socketClient != null)
                {
                    _socketClient.Send(_packet.ToArray(), _packet.Length());
                }
            } catch (Exception _e)
            {
                Debug.Log(_e.ToString());
            }
        }

        public void Disconnect()
        {
            endPoint = null;
            if (isListening)
            {
                StopListener();
            }
            _socketClient.Close();
            isConnected = false;
        }
    }


    // WebSocket Connectivity
    public class WSClient
    {
        public WebSocket _socketClient;
        public bool isListening = false;
        public bool isConnected = false;
        
        public void Connect()
        {
            Debug.Log($"Connecting to : ws://{instance.ip}:{instance.WSPort}");
            _socketClient = new WebSocket($"ws://{instance.ip}:{instance.WSPort}");
            _socketClient.Connect();
            isConnected = true;            
            _socketClient.OnMessage += (sender, e) =>
            {
                ServerSocketManager.HandleSocketMessage((WebSocket)sender, e);
                //Debug.Log($"Message from {((WebSocket)sender).Url}, Data: {e.Data}");
            };            
        }
        public void SendData(string _json)
        {
            try
            {
                _socketClient.Send(_json);
            } catch(Exception _e)
            {
                Debug.Log(_e.Message);
            }
        }
        public void Disconnect()
        {
            _socketClient.Close();
            isConnected = false;            
        }
    }
    
    
}
