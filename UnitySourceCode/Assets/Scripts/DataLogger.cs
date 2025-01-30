using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataLogger : MonoBehaviour
{
    [Header("Config")]
    public string testName = "test.txt";
    public float timeStep = .1f;

    [Header("Track Selection")]
    public bool trackHeadOrientation = false;
    public bool trackBodyPosition = false;
    public bool trackFramerate = false;


    [Header("HelperObjects")]
    public GameObject body;
    public GameObject head;
    float averageFrameRate = 0;
    
    void Start(){
        StartCoroutine(TrackDataRoutine());
    }

    float averageFramerate;
    public void Update()
    {
        averageFramerate = Time.frameCount / Time.time;
    }

    public IEnumerator TrackDataRoutine(){
        while(true){

            yield return new WaitForSeconds(timeStep);
            if(trackBodyPosition){
                WriteData(body.transform.position.ToString(), testName + "_body_position.csv");
            }

            if(trackHeadOrientation){
                WriteData(head.transform.localEulerAngles.ToString(), testName + "_head_orientation.csv");
            }

            if(trackFramerate){
                 WriteData(averageFramerate.ToString("F2"), testName + "_average_framerate.csv");
            }
            
            
        }
    }
    string path = "C:\\Users\\samue\\OneDrive\\Desktop\\TestDataStoreLocation/";
    public void WriteData(string data, string fileName){
        string timeStamp = System.DateTime.Now.ToString();
        StreamWriter writer = new StreamWriter(path+fileName, true);
        writer.Write(timeStamp+", " + data + "\n");
        writer.Close();
    }

}
