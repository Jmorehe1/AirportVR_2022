using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;

/// <summary>

public class ServerCommunicator : RunAbleThread
{

    protected override void Run()
    {
        ForceDotNet.Force(); // Prevents Unity from Freezing
        using (RequestSocket client = new RequestSocket())
        {
            client.Connect("tcp://localhost:5555");
        
            while (Running){
                while(messageLock){}
                messageLock = true;
                Debug.Log("Sending " + lastMessage);
                client.SendFrame(lastMessage);

                string response = null;
                bool gotResponse = false;
                while (Running)
                {
                    gotResponse = client.TryReceiveFrameString(out response); //returns true on successful message delivery
                    if (gotResponse) break;
                }

                if (gotResponse) Debug.Log("Received " + response);
                lastResponse = response;
            }
        }

        NetMQConfig.Cleanup(); // prevent Unity freeze
    }
    bool messageLock = true;
    string lastMessage = "";
    string lastResponse = "";
    public void SendMessage(string message){
        lastMessage = message;
        messageLock = false;
    }
    public string GetLastResponse(){
        return lastResponse;
    }
    public void AcknowledgeLastResponse(){
        lastResponse = "";
    }
}