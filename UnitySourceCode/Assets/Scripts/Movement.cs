using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
public class Movement : LocomotionProvider
{

    public List<XRController> controllers = null;
    CharacterController characterController;
    private GameObject head;

    public DialoguePanel dialoguePanel;

    [Header("MOVEMENT CONFIG")]
    public float speed = 1f;
    public float gravityFactor = 1f;
    public bool disableMovement = false;

    public bool canMute = false;

    
    // Start is called before the first frame update
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        head = GetComponent<XRRig>().cameraGameObject;    
    }
    
    void Start(){
        PositionController();
    }

    // Update is called once per frame
    void Update()
    {
        PositionController();
        CheckForInput();
        ApplyGravity();
    }


    void PositionController(){
        float headHeight = Mathf.Clamp(head.transform.localPosition.y,1,2);
        characterController.height = headHeight;

        Vector3 newCenter = head.transform.localPosition;
        newCenter.y = characterController.height / 2;
        newCenter.y += characterController.skinWidth;

        characterController.center = newCenter;
        

    }

    void CheckForInput(){
        foreach(XRController controller in controllers){
            if(controller.enableInputActions){
                if(!disableMovement){
                    CheckForMovement(controller.inputDevice);
                }
                //CheckForSelection(controller.inputDevice);   
                
            }
        }
    }
    bool pressedSecondary = false;
    bool pressedPrimary = false;
    bool clickedStick = false;
    bool canCycle = false;
    bool canChoose = false;
    // private void CheckForSelection(InputDevice device){
    //     device.TryGetFeatureValue(CommonUsages.secondaryButton, out pressedSecondary);
    //     device.TryGetFeatureValue(CommonUsages.primaryButton, out pressedPrimary);
    //     device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out clickedStick);
    //     if(dialoguePanel.open){
    //         if(pressedSecondary && canCycle){
    //             canCycle = false;
    //             dialoguePanel.CycleSelection();
    //         }else if(!pressedSecondary){
    //             canCycle = true;
    //         }
    //         if(pressedPrimary && canChoose){
    //             canChoose = false;
    //             dialoguePanel.Choose();
    //         }else if(!pressedPrimary){
    //             canChoose = true;
    //         }
    //         if(clickedStick && canMute){
    //             speechConverter.ToggleMute();
    //             canMute = false;
    //         }else if(!clickedStick){
    //             canMute = true;
    //         }
    //     }

    // }

    private void CheckForMovement(InputDevice device){
        if(device.TryGetFeatureValue(CommonUsages.primary2DAxis,out Vector2 position)){
            StartMove(position.normalized);
        }
    }

    void StartMove(Vector2 position){
        Vector3 direction = new Vector3(position.x, 0,position.y);
        Vector3 headRotation = new Vector3(0,head.transform.eulerAngles.y,0);

        direction = Quaternion.Euler(headRotation) * direction;

        Vector3 movement = direction * speed;

        characterController.Move(movement * Time.deltaTime);
        


    }
    void ApplyGravity(){
        Vector3 gravity = new Vector3(0,Physics.gravity.y * gravityFactor,0);
        gravity.y *= Time.deltaTime;

        characterController.Move(gravity * Time.deltaTime);

    }


}   
