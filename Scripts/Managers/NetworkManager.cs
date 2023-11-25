using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public GameManager gameManager;
    TcpClient client;
    public GameObject chat;
    public string ingamechat;
    public NetworkStream connection;
    Thread ClientReceiveThread;
    public object netlock = new object();
    string buffer;
    public string authhash;
    public string playername;
    bool ping = false;
    bool connected = false;
    public bool chatactive = false;
     private void Start() 
     {
        playername = gameManager.getPlayername();
        ingamechat = "";
        if (Connect())
        {
            connected = true;
            ClientReceiveThread = new Thread(new ThreadStart(ListenForData));
            ClientReceiveThread.IsBackground = true;
            ClientReceiveThread.Start();
        }
    }    
    private void Update()
    {
        if (Input.GetButtonUp("Submit"))
        {
            chat.SetActive(true);
            chatactive = true;
        }
    }
    public bool Connect()
    {
        try
        {
            if (client != null && client.Connected)
            {
                return true;
            }
        }
        catch (System.Exception e){
            Debug.Log("M");
            Debug.LogError(e.ToString());
            Debug.LogError(e.StackTrace);
        }

        client = new TcpClient();
        client.ReceiveBufferSize = ushort.MaxValue;
        string host = gameManager.getserverhost();
        System.Net.IPAddress ip = GetIp(host);
        if (ip == null)
        {
            Debug.Log("Couldn't parse ip address");
            return false;
        }
        int port = Int32.Parse(gameManager.getServerPort());
        try
        {
            client.Connect(ip, port);
        }
        catch (System.Exception e)
        {
            Debug.Log("Connection couldn't be established"+e);
            client.Dispose();
            return false;
        }
        connection = client.GetStream();
        ClientReceiveThread = new Thread(new ThreadStart(ListenForData));
        ClientReceiveThread.IsBackground = true;
        ClientReceiveThread.Start();
        try
        {
            InitConnection();
        }
        catch (System.Exception e)
        {
            Debug.Log("Couldn't send our name/init sequence"+e);
        }
        return true;
    }

    public static System.Net.IPAddress GetIp(string hostname)
    {
        System.Net.IPAddress outip;
        if (System.Net.IPAddress.TryParse(hostname, out outip) == true)
        {
            return outip;
        }
        outip = System.Net.Dns.GetHostAddresses(hostname)[0].MapToIPv4();
        Debug.Log(outip);
        return outip;

    }

    void InitConnection()
    {
        string hello = "H:"+playername+":";
        SendMsg(hello);
    }

    public void Disconnect()
    {
        string send = "Q:\n";
        SendMsg(send);
    }
    public bool SendMsg(string msg)
    {
        if (connection == null || connected == false)
        {
            //Let's try to open a connection because apparently somebody is trying to send a message
            if (!Connect())
            {
                Debug.Log("Couldn't send message because connection couldn't be established");
                return false;
            }
            else
            {
                Debug.Log("Somehow we got here");
                connected = true;
            }
        }
        Debug.Log("Trying to send:"+msg);
        if(connection.CanWrite)
        {
            byte[] bs  = System.Text.Encoding.UTF8.GetBytes(msg);
            byte[] bs2 = new byte[bs.Length+1];
            byte nt = (byte)'\0';
            System.Array.Copy(bs, 0, bs2, 0, bs.Length);
            bs2[bs.Length] = nt;       
            connection.Write(bs2, 0, bs2.Length);
            return true;
        }
        else
        {
            Debug.Log("Can't write to stream");
            return false;
        }
    }
    
   private void ListenForData()
{
    List<byte> bytesList = new List<byte>();
    byte[] buffer = new byte[1024]; // Adjust the buffer size as needed
    StringBuilder currentMessage = new StringBuilder();

    while (true)
    {
        int bytesRead = connection.Read(buffer, 0, buffer.Length);

        for (int i = 0; i < bytesRead; i++)
        {
            bytesList.Add(buffer[i]);
            currentMessage.Append((char)buffer[i]);

            // Check if the terminator is present in the received data
            if (buffer[i] == '|')
            {
                byte[] receivedBytes = bytesList.ToArray();
                string receivedMessage = Encoding.ASCII.GetString(receivedBytes);
                bytesList.Clear();
                CheckMessages(receivedMessage);
                currentMessage.Clear();
            }
        }
    }
}

   
     private void CheckMessages(string response)
    {
        lock(netlock)
        {
            if (response.Substring(0, 1) != "P" && response.Substring(0, 1) != "T" && response.Substring(0, 1) != "H" && response.Substring(0, 1) != "M")
            {
                buffer += response;   
            }
            else
            {
                buffer = response;
            }  
            response = buffer;      
            if (response[response.Length-1] != '|')
            {
                //Debug.Log(response);
                return;
            }
            int marker = response.IndexOf("|");
            response = response.Substring(0, marker-1);

            string[] answer = response.Split(':');

            if (answer[0].Contains("E"))
            {
                Debug.Log("Server Error:"+answer[1]);
                //Some error
            }
            else if (answer[0].Contains("P"))
            {
                if(ping)
                {
                    //We apparently sent a ping and now got a pong
                    ping = false;
                }
                else
                {
                    //Something is wrong. We got a pong but never sent a ping? Weird.
                }
            }
            else if (answer[0].Contains("T"))
            {
                answer[2] = answer[2].Replace(";;;pipesymbol;;;", "|").Replace(";;;colon;;;", ":");
                if (playername != null && answer[1] != null && answer[1].Contains(playername) && answer[1].Length == playername.Length)
                {
                    //This is a chat message by ourselves - let's handle it here because that means everything worked
                    try
                    {
                        ingamechat += "<color=\"orange\">"+answer[1]+": "+answer[2]+"</color>\n";
                    }
                    catch(System.Exception e)
                    {
                        Debug.Log(e);
                    }
                }
                else
                {
                    ingamechat += "<color=\"blue\">"+answer[1]+": "+answer[2]+"</color>\n";
                    //This is a chat message by someone else
                }
            }
            else if (answer[0].Contains("H"))
            {
                //We receive a hello message - let's see who this is about and what role they play
                answer[2] = answer[2].Replace(";;;pipesymbol;;;", "|").Replace(";;;colon;;;", ":");
                if (playername != null && answer[1] != null && answer[1].Contains(playername) && answer[1].Length == playername.Length)
                {
                    //This is a status message about ourselves - let's find out if we are player 1, 2 or guest
                    if (answer[2].Contains("A"))
                    {
                        if (gameManager.player != null && gameManager.p2 != null)
                            gameManager.player = gameManager.p2;
                        //We are player 1
                        Debug.Log("We are player 2");
                    }
                    else if (answer[2].Contains("B"))
                    {
                        if (gameManager.player != null && gameManager.p1 != null)
                            gameManager.player = gameManager.p1;
                        //We are player 2
                        Debug.Log("We are player 1");
                    }
                    else
                    {
                        //We are a guest player
                        Debug.Log("We are a guest player");
                    }
                }
            }
            else if (answer[0].Contains("M"))
            {
                if (!answer[1].Contains(playername))
                {
                    string[] coords = answer[2].Split(',');
                    Vector3 moveto = ConvertToVector3(coords[0], coords[1], coords[2]);
                    Debug.Log(moveto.ToString());
                    Debug.Log("Moving Unit"+answer[3]+" to x:"+moveto.x+" y:"+moveto.y+" z:"+moveto.z);

                }
            }
            
            //Debug.Log(response);
        }

    }
    Vector3 ConvertToVector3(string xStr, string yStr, string zStr)
    {
        Debug.Log("raw:"+xStr+yStr+zStr);
        // Parse strings to floats
        float x = float.Parse(xStr);
        float y = float.Parse(yStr);
        float z = float.Parse(zStr);

        // Create Vector3
        Vector3 vector = new Vector3(x, y, z);

        return vector;
    }
    void OnApplicationQuit()
    {
        try
        {
            if (ClientReceiveThread.IsAlive)
                ClientReceiveThread.Abort();
        }
        catch(System.Exception e)
        {
           Debug.Log("some error?"+e);
        }
    }
   
}
