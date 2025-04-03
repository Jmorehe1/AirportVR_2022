using UnityEngine;
using Renci.SshNet;
using System.Text;
using System.Threading;

public class SSHClient_gpu : MonoBehaviour
{
    private string host = "129.115.106.109";  // Arc server IP
    private string username = "iav055";       // Arc username
    private string password = "VioletGTO2004!?";  // Arc password (Hardcoded)
    private string twoFactorCode = "1";  // Replace with real 2FA code if required
    private string pythonHost = "129.115.106.109"; // Python server running on Arc
    private int pythonPort = 5005;

    private SshClient client;
    private ShellStream shellStream;

    void Start()
    {
        ConnectToSSH();
    }

    private void ConnectToSSH()
    {
        Debug.Log("Starting SSH connection...");

        var authMethod = new KeyboardInteractiveAuthenticationMethod(username);
        authMethod.AuthenticationPrompt += (sender, e) =>
        {
            foreach (var prompt in e.Prompts)
            {
                string request = prompt.Request;
                Debug.Log("SSH Prompt: " + request);

                if (request.Contains("Password"))
                {
                    prompt.Response = password;
                }
                else if (request.Contains("Duo") || request.Contains("Two-Factor"))
                {
                    prompt.Response = twoFactorCode;
                }
            }
        };

        var connectionInfo = new ConnectionInfo(host, username, authMethod);
        client = new SshClient(connectionInfo);

        try
        {
            client.Connect();
            Debug.Log("SSH Connected Successfully!");

            shellStream = client.CreateShellStream("bash", 80, 24, 800, 600, 1024);

            // Start GPU session and activate Conda environment
            ExecuteCommand("srun -p gpu1v100 -n 1 -t 01:30:00 --pty bash", 5000);
            ExecuteCommand("source ~/miniconda3/bin/activate && conda activate LLAMA", 2000);

            // Send message to AI server
            string command = $"echo \"Hola!\" | nc {pythonHost} {pythonPort}";
            string response = ExecuteCommand(command, 2000);

            Debug.Log("Server Response: " + response);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("SSH Connection Failed: " + ex.Message);
        }
    }

    private string ExecuteCommand(string command, int delay = 1000)
    {
        Debug.Log($"Executing Command: {command}");
        shellStream.WriteLine(command);
        shellStream.Flush();

        Thread.Sleep(delay); // Allow time for command execution

        StringBuilder responseBuilder = new StringBuilder();
        while (shellStream.DataAvailable)
        {
            byte[] readBuffer = new byte[2048];
            int bytesRead = shellStream.Read(readBuffer, 0, readBuffer.Length);
            if (bytesRead > 0)
            {
                string response = Encoding.ASCII.GetString(readBuffer, 0, bytesRead);
                responseBuilder.Append(response);
            }
        }

        string finalResponse = responseBuilder.ToString();
        Debug.Log($"Command Response: {finalResponse}");
        return finalResponse;
    }

    void OnApplicationQuit()
    {
        if (client != null && client.IsConnected)
        {
            client.Disconnect();
            Debug.Log("SSH Disconnected.");
        }
    }
}
