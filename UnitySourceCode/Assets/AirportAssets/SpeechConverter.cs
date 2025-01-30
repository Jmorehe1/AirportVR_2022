using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;
using System.Text.RegularExpressions;


public class SpeechConverter : MonoBehaviour
{
    //public Text responseText; 
    //public Text statusText;
    public DictationRecognizer dictationRecognizer;

    public string recognizedSpeech;
     void Start()
    {


        // foreach (string device in Microphone.devices)
        // {
        //     Debug.Log("Name: " + device);
        // }
    }

    void Awake(){
        if(GameObject.FindGameObjectsWithTag("SpeechConverter").Length > 1){
            Destroy(this.gameObject);
        }else{
            DontDestroyOnLoad(this.gameObject);
            dictationRecognizer = new DictationRecognizer();
            dictationRecognizer.DictationHypothesis += dictationRecognizer_DictationHypothesis;
            dictationRecognizer.DictationResult += dictationRecognizer_DictationResult;
            dictationRecognizer.DictationComplete += dictationRecognizer_DictationComplete;
            dictationRecognizer.Start();
            Debug.Log("STARTING DICTATION RECOGNIZER");
        }
    }
    // void OnDestroy() {
    //  Debug.Log("STOPPING DICTATION");
    //   dictationRecognizer.Stop();  
    // }
    private void dictationRecognizer_DictationComplete(DictationCompletionCause cause){
        dictationRecognizer.Stop();
        
        if(!muted){
            dictationRecognizer.Start();
        }
        
    }
    private void dictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        //responseText.text = text;
        if(recognizedSpeech != ""){
            recognizedSpeech += " ";
        }
        recognizedSpeech += text;
        Debug.Log(recognizedSpeech);
    }
    private void dictationRecognizer_DictationHypothesis(string text)
    {
        
    }

    bool muted = false;
    public void ToggleMute(){
        muted = !muted;
        //statusText.text = "Muted";
        if(muted){
            dictationRecognizer.Stop();
        }else{
            //statusText.text = "Listening";
            dictationRecognizer.Start();
        }
        
    }


    public string RequestCache(){
        string output = recognizedSpeech;
        recognizedSpeech = "";
        return output;
    }
}
