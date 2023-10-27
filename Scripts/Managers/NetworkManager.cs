using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

using UnityEngine;

public class NetworkManager : MonoBehaviour
{
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
        string host = "pirotess.duckdns.org";
        System.Net.IPAddress ip = GetIp(host);
        if (ip == null)
        {
            Debug.Log("Couldn't parse ip address");
            return false;
        }
        int port = 8044;
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
        while(true)
        {
            try
            {
            byte[] bytes = new byte[1024];
            int length;
            Thread.Sleep(0);
            if (connection == null)
            {
                return;
            }
            while ((length = connection.Read(bytes, 0, bytes.Length)) != 0)
            {
                var incomingdata = new byte[length];
                System.Array.Copy(bytes, 0, incomingdata, 0, length);
                string servermessage = System.Text.Encoding.UTF8.GetString(incomingdata);
                CheckMessages(servermessage);
            }
            }
            catch (System.Exception e)
            {
                
                Debug.Log("Catching exception" + e);
                return;
            }
        }
    }

     private void CheckMessages(string response)
    {
        Debug.Log("Got response "+response);
        lock(netlock)
        {
            if (response.Substring(0, 1) != "P" && response.Substring(0, 1) != "T")
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
            Debug.Log(response);
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
                        //We are player 1
                        Debug.Log("We are player 1");
                    }
                    else if (answer[2].Contains("B"))
                    {
                        //We are player 2
                        Debug.Log("We are player 2");
                    }
                    else
                    {
                        //We are a guest player
                        Debug.Log("We are a guest player");
                    }
                }
            }
            
            //Debug.Log(response);
        }

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
