using System;
using System.IO;
using Renci.SshNet;
using UnityEngine;

public class SshClientExample : MonoBehaviour
{
    private string host = "arc.utsa.edu";
    private string username = "iav055";
    private string password = "VioletGT02004!?";

    void Start()
    {
        try
        {
            using (var client = new SshClient(host, username, password))
            {
                Debug.Log("Connecting to server...");
                client.Connect();
                Debug.Log("Connected to server.");

                // Run a command on the remote server
                var result = client.RunCommand("ls -l");
                Debug.Log("Command output: " + result.Result);

                // Download a file
                using (var sftp = new SftpClient(host, username, password))
                {
                    sftp.Connect();
                    Debug.Log("SFTP Connected.");

                    using (var ms = new MemoryStream())
                    {
                        sftp.DownloadFile("/home/sshnet/file123", ms);
                        Debug.Log("File downloaded successfully.");
                    }

                    sftp.Disconnect();
                }

                client.Disconnect();
                Debug.Log("Disconnected from server.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("SSH Error: " + ex.Message);
        }
    }
}
