using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using SpeechGenerationSystem;
using UnityText2Speech;
using UnityEngine.SceneManagement;


// using SpeechLib; // Old system namespace for SpVoice

public class Conversation_Manager_NEW : MonoBehaviour
{
    [Header("Config")]
    public float triggerDelay = 1f;
    public float animationTriggerChance = 0.8f; // Probability of triggering an animation (80%)
    public int cachedLine;
    [Header("Data")]
    public List<AudioClip> audioClips;
    public AudioClip finalClip;
    public Animator animator; // Animator for triggering animations

    [Header("Objects")]
    public AudioSource audioSource; // AudioSource for playing speech
    public ResponsePanel responsePanel;
    public Transform passport;

    // Old SpVoice text-to-speech system
    // SpVoice Attendant_voice; // Removed the old SpVoice object

    // New Speech System Fields
    public USgs speechGenerator; // Speech Generation System (replaces SpVoice)
    public Text inputText; // Text input to be converted to speech

    [Header("Controllers")]
    public List<XRController> controllers;

    //trackers Input 
    bool triggerIsDown = false;
    int voiceLineTracker = -1;


    private string[] conversationTriggers = { "1", "2", "3", "4" };

    //Misc=======================
    ServerCommunicator serverCommunicator;
    SpeechConverter speechConverter;


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

    void Awake()
    {
        passport.localScale = Vector3.zero;
    }

    void Start()
    {
        // Old SpVoice initialization (Removed)
        // Attendant_voice = new SpVoice();
        //inputText= new Text();

        speechConverter = GameObject.FindGameObjectWithTag("SpeechConverter").GetComponent<SpeechConverter>();
        serverCommunicator = new ServerCommunicator();
        serverCommunicator.Start();

        // Initialize the Speech Generation System
        speechGenerator = GetComponent<USgs>(); // Get the USgs component for speech generation
        speechGenerator.audioPlayer = audioSource; // Assign the AudioSource for playing speech

        prompts.Add("¿Ha notado algo sospechoso durante sus viajes hasta este momento?");

        response = "Default";
    }

    void Update()
    {
        //For means of testing
        if (Input.GetKeyDown(KeyCode.S))
        {

            // Define an array of three different sentences in Spanish
            string[] sentences = {
                "El día está soleado y hermoso.",  // Positive sentence
                "No me gusta cómo está lloviendo hoy.",  // Negative sentence
                "Hoy es un día como cualquier otro.",
                "Esta es una oración de ejemplo que utiliza el nuevo sistema de generación de voz."  // Neutral sentence
            };

            // Choose a random sentence from the array
            int randomIndex = Random.Range(0, sentences.Length);
            string selectedSentence = sentences[randomIndex];

            // Speak the selected sentence
            SpeakText(selectedSentence);

            // Classify the sentence as positive, negative, or neutral
            string classification = "";
            if (randomIndex == 0)
            {
                classification = "positive";
            }
            else if (randomIndex == 1)
            {
                classification = "negative";
            }
            else
            {
                classification = "neutral";
            }

            // Log the classification in the console
            Debug.Log("The selected sentence is: " + classification);

            SpeakText("Esta es una oración de ejemplo que utiliza el nuevo sistema de generación de voz.");
        }

        //check to see if either trigger is pressed to proceed with dialogue
        bool rightHandTrigger = false;
        controllers[0].inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out rightHandTrigger);

        bool leftHandTrigger = false;
        controllers[1].inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out leftHandTrigger);

        triggerIsDown = rightHandTrigger || leftHandTrigger;
    }

    // Method for triggering speech using the new Speech Generation System
    void SpeakText(string textToSpeak)
    {
        // Old system using SpVoice
        // Attendant_voice.Speak(textToSpeak, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        // Randomly decide if an animation should be triggered


        TriggerRandomAnimation();


        speechGenerator.ReceiveTextToSpeech(textToSpeak); // Convert and play speech using the Speech Generation System
    }

    // Method to randomly trigger one of the four conversation animations
    void TriggerRandomAnimation()
    {
        // Select a random index for the animation triggers
        int randomIndex = Random.Range(0, conversationTriggers.Length);

        // Trigger the selected animation
        if (animator != null)
        {
            animator.SetTrigger(conversationTriggers[randomIndex]);
        }
        else
        {
            Debug.LogWarning("Animator is not assigned in the Inspector.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !beganConvo)
        {
            beganConvo = true;
            //scoring has been disabled for now
            //BeginScoring();
            BeginConversation();
            BeginVoiceRecording();

        }
    }

    public bool passAnswer = false;
    // Trigger the start of the conversation
    void BeginConversation()
    {
        passport.transform.localScale = Vector3.one; // Show passport object for interaction
        StartCoroutine(ConversationRoutine());

    }

    void BeginVoiceRecording()
    {

        //stores answers
        transcript = new List<string>();
        foreach (AudioClip ac in audioClips)
        {
            transcript.Add("");
        }

        StartCoroutine(VoiceRecordingRoutine());
    }


    //handles the sending of information to the backend
    void BeginScoring()
    {
        scores = new List<string>();
        foreach (AudioClip ac in audioClips) { scores.Add("ERROR"); }

        cachedLine = voiceLineTracker;

        StartCoroutine(ScoreRoutine());
    }
    IEnumerator ConversationRoutine()
    {
        //yield return new WaitForSeconds(2f);

        Debug.Log("BEGINNING CONVO");

        ServerCom(prompts[promptIndex]);

        while (response != null)
        {
            Debug.Log(response);
            //unity check to see if we get the same question twicw move on with another topic. Not really used right now
            if (repetitive == true)
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
            yield return new WaitUntil(() => Server_flag);
            SpeakText(response);
            voiceLineTracker += 1; //all recorded data added to the
            Server_flag = false;
            yield return new WaitUntil(() => triggerIsDown);
            yield return new WaitUntil(() => !triggerIsDown); //press and release
            yield return new WaitForSeconds(triggerDelay); // 
                                                           //if repititive is triggered, move on to another topic in the prompts list
            if (repetitive)
            {
                promptIndex++;
            }


        }
        //Old code used to use pre recorded questions and scoring instead of an AI model conversation
        //foreach (AudioClip ac in audioClips)
        //{
        //    audioSource.clip = ac;
        //    audioSource.Play();
        //    yield return new WaitUntil(() => audioSource.isPlaying);
        //    yield return new WaitUntil(() => !audioSource.isPlaying);
        //    voiceLineTracker += 1; //all recorded data added to the 
        //    yield return new WaitUntil(() => triggerIsDown);
        //    yield return new WaitUntil(() => !triggerIsDown); //press and release
        //    yield return new WaitForSeconds(triggerDelay); // 

        //}

        //// Debug.Log("ALL DONE");
        //audioSource.clip = finalClip;
        //audioSource.Play();
        //yield return new WaitUntil(() => audioSource.isPlaying);
        //yield return new WaitUntil(() => !audioSource.isPlaying);
        voiceLineTracker += 1;
        for (int i = 0; i < scores.Count; i++)
        {

            responsePanel.Fill(transcript[i], scores[i]);
            if (i < transcript.Count && i < scores.Count)
            {
                if (transcript[i] == "")
                {
                    transcript[i] = "Error. Speech not recognized.";
                    scores[i] = "No score provided.";
                }
                responsePanel.Fill(transcript[i], scores[i]);
                yield return new WaitUntil(() => triggerIsDown);
                yield return new WaitUntil(() => !triggerIsDown);
            }

        }

        GameObject.FindObjectOfType<ScreenFader>().FadeOut();
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("MainMenu");
        yield return null;
    }
    IEnumerator VoiceRecordingRoutine()
    {
        speechConverter.RequestCache(); //clear the cache
        yield return new WaitUntil(() => voiceLineTracker > -1); // wait until we finish our first voice line
        Debug.Log("VOICE LINES");
        //store the speech in the corresponding question slot of the transcript list
        while (response != null)
        {
            string recordedSpeech = speechConverter.RequestCache();
            if (recordedSpeech != "")
            {
                Debug.Log(voiceLineTracker + " ADDING TO TRANSCRIPT: " + recordedSpeech);
                transcript[voiceLineTracker] += recordedSpeech + " ";
            }

            yield return null;
        }
        Debug.Log("VOICE LINES OVER");
        yield return null;
    }

    IEnumerator ScoreRoutine()
    {
        yield return new WaitUntil(() => voiceLineTracker > -1);
        cachedLine = voiceLineTracker;
        while (true)
        {
            yield return new WaitUntil(() => voiceLineTracker != cachedLine);
            Debug.Log(cachedLine);
            messageToSend = transcript[cachedLine];
            cachedLine = voiceLineTracker;

            yield return null;
        }

        yield return null;
    }

    IEnumerator WaitForScore(int index)
    {
        yield return new WaitUntil(() => serverCommunicator.GetLastResponse() != "");
        Debug.Log("RECIEVED SCORE: " + serverCommunicator.GetLastResponse());
        scores[index] = (serverCommunicator.GetLastResponse());
        serverCommunicator.AcknowledgeLastResponse();
    }

    //function used to receive responses from the AI model instead of requesting foir a score
    IEnumerator ServerCom(string msg)
    {
        serverCommunicator.SendMessage(msg);
        yield return new WaitUntil(() => serverCommunicator.GetLastResponse() != "");
        Debug.Log("RESPONSE: " + serverCommunicator.GetLastResponse());
        response = serverCommunicator.GetLastResponse();
        serverCommunicator.AcknowledgeLastResponse();
        Server_flag = true;
    }
}
