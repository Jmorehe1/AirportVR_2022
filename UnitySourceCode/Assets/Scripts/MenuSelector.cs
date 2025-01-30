using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

public class MenuSelector : MonoBehaviour
{
    // Start is called before the first frame update
    public Image selectorBox;
    public List<Text> options;
    public List<Image> selectors;

    public List<XRController> controllers = null;

    public int selectionIndex = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckForSelection();
    }
    bool pressedSecondary = false;
    bool pressedPrimary = false;

    bool canCycle = true;
    bool canSelect = true;

    void CheckForSelection(){
        foreach(XRController controller in controllers){
            if(controller.enableInputActions){
                CheckForSelection(controller.inputDevice);   
                
            }
        }
    }
    private void CheckForSelection(InputDevice device){
        device.TryGetFeatureValue(CommonUsages.secondaryButton, out pressedSecondary);
        device.TryGetFeatureValue(CommonUsages.primaryButton, out pressedPrimary);
        if(pressedSecondary && canCycle){
            canCycle = false;
            MoveSelection();
        }else if(!pressedSecondary){
            canCycle = true;
        }
        if(pressedPrimary && canSelect){
            Select();
        }

        
        

    }

    public void Select(){
        if(selectionIndex == 0){
            SceneManager.LoadScene("MainScene");
        }
    }

    public void MoveSelection(){
        selectionIndex += 1;
        selectionIndex %= options.Count;
        for(int i = 0; i<3; i++){
            selectors[i].color = Color.clear;
            if(i == selectionIndex){
                Debug.Log("WHITE");
                selectors[i].color = Color.white;
            }
        }
    }
}
