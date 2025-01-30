using System;
using System.Text;
using UnityEngine;
using NetMQ;
using NetMQ.Sockets;

public class ZmqClient : MonoBehaviour
{
    // Socket for communicating with the server
    private RequestSocket clientSocket;

    // Address of the server (replace with the actual address of your server)
    private string serverAddress = "tcp://192.168.116.123:8888";

    void Start()
    {
        // Initialize the ZeroMQ Request socket
        Debug.Log("Initializing ZeroMQ Client...");

        // Create a new Request socket for communication with the server
        clientSocket = new RequestSocket();

        // Connect the socket to the specified server address
        clientSocket.Connect(serverAddress);
        Debug.Log("Connected to the server.");
    }

    void Update()
    {
        // Check if the spacebar is pressed to trigger communication
        if (Input.GetKeyDown(KeyCode.Space)) // Spacebar triggers the message
        {
            // Message to be sent to the server
            string messageToSend = "Hello, server! Please work";
            Debug.Log("Sending: " + messageToSend);

            // Send the message to the server
            clientSocket.SendFrame(messageToSend);

            // Variable to store the server's response
            string response;

            // Attempt to receive the server's response with a timeout of 5 seconds
            if (clientSocket.TryReceiveFrameString(out response))
            {
                // Log the received response from the server
                Debug.Log("Received: " + response);
            }
            else
            {
                // Log an error if no response is received within the timeout period
                Debug.LogError("Failed to receive response from the server.");
            }
        }
    }

    private void OnDestroy()
    {
        // Dispose of the client socket when the object is destroyed
        // This ensures the connection is properly closed and resources are released
        clientSocket?.Dispose();
    }
}
