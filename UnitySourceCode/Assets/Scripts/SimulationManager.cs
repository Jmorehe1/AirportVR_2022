using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
public class SimulationManager : MonoBehaviour
{
    public CourseGenerator courseGenerator;
    public ConversationManager conversationManager;
    public Transform userRig; //used for moving the user at the start of the simulation
    public bool teleportUser = false;
    bool teleportedToEnd = false;
    Vector3 endPos;
    Vector3 endRotation;
    public void Start(){
        endPos = userRig.position;
        endRotation = userRig.eulerAngles;
        XRSettings.enabled = true;
        StartSimulation();
    }

    public void StartSimulation(){
        Debug.Log("CONDITION");
        Debug.Log(PlayerPrefs.GetInt("condition"));
        if(PlayerPrefs.GetInt("condition") == 0){
            courseGenerator.GenerateFlat();
        }else if(PlayerPrefs.GetInt("condition") == 1){
            Debug.Log("STAIRS");
            courseGenerator.GenerateWithStairs();
        }else if(PlayerPrefs.GetInt("condition") == 3){
            //do nothing I guess?
            teleportUser = false;
            Debug.Log("DOIN NOTHING");
        }else{
            courseGenerator.GenerateWithStairs();
        }
        if(teleportUser){
            Transform courseStart = courseGenerator.GetStartOfCourse();
            userRig.position = courseStart.position;
            userRig.eulerAngles = courseStart.localEulerAngles;
        }
    }   

    void Update(){
        if(!conversationManager.beganConvo && !teleportedToEnd && Input.GetKeyDown(KeyCode.UpArrow)){
            //teleport user to the end
            Debug.Log("TELEPORTING TO END");
            teleportedToEnd = true;
            TeleportToEnd();
        }
        
       
    }

    void TeleportToEnd(){
        StartCoroutine(TeleportToEndRoutine());
        IEnumerator TeleportToEndRoutine(){
            ScreenFader sf = GameObject.FindObjectOfType<ScreenFader>();
            sf.FadeOut();
            yield return new WaitForSeconds(1f);
            //teleport
            //userRig.GetComponent<CharacterController>().enabled = false;
            userRig.position = endPos;
            userRig.eulerAngles = endRotation;
             yield return new WaitForSeconds(1f);
            //userRig.GetComponent<CharacterController>().enabled = true;
            sf.FadeIn();
        }
        
    }
}
