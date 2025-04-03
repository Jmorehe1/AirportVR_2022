using UnityEngine;
using Renci.SshNet;
using System.Linq;
using System.Text;

public class SSHClient : MonoBehaviour
{
    private string host = "129.115.106.109";  // Arc server IP
    private string username = "iav055";       // Arc username
    private string password = "VioletGTO2004!?";  // Arc password (Hardcoded)
    private string twoFactorCode = "1";  // Replace with real 2FA code if required

    private string pythonHost = "129.115.106.109"; // Python server running on Arc
    private int pythonPort = 5005;

    void Start()
    {
        ConnectToSSH();
    }

    private void ConnectToSSH()
    {
        Debug.Log("Starting SSH connection...");

        // Create authentication method for password + 2FA
        var authMethod = new KeyboardInteractiveAuthenticationMethod(username);
        authMethod.AuthenticationPrompt += (sender, e) =>
        {
            foreach (var prompt in e.Prompts)
            {
                string request = prompt.Request;
                Debug.Log("SSH Prompt: " + request);

                if (request.Contains("Password"))
                {
                    Debug.Log("Sending password...");
                    prompt.Response = password;
                }
                else if (request.Contains("Duo") || request.Contains("Two-Factor"))
                {
                    Debug.Log("Sending two-factor authentication code...");
                    prompt.Response = twoFactorCode;
                }
            }
        };

        // Create SSH client with ASCII encoding
        var connectionInfo = new ConnectionInfo(host, username, authMethod);
        using (var client = new SshClient(connectionInfo))
        {
            try
            {
                client.Connect();
                Debug.Log("SSH Connected Successfully!");

                using (var shellStream = client.CreateShellStream("bash", 80, 24, 800, 600, 1024))
                {
                    string command = $"echo \"Hola!\" | nc {pythonHost} {pythonPort}\n";
                    byte[] commandBytes = Encoding.ASCII.GetBytes(command); // ASCII encoding
                    shellStream.Write(commandBytes, 0, commandBytes.Length);
                    shellStream.Flush();

                    byte[] readBuffer = new byte[1024];
                    int bytesRead = shellStream.Read(readBuffer, 0, readBuffer.Length);
                    string response = Encoding.ASCII.GetString(readBuffer, 0, bytesRead); // ASCII encoding

                    Debug.Log("AI Response: " + response);
                }

                client.Disconnect();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("SSH Connection Failed: " + ex.Message);
            }
        }
    }
}
