using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConditionSetupHandler : MonoBehaviour
{
    public Dropdown conditionDropdown;
    public Text inputText; 
    public Button confirmButton;

    void Start(){
        Screen.SetResolution(1920,1080,true,-1);
    }

    void Update(){
        if(inputText.text != ""){
            confirmButton.gameObject.SetActive(true);
        }else{
            confirmButton.gameObject.SetActive(false);
        }
    }

    public void BeginTrial(){
        if(conditionDropdown.value == 2){
            SceneManager.LoadScene("DarkLanguageScene");
        }else{
            PlayerPrefs.SetInt("condition",conditionDropdown.value);
            PlayerPrefs.SetString("save name", inputText.text);
            SceneManager.LoadScene("ForestScene");
        }
        
    }
}
