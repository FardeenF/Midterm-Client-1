using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class Client : MonoBehaviour
{
    static Socket client;
    static byte[] buffer = new byte[512];
    static string message = "";
    public InputField inputMessageClone;
    public InputField serverIPClone;

    public static InputField inputMessage;
    public static InputField serverIP;

    public static GameObject canvasItems;
    public static GameObject levelItems;
    public static GameObject loginItems;

    public GameObject canvasItemsClone;
    public GameObject levelItemsClone;
    public GameObject loginItemsClone;

    public static Text displayedChat;

    public Text displayedChatClone;

    public static string chat = "";

    public static bool connectionStarted = false;

    //UDP Stuff
    public GameObject cube1Clone;
    public GameObject cube2Clone;

    public static GameObject cube1;
    public static GameObject cube2;

    private static IPEndPoint remoteEP;
    private static Socket udpClient;

    //UDP Receive
    static byte[] udpBuffer = new byte[512];
    static int rec = 0;
    private static float[] pos;
    private static byte[] bpos;

    private static float[] pos2;
    private static byte[] bpos2;
    private static IPEndPoint udpRemote;
    private static EndPoint remoteServer;
    //private static Socket udpClientReceive;

    //Timer
    static float timer = 0.0f;
    static float timer2 = 0.0f;
    static float sendInterval = 5.0f;

    public static void StartUDPConnection()
    {
        IPAddress ip = IPAddress.Parse(serverIP.text);
        remoteEP = new IPEndPoint(ip, 11112); //11112

        udpClient = new Socket(ip.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        //udpClientReceive = new Socket(ip.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

        //UDP
        udpRemote = new IPEndPoint(IPAddress.Any, 11114); //11114
        remoteServer = (EndPoint)udpRemote;

        udpClient.Bind(udpRemote);
        udpClient.Connect(remoteEP);
        //udpClientReceive.Bind(remoteEP);
    }

    public static void StartConnection()
    {

        //loginItems.SetActive(false);
        //canvasItems.SetActive(true);
        //levelItems.SetActive(true);

        //Setup our end point (server)
        try
        {
            //IPAddress ip = IPAddress.Parse(serverIP.text);
            IPAddress ip = IPAddress.Parse("127.0.0.1");

            IPEndPoint server = new IPEndPoint(ip, 11111);

            //Create our client socket
            client = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //client.Blocking = false;

            // Attempt a connection
            try
            {
                Debug.Log("Connecting to server...");
                client.Connect(server);
                connectionStarted = true;

                Debug.Log("Connected to IP: " + client.RemoteEndPoint.ToString());


                loginItems.SetActive(false);
                canvasItems.SetActive(true);
                levelItems.SetActive(true);

                message = "Player 1 has joined!";
                byte[] msg = Encoding.ASCII.GetBytes(message);

                try
                {
                    client.Send(msg);

                }
                catch (SocketException se)
                {
                    if (se.SocketErrorCode != SocketError.WouldBlock)
                    {
                        Debug.Log(se.ToString());
                    }
                }

                displayedChat.text = chat;

            }
            catch (ArgumentNullException excep)
            {
                Console.WriteLine("ArgumentNullException: {0}", excep.ToString());
            }
            catch (SocketException se)
            {
                Debug.Log("BUG!");
                if (se.SocketErrorCode != SocketError.WouldBlock)
                {
                    Debug.Log(se.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("UnexpectedException: {0}", e.ToString());
            }
        }
        catch (SocketException se)
        {
            if (se.SocketErrorCode != SocketError.WouldBlock)
            {
                Debug.Log(se.ToString());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("UnexpectedException: {0}", e.ToString());
        }

        //client.Shutdown(SocketShutdown.Both);
        //client.Close();
    }

    public static void RunUDPClient()
    {
        pos = new float[] { cube1.transform.position.x, cube1.transform.position.y, cube1.transform.position.z };

        Buffer.BlockCopy(pos, 0, bpos, 0, bpos.Length);

        //UDP Server
        timer += Time.deltaTime;
        if (timer > 0.0001f)
        {
            try
            {
                udpClient.SendTo(bpos, remoteEP);

                //udpClient.SendTo(bpos, remoteEP);
                //Debug.Log("SENDING!");
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode != SocketError.WouldBlock)
                {
                    Console.WriteLine("Exception: " + e.ToString());
                }
            }

            timer = 0.0f;
        }

        //udpClient.SendTo(bpos, remoteEP);

        //UDP Server
        //timer2 += Time.deltaTime;
        //if (timer2 > 0.5f)
        //{
            try
            {
                //Debug.Log("Prologue.");
                rec = udpClient.ReceiveFrom(udpBuffer, ref remoteServer);
                //Debug.Log("RECEIVING!");
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode != SocketError.WouldBlock)
                {
                    Debug.Log("Exception: " + e.ToString());
                }
            }
            timer2 = 0.0f;
        //}


        pos2 = new float[rec / 4];
        Buffer.BlockCopy(udpBuffer, 0, pos2, 0, rec);

        //Vector3 testVec = new Vector3(pos2[0], pos2[1], pos2[2]);

        //Debug.Log("My Position - X: " + cube1.transform.position.x + ", Y: " + cube1.transform.position.y + ", Z: " + cube1.transform.position.z);
        //Debug.Log("Received Position - X: " + testVec[0] + ", Y: " + testVec[1] + ", Z: " + testVec[2]);

        try
        {
            cube2.transform.position = new Vector3(pos2[0], pos2[1], pos2[2]);
        }
        catch(Exception e)
        {

        }
    }

    public static void RunClient()
    {
        //Receive data from server
        try
        {
            int recv = client.Receive(buffer);

            if (Encoding.ASCII.GetString(buffer, 0, recv) != "" && Encoding.ASCII.GetString(buffer, 0, recv) != chat)
            {
                chat = Encoding.ASCII.GetString(buffer, 0, recv); //+= "\n" + message;
            }
            displayedChat.text = chat;
        }
        catch(SocketException se)
        {
            if (se.SocketErrorCode != SocketError.WouldBlock)
            {
                Debug.Log(se.ToString());
            }
        }
        

        //Console.WriteLine("Recieved: {0}", Encoding.ASCII.GetString(buffer, 0, recv));
        //Send data to server
        //Debug.Log("Send message: ");
        if (Input.GetKeyDown(KeyCode.Return) && (inputMessage.text == "Quit" || inputMessage.text == "quit"))
        {
            message = "Player 1: " + inputMessage.text;
            //chat += "\n" + message;
            byte[] msg = Encoding.ASCII.GetBytes(message);

            float timer2 = 0.0f;
            float limit = 100.0f;

            try
            {
                //Debug.Log("Checkpoint 1.");
                client.Send(msg);
                inputMessage.text = "";

                do
                {
                    timer2 += Time.deltaTime;


                } while (timer2 < limit);

                try
                {
                    int recv = client.Receive(buffer);

                    chat = Encoding.ASCII.GetString(buffer, 0, recv); //+= "\n" + message;

                    displayedChat.text = chat;
                }
                catch (SocketException se)
                {
                    if (se.SocketErrorCode != SocketError.WouldBlock)
                    {
                        Debug.Log(se.ToString());
                    }
                }

                client.Shutdown(SocketShutdown.Both);
                client.Close();
                udpClient.Shutdown(SocketShutdown.Both);
                udpClient.Close();
                connectionStarted = false;

                //Debug.Log("REACHED!");

                loginItems.SetActive(true);
                canvasItems.SetActive(false);
                levelItems.SetActive(false);
            }
            catch(SocketException se)
            {
                if (se.SocketErrorCode != SocketError.WouldBlock)
                {
                    Debug.Log(se.ToString());
                }
            }
        }
        else if(Input.GetKeyDown(KeyCode.Return) && inputMessage.text != "Quit")
        {
            message = "Player 1: " + inputMessage.text;
            //chat += "\n" + message;
            byte[] msg = Encoding.ASCII.GetBytes(message);

            try
            {
                client.Send(msg);
                inputMessage.text = "";

            }
            catch (SocketException se)
            {
                if (se.SocketErrorCode != SocketError.WouldBlock)
                {
                    Debug.Log(se.ToString());
                }
            }
        }
        

    }

    public static void QuitClient()
    {
        if (Input.GetKeyDown(KeyCode.Return) && inputMessage.text == "Quit")
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();

            loginItems.SetActive(true);
            canvasItems.SetActive(false);
            levelItems.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        loginItems = loginItemsClone;
        canvasItems = canvasItemsClone;
        levelItems = levelItemsClone;

        inputMessage = inputMessageClone;
        serverIP = serverIPClone;

        serverIP.text = "127.0.0.1";

        displayedChat = displayedChatClone;

        cube1 = cube1Clone;
        cube2 = cube2Clone;

        pos = new float[] { cube1.transform.position.x, cube1.transform.position.y, cube1.transform.position.z };
        bpos = new byte[pos.Length * 4];

        loginItems.SetActive(true);
        canvasItems.SetActive(false);
        levelItems.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!loginItems.activeSelf)
        {
            client.Blocking = false;
            udpClient.Blocking = false;
            //udpClientReceive.Blocking = false;
        }
        

        if(connectionStarted)
        {
            //Debug.Log("Reached Update Function!");
            RunClient();
            if(udpClient.Connected)
                RunUDPClient();
            //QuitClient();

            //displayedChat.text = chat;
        }

        if(inputMessage.isFocused)
        {
            cube1.GetComponent<cube>().enabled = false;
        }
        else
        {
            cube1.GetComponent<cube>().enabled = true;
        }
    }
}
