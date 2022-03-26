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

    public static string chat = "--START OF CHAT--";

    public static bool connectionStarted = false;

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

    public static void RunClient()
    {
        //Receive data from server
        try
        {
            int recv = client.Receive(buffer);

            if (Encoding.ASCII.GetString(buffer, 0, recv) != "--START OF CHAT--" && Encoding.ASCII.GetString(buffer, 0, recv) != chat)
            {
                chat += "\n" + message;
            }
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
        Debug.Log("Send message: ");
        if (Input.GetKeyDown(KeyCode.Return) && inputMessage.text != "Quit")
        {
            message = inputMessage.text;
            chat += "\n" + message;
            byte[] msg = Encoding.ASCII.GetBytes(chat);

            try
            {
                client.Send(msg);
            }
            catch(SocketException se)
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
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //canvasItems = GameObject.Find("Canvas Level Group");
        //levelItems = GameObject.Find("Level Group");
        //loginItems = GameObject.Find("Login Group");

        //inputMessage = GameObject.Find("Chat Input Field").GetComponent<InputField>();
        //serverIP = GameObject.Find("Server Input Field").GetComponent<InputField>();

        loginItems = loginItemsClone;
        canvasItems = canvasItemsClone;
        levelItems = levelItemsClone;

        inputMessage = inputMessageClone;
        serverIP = serverIPClone;

        displayedChat = displayedChatClone;

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
        }

        if(connectionStarted)
        {
            Debug.Log("Reached Update Function!");
            RunClient();
            QuitClient();

            displayedChat.text = chat;
        }
    }
}
