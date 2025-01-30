using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VIDE_Data;
using FrostweepGames.Plugins.GoogleCloud.TextToSpeech;
public class DialoguePanel : MonoBehaviour
{
    public GameObject display;
    public GameObject resultsPanel;
    public Text resultText;

    public Text speakerText;
    public List<ChoicePanel> choices;
    public Color deselectedColor;
    public Color selectedColor;
    public Color correctColor;
    public Color incorrectColor;
    DialogueNode currentNode;

    int selectionIndex = 0;
    public bool open = false;
    bool readyToSelect = false;
    // Start is called before the first frame update
    float correctTracker = 0;
    float totalTracker = 0;
    List<int> randomIndex; 

    public Animator animator;

    public GC_TextToSpeech_TutorialExample speaker;


    public List<string> currentOptions;
    void Start()
    {
        display.SetActive(false);
        resultsPanel.SetActive(false);
    }

    // Update is called once per frame
    public void Fill(DialogueNode dialogueNode){
        currentNode = dialogueNode;
        open = true;
        readyToSelect = false;
        StartCoroutine(FillRoutine());
    }

    IEnumerator FillRoutine(){
        display.SetActive(true);
        currentOptions = new List<string>();
        Deselect(); //reset color

        foreach(ChoicePanel c in choices){ //deactivate all
            c.gameObject.SetActive(false);
        }
        
        WriteText(speakerText,currentNode.comment[0].text);
        yield return new WaitUntil(()=>!writingText);

        randomIndex = new List<int>();

        //randomize order of objects -- this is not true random make sure to shuffle this later
        for(int i = 0; i<currentNode.comment[0].outNode.comment.Count; i++){
            if(Random.Range(0,2) == 0){
                randomIndex.Insert(0,i);
            }else{
                randomIndex.Add(i);
            }
        }
        

        for(int i = 0; i<currentNode.comment[0].outNode.comment.Count; i++){ //fill in the options with text
            yield return new WaitForSeconds(.1f);
            choices[i].gameObject.SetActive(true);
            choices[i].speechText.text = currentNode.comment[0].outNode.comment[randomIndex[i]].text;
            currentOptions.Add(currentNode.comment[0].outNode.comment[randomIndex[i]].text);
        }   
        readyToSelect = true;
        //Select(0);
        speaker.Speak(speakerText.text);
        animator.SetBool("talking",true);
        Debug.Log("SET BOOL");
        yield return null;
        animator.SetBool("talking",false);
    }

    void Close(){
        display.SetActive(false);
        StartCoroutine(ShowResultsPanel());
        open = false;
    }

    public void Select(int index){
        if(readyToSelect){
            Deselect();
            choices[index].GetComponent<Image>().color = selectedColor;
            selectionIndex = index;
        }
    }

    public void Choose(){ //transition 
        if(readyToSelect){
            readyToSelect = false;
            if(currentNode.comment[0].outNode.comment[randomIndex[selectionIndex]].outNode == null){
                Close();
                
            }else{
                StartCoroutine(ChooseRoutine());
            }
        }
    }

    IEnumerator ChooseRoutine(){
        yield return null;
        totalTracker += 1;
        if(randomIndex[selectionIndex] == 0){
            correctTracker += 1;
            choices[selectionIndex].GetComponent<Image>().color = correctColor;
        }else{
            choices[selectionIndex].GetComponent<Image>().color = incorrectColor;
        }
        yield return new WaitForSeconds(1f);
        Fill(currentNode.comment[0].outNode.comment[randomIndex[selectionIndex]].outNode);
    }

    void Deselect(){
        foreach(ChoicePanel c in choices){
            c.GetComponent<Image>().color = deselectedColor;
        }
    }

    void Update(){
        //manage selection input
    }

    public void CycleSelection(){
        selectionIndex += 1;
        if(selectionIndex > currentNode.comment[0].outNode.comment.Count - 1){
            selectionIndex = 0;
        }
        Select(selectionIndex);
    }

    bool writingText = false;
    public void WriteText(Text field, string message){
        writingText = true;
        StartCoroutine(WriteTextRoutine(field,message));

    }

    IEnumerator WriteTextRoutine(Text field, string message){
        field.text = "\"";
        yield return new WaitForSeconds(.01f);
        foreach(char c in message){
            field.text += c;
            yield return new WaitForSeconds(.01f);
        }
        field.text += "\"";
        yield return new WaitForSeconds(.01f);
        writingText = false;
    }
    
    IEnumerator ShowResultsPanel(){
        resultsPanel.SetActive(true);
        resultText.text = ((correctTracker / totalTracker) * 100).ToString("F0") + "%";
        yield return new WaitForSeconds(3f);
        resultsPanel.SetActive(false);
        
    }
}
