using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using SpeechLib;

public class ConversationManager : MonoBehaviour
{
    
    [Header("Config")]

    public float triggerDelay = 1f; //ensures the dictation recognizer will be done in time

    [Header("Data")]
    public List<AudioClip> audioClips; //clips that require responses
    public AudioClip finalClip; //clip to end experience

    [Header("Objects")]
    public AudioSource audioSource;
    public ResponsePanel responsePanel;
    public Transform passport;

    

    [Header("Controllers")]
    public List<XRController> controllers;


    //Trackers===============
    bool triggerIsDown = false;
    int voiceLineTracker = -1; 

    //Misc=======================
    ServerCommunicator serverCommunicator;
    SpeechConverter speechConverter;
    SpVoice Attendant_voice;

    public List<string> questions;
    int questionIndex = 0;
    public bool beganConvo = false;
    public List<string> transcript;
    int t_index = 0;
    string messageToSend;
    string response;
    bool repetitive = true;
    bool Server_flag = false;
    public List<string> scores;
    public List<string> prompts;
    public int promptIndex = 0;


    void Awake(){
       
        passport.localScale = Vector3.zero;
    }

    void Start(){
        speechConverter = GameObject.FindGameObjectWithTag("SpeechConverter").GetComponent<SpeechConverter>();
        serverCommunicator = new ServerCommunicator();
        serverCommunicator.Start();
        Attendant_voice = new SpVoice();
        prompts.Add("hola");
        response = "default";
    }

    
    void Update(){

        //Below are test cases to test the voices of TTS with english and spanish
        if(Input.GetKeyDown(KeyCode.S))
        {
            Attendant_voice.Speak("Didn't we had some fun? Remember when the platform was sliding into the fire pit and I said Goodbye and you were like No way! and I was all We pretended we were going to murder you? That was great",
            SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            Attendant_voice.Speak("Buenos dias, bienvenido al Aeropuerto de Bogota. Cual es el proposito de su visita?",
            SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        }
        //check to see if either trigger is pressed to proceed with dialogue
        bool rightHandTrigger = false;
        controllers[0].inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out rightHandTrigger);

        bool leftHandTrigger = false;
        controllers[1].inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out leftHandTrigger);
        
        triggerIsDown = rightHandTrigger || leftHandTrigger;
        
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player" && !beganConvo){
            beganConvo = true;
            //scoring has been disabled for now
            //BeginScoring();
            BeginConversation();
            BeginVoiceRecording();
        }
    }
    public bool passAnswer = false;
    void BeginConversation(){
        passport.transform.localScale = Vector3.one;
        StartCoroutine(ConversationRoutine());
        IEnumerator ConversationRoutine(){
            //yield return new WaitForSeconds(2f);
            
            Debug.Log("BEGINNING CONVO");

            ServerCom(prompts[promptIndex]);
            
            while(response != null)
            {
                //unity check to see if we get the same question twicw move on with another topic. Not really used right now
                if(repetitive == true)
                {
                    StartCoroutine(ServerCom(prompts[promptIndex]));
                    repetitive = false;
                }
                //send message to server
                else
                {
                    StartCoroutine(ServerCom(transcript[voiceLineTracker]));
                }
                //wait for server response
                yield return new WaitUntil(()=>Server_flag);
                Attendant_voice.Speak(response,
                SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
                voiceLineTracker += 1; //all recorded data added to the
                Server_flag = false; 
                yield return new WaitUntil(()=>triggerIsDown);
                yield return new WaitUntil(()=>!triggerIsDown); //press and release
                yield return new WaitForSeconds(triggerDelay); // 
                //if repititive is triggered, move on to another topic in the prompts list
                if(repetitive)
                {
                    promptIndex++;
                }
        
                
            }
            //Old code used to use pre recorded questions and scoring instead of an AI model conversation
            foreach(AudioClip ac in audioClips){
                audioSource.clip = ac;
                audioSource.Play();
                yield return new WaitUntil(()=>audioSource.isPlaying);
                yield return new WaitUntil(()=>!audioSource.isPlaying);
                voiceLineTracker += 1; //all recorded data added to the 
                yield return new WaitUntil(()=>triggerIsDown);
                yield return new WaitUntil(()=>!triggerIsDown); //press and release
                yield return new WaitForSeconds(triggerDelay); // 

            }
            
            // Debug.Log("ALL DONE");
            audioSource.clip = finalClip;
            audioSource.Play();
            yield return new WaitUntil(()=>audioSource.isPlaying);
            yield return new WaitUntil(()=>!audioSource.isPlaying);
            voiceLineTracker += 1; 
            for(int i = 0; i<scores.Count; i++){

                responsePanel.Fill(transcript[i],scores[i]);
                if(i<transcript.Count && i <scores.Count){
                    if(transcript[i] == ""){
                        transcript[i] = "Error. Speech not recognized.";
                        scores[i] = "No score provided.";
                    }
                    responsePanel.Fill(transcript[i],scores[i]);
                    yield return new WaitUntil(()=>triggerIsDown);
                    yield return new WaitUntil(()=>!triggerIsDown);
                }
                
            }

            GameObject.FindObjectOfType<ScreenFader>().FadeOut();
            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadScene("MainMenu");
            yield return null;
        }
    }

    void BeginVoiceRecording(){
        
        //stores answers
        transcript = new List<string>();
        foreach(AudioClip ac in audioClips){
            transcript.Add("");
        }

        StartCoroutine(VoiceRecordingRoutine());

        IEnumerator VoiceRecordingRoutine(){
            speechConverter.RequestCache(); //clear the cache
            yield return new WaitUntil(()=>voiceLineTracker>-1); // wait until we finish our first voice line
            Debug.Log("VOICE LINES");
            //store the speech in the corresponding question slot of the transcript list
            while(response != null){
                string recordedSpeech = speechConverter.RequestCache();
                if(recordedSpeech != ""){
                    Debug.Log(voiceLineTracker + " ADDING TO TRANSCRIPT: " + recordedSpeech);
                    transcript[voiceLineTracker] += recordedSpeech + " ";
                }
                
                yield return null;
            }
            Debug.Log("VOICE LINES OVER");
            yield return null;
        }
    }


    //handles the sending of information to the backend
    void BeginScoring(){
        scores = new List<string>();
        foreach(AudioClip ac in audioClips){scores.Add("ERROR");}

        int cachedLine = voiceLineTracker;
        
        StartCoroutine(ScoreRoutine());
        IEnumerator ScoreRoutine(){
            yield return new WaitUntil(()=>voiceLineTracker > -1);
            cachedLine = voiceLineTracker;
            while(true){
                yield return new WaitUntil(()=>voiceLineTracker!=cachedLine);
                Debug.Log(cachedLine);
                messageToSend = transcript[cachedLine];
                cachedLine = voiceLineTracker;

                yield return null;
            }

            yield return null;
        }
    }

    IEnumerator WaitForScore(int index){
        yield return new WaitUntil(()=>serverCommunicator.GetLastResponse() != "");
        Debug.Log("RECIEVED SCORE: " + serverCommunicator.GetLastResponse());
        scores[index] = (serverCommunicator.GetLastResponse());
        serverCommunicator.AcknowledgeLastResponse();
    }

    //function used to receive responses from the AI model instead of requesting foir a score
    IEnumerator ServerCom(string msg){
        serverCommunicator.SendMessage(msg);
        yield return new WaitUntil(()=>serverCommunicator.GetLastResponse() != "");
        Debug.Log("RESPONSE: " + serverCommunicator.GetLastResponse());
        response = serverCommunicator.GetLastResponse();
        serverCommunicator.AcknowledgeLastResponse();
        Server_flag = true;
    }


}