using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject menuObject; // Assign your menu GameObject in the Inspector
    private bool dialogueFinished;

    void Start()
    {
        dialogueFinished = false;
        if (menuObject != null)
        {
            menuObject.SetActive(false); // Ensure the menu starts disabled
        }
    }

    void Update()
    {
        if (menuObject != null)
        {
            menuObject.SetActive(dialogueFinished);
        }
    }

    // Call this method when dialogue is finished
    public void SetDialogueFinished(bool isFinished)
    {
        dialogueFinished = isFinished;
    }
}
