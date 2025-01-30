using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System;
using UnityEngine.SceneManagement;

public class DialogueSequence : MonoBehaviour
{
    // Start is called before the first frame update
    bool triggered = false;
    public DialoguePanel dialoguePanel;
    public Movement userMovement;

    private KeywordRecognizer keywordRecognizer;
    public ConfidenceLevel confidence = ConfidenceLevel.Medium;

    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    public string[] keywords; 
    VIDE_Assign dialogue;

    public Animator animator;
    void Start(){
        dialogue = GetComponent<VIDE_Assign>();
    }

    void Quit(){
        Debug.Log("QUITTING...");
    }

    void OnTriggerEnter(Collider other){
        if(!triggered){
            ActivateDialogueSequence();
        }
    }

    void ActivateDialogueSequence(){
        triggered = true;
        //disable movement
        userMovement.disableMovement = true;

        //Debug.Log(dialogue.playerDiags[0].comment[0].text);
        dialoguePanel.Fill(dialogue.playerDiags[0]);

        StartCoroutine(AllowMovementRoutine());

    }

    IEnumerator AllowMovementRoutine(){
        yield return new WaitUntil(()=>!dialoguePanel.open);
        userMovement.disableMovement = false;
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("MainMenu");
    }

    //Voice Recognition Handling
    void RecognizedSpeech(PhraseRecognizedEventArgs args){
        Debug.Log(args.text);
    }
}
