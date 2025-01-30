using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using SpeechLib;

public class MainMenuHandler : MonoBehaviour
{
    public ScreenFader screenFader;

    public GameObject testMicButton;
    ServerCommunicator serverCommunicator;
    SpeechConverter speechConverter;
    SpVoice Attendant_voice;
    public Text testMicText;
    public Text micNameText;
    public string message;
    public bool testEnabled = false;
    //public Text responseText;
    public AudioClip myAudioClip; 

    public int rNumber = 0;
    public void Quit(){
        Application.Quit();
    }

    void Start(){
        //windows dictation recognizer
        speechConverter = GameObject.FindGameObjectWithTag("SpeechConverter").GetComponent<SpeechConverter>();
        //initializing and starting the com with the server
        serverCommunicator = new ServerCommunicator();
        serverCommunicator.Start();
        //initializing the TTS
        Attendant_voice = new SpVoice();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.S) && testEnabled==true)
        {
            Debug.Log("I am in S");
            //removing these two functions below and adding a send message here should allow it to work with a server
           EndRecording();
           StartRecording();
        }
    }

    public void BeginDialogue(){
        //speechConverter.dictationRecognizer.Stop();
        StartCoroutine(BeginDialogueRoutine());
        IEnumerator BeginDialogueRoutine(){
            screenFader.FadeOut();
            yield return new WaitForSeconds(screenFader.fadeTime);
            SceneManager.LoadScene("DialogueScene");
        }
    }
    
    public void TestMic(){
        
        
        if(Microphone.devices.Length > 0){
            micNameText.text = Microphone.devices[0];
            testMicButton.SetActive(false);
            testMicText.text = "Recorded speech will be printed here.";
            testEnabled = true;
            StartCoroutine(TestMicRoutine());
            
        }else{
            micNameText.text = "No microphone detected.";
        }
        IEnumerator TestMicRoutine(){
            yield return null;
            StartRecording();
            while(true){
                string voiceRecording = speechConverter.RequestCache();
                //Debug.Log(voiceRecording);
                if(voiceRecording != "" && voiceRecording != null){
                    testMicText.text = voiceRecording;
                    message = voiceRecording;
                }
                yield return new WaitForSeconds(.5f);
            }
            
        }
        
    }

    public void StartRecording(){
        //3rd parameter herer is length of the recording in seconds
        myAudioClip = Microphone.Start ( null, false, 30, 44100 );
    }

    public void EndRecording(){
        SavWav.Save("myfile" + rNumber, myAudioClip);
        rNumber++;
    }


    IEnumerator WaitForScore(){
        yield return new WaitUntil(()=>serverCommunicator.GetLastResponse() != "");
        Debug.Log("RESPONSE: " + serverCommunicator.GetLastResponse());
        Attendant_voice.Speak(serverCommunicator.GetLastResponse(),
            SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        serverCommunicator.AcknowledgeLastResponse();
    }
}
