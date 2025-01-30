using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VIDE_Data;

public class DialogueManager : MonoBehaviour
{
    VIDE_Assign dialogue;

    //Trackers
    int nodeIndex = 0;
    DialogueNode currentNode;
    void Start(){
        StartCoroutine(BeginDialogue());
    }

    IEnumerator BeginDialogue(){
        yield return new WaitForSeconds(1f);
        dialogue = GetComponent<VIDE_Assign>();
        nodeIndex = 1;//;dialogue.overrideStartNode;
        currentNode = dialogue.playerDiags[0];


        Debug.Log(currentNode.comment[0].text);
        Debug.Log(currentNode.comment[0].outNode.comment[0].text);
    }
}
