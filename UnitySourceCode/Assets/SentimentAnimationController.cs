using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;
using UMA.CharacterSystem;
using UMA.PoseTools;

public class SentimentAnimationController : MonoBehaviour
{
    [SerializeField] private ExpressionPlayer expression;             // Reference to UMA data for accessing blend shapes
    [SerializeField] private DynamicCharacterAvatar avatar; // Reference to UMA DynamicCharacterAvatar for UMA control
    [SerializeField] private Animator animator;           // Reference to Animator for body animations

    // Enum to track sentiment states
    public enum SentimentState
    {
        Neutral,
        Positive,
        Negative,
        Angry,
        Sad,
    }

    public SentimentState currentSentiment = SentimentState.Neutral; // Default to Neutral

    void Start()
    {
        // Initialize UMA and animator references if not assigned in inspector
        //  if (umaData == null) umaData = GetComponent<UMAData>();
        if (avatar == null) avatar = GetComponent<DynamicCharacterAvatar>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Check if the "P" key is pressed to cycle through sentiments
        if (Input.GetKeyDown(KeyCode.P))
        {
            CycleSentiment();               // Cycle to the next sentiment state
            Debug.Log("Input Recieved");
        }
        UpdateFacialExpression();
    }

    // Method to cycle through each sentiment for testing purposes
    void CycleSentiment()
    {
        // Advance to the next sentiment state in the enum
        currentSentiment = (SentimentState)(((int)currentSentiment + 1) % System.Enum.GetValues(typeof(SentimentState)).Length);
    }

    // Method to update sentiment based on input from sentiment analysis algorithm or test cycling
    public void UpdateSentiment(SentimentState newSentiment)
    {
        if (currentSentiment != newSentiment) // Only update if the sentiment has changed
        {
            currentSentiment = newSentiment;
            UpdateFacialExpression();         // Update facial expression blend shapes
            UpdateBodyAnimation();            // Trigger body animation
        }
    }

    // Method to adjust UMA blend shapes based on sentiment
    void UpdateFacialExpression()
    {
        float delta = 3 * Time.deltaTime;
        switch (currentSentiment)
        {
            case SentimentState.Positive:
                expression.leftMouthSmile_Frown = Mathf.Lerp(expression.leftMouthSmile_Frown, 1.0f, delta);
                expression.rightMouthSmile_Frown = Mathf.Lerp(expression.rightMouthSmile_Frown, 1.0f, delta);
                expression.midBrowUp_Down = Mathf.Lerp(expression.midBrowUp_Down, -1.0f, delta);
                expression.leftBrowUp_Down = Mathf.Lerp(expression.leftBrowUp_Down, 0f, delta);
                expression.rightBrowUp_Down = Mathf.Lerp(expression.rightBrowUp_Down, 0f, delta);
                expression.rightUpperLipUp_Down = Mathf.Lerp(expression.rightUpperLipUp_Down, 0f, delta);
                expression.leftUpperLipUp_Down = Mathf.Lerp(expression.leftUpperLipUp_Down, 0f, delta);
                expression.rightLowerLipUp_Down = Mathf.Lerp(expression.rightLowerLipUp_Down, -0.1f, delta);
                expression.leftLowerLipUp_Down = Mathf.Lerp(expression.leftLowerLipUp_Down, -0.1f, delta);
                expression.mouthNarrow_Pucker = Mathf.Lerp(expression.mouthNarrow_Pucker, 0f, delta);
                expression.jawOpen_Close = Mathf.Lerp(expression.jawOpen_Close, 0f, delta);
                expression.noseSneer = Mathf.Lerp(expression.noseSneer, 0.5f, delta);
                expression.leftEyeOpen_Close = Mathf.Lerp(expression.leftEyeOpen_Close, -0.2f, delta);
                expression.rightEyeOpen_Close = Mathf.Lerp(expression.rightEyeOpen_Close, -0.2f, delta);
                break;
            case SentimentState.Angry:
                expression.leftMouthSmile_Frown = Mathf.Lerp(expression.leftMouthSmile_Frown, -1.0f, delta);     // Downturned mouth
                expression.rightMouthSmile_Frown = Mathf.Lerp(expression.rightMouthSmile_Frown, -1.0f, delta);    // Downturned mouth
                expression.midBrowUp_Down = Mathf.Lerp(expression.midBrowUp_Down, 1.0f, delta);                  // Furrowed brows
                expression.leftBrowUp_Down = Mathf.Lerp(expression.leftBrowUp_Down, 1.0f, delta);                // Lowered left brow
                expression.rightBrowUp_Down = Mathf.Lerp(expression.rightBrowUp_Down, 1.0f, delta);              // Lowered right brow
                expression.rightUpperLipUp_Down = Mathf.Lerp(expression.rightUpperLipUp_Down, 0.5f, delta);      // Tensed upper lip
                expression.leftUpperLipUp_Down = Mathf.Lerp(expression.leftUpperLipUp_Down, 0.5f, delta);        // Tensed upper lip
                expression.rightLowerLipUp_Down = Mathf.Lerp(expression.rightLowerLipUp_Down, 0.2f, delta);      // Slight lower lip tension
                expression.leftLowerLipUp_Down = Mathf.Lerp(expression.leftLowerLipUp_Down, 0.2f, delta);        // Slight lower lip tension
                expression.mouthNarrow_Pucker = Mathf.Lerp(expression.mouthNarrow_Pucker, 0.2f, delta);          // Narrowed mouth
                expression.jawOpen_Close = Mathf.Lerp(expression.jawOpen_Close, -0.1f, delta);                   // Slight jaw clench
                expression.noseSneer = Mathf.Lerp(expression.noseSneer, 1.0f, delta);                            // Flared nostrils
                expression.leftEyeOpen_Close = Mathf.Lerp(expression.leftEyeOpen_Close, -0.3f, delta);           // Slightly narrowed eyes
                expression.rightEyeOpen_Close = Mathf.Lerp(expression.rightEyeOpen_Close, -0.3f, delta);         // Slightly narrowed eyes

                break;
            case SentimentState.Sad:
                expression.leftMouthSmile_Frown = Mathf.Lerp(expression.leftMouthSmile_Frown, -1.0f, delta);
                expression.rightMouthSmile_Frown = Mathf.Lerp(expression.rightMouthSmile_Frown, -1.0f, delta);
                expression.midBrowUp_Down = Mathf.Lerp(expression.midBrowUp_Down, 1.0f, delta);
                expression.leftBrowUp_Down = Mathf.Lerp(expression.leftBrowUp_Down, -1.0f, delta);
                expression.rightBrowUp_Down = Mathf.Lerp(expression.rightBrowUp_Down, -1.0f, delta);
                expression.rightUpperLipUp_Down = Mathf.Lerp(expression.rightUpperLipUp_Down, 0f, delta);
                expression.leftUpperLipUp_Down = Mathf.Lerp(expression.leftUpperLipUp_Down, 0, delta);
                expression.rightLowerLipUp_Down = Mathf.Lerp(expression.rightLowerLipUp_Down, 0f, delta);
                expression.leftLowerLipUp_Down = Mathf.Lerp(expression.leftLowerLipUp_Down, 0f, delta);
                expression.mouthNarrow_Pucker = Mathf.Lerp(expression.mouthNarrow_Pucker, -0.7f, delta);
                expression.jawOpen_Close = Mathf.Lerp(expression.jawOpen_Close, 0f, delta);
                expression.noseSneer = Mathf.Lerp(expression.noseSneer, -0.1f, delta);
                expression.leftEyeOpen_Close = Mathf.Lerp(expression.leftEyeOpen_Close, 1.0f, delta);
                expression.rightEyeOpen_Close = Mathf.Lerp(expression.rightEyeOpen_Close, 1.0f, delta);
                break;

            case SentimentState.Negative:
                expression.leftMouthSmile_Frown = Mathf.Lerp(expression.leftMouthSmile_Frown, -0.5f, delta);     // Subtle frown
                expression.rightMouthSmile_Frown = Mathf.Lerp(expression.rightMouthSmile_Frown, -0.5f, delta);   // Subtle frown
                expression.midBrowUp_Down = Mathf.Lerp(expression.midBrowUp_Down, 0.3f, delta);                 // Slight furrowed brows
                expression.leftBrowUp_Down = Mathf.Lerp(expression.leftBrowUp_Down, 0.2f, delta);               // Slightly lowered left brow
                expression.rightBrowUp_Down = Mathf.Lerp(expression.rightBrowUp_Down, 0.2f, delta);             // Slightly lowered right brow
                expression.rightUpperLipUp_Down = Mathf.Lerp(expression.rightUpperLipUp_Down, 0.1f, delta);     // Mild tension in upper lip
                expression.leftUpperLipUp_Down = Mathf.Lerp(expression.leftUpperLipUp_Down, 0.1f, delta);       // Mild tension in upper lip
                expression.rightLowerLipUp_Down = Mathf.Lerp(expression.rightLowerLipUp_Down, 0.0f, delta);     // Relaxed lower lip
                expression.leftLowerLipUp_Down = Mathf.Lerp(expression.leftLowerLipUp_Down, 0.0f, delta);       // Relaxed lower lip
                expression.mouthNarrow_Pucker = Mathf.Lerp(expression.mouthNarrow_Pucker, 0.1f, delta);         // Slightly narrowed mouth
                expression.jawOpen_Close = Mathf.Lerp(expression.jawOpen_Close, 0.0f, delta);                   // Neutral jaw position
                expression.noseSneer = Mathf.Lerp(expression.noseSneer, 0.1f, delta);                           // Slight nostril flare
                expression.leftEyeOpen_Close = Mathf.Lerp(expression.leftEyeOpen_Close, -0.1f, delta);          // Slightly narrowed eyes
                expression.rightEyeOpen_Close = Mathf.Lerp(expression.rightEyeOpen_Close, -0.1f, delta);        // Slightly narrowed eyes

                break;
            default: // Neutral or other undefined states
                expression.leftMouthSmile_Frown = Mathf.Lerp(expression.leftMouthSmile_Frown, 0, delta);
                expression.rightMouthSmile_Frown = Mathf.Lerp(expression.rightMouthSmile_Frown, 0, delta);
                expression.midBrowUp_Down = Mathf.Lerp(expression.midBrowUp_Down, 0, delta);
                expression.leftBrowUp_Down = Mathf.Lerp(expression.leftBrowUp_Down, 0, delta);
                expression.rightBrowUp_Down = Mathf.Lerp(expression.rightBrowUp_Down, 0, delta);
                expression.rightUpperLipUp_Down = Mathf.Lerp(expression.rightUpperLipUp_Down, 0, delta);
                expression.leftUpperLipUp_Down = Mathf.Lerp(expression.leftUpperLipUp_Down, 0, delta);
                expression.rightLowerLipUp_Down = Mathf.Lerp(expression.rightLowerLipUp_Down, 0, delta);
                expression.leftLowerLipUp_Down = Mathf.Lerp(expression.leftLowerLipUp_Down, 0, delta);
                expression.mouthNarrow_Pucker = Mathf.Lerp(expression.mouthNarrow_Pucker, 0, delta);
                expression.jawOpen_Close = Mathf.Lerp(expression.jawOpen_Close, 0, delta);
                expression.noseSneer = Mathf.Lerp(expression.noseSneer, 0, delta);
                expression.leftEyeOpen_Close = Mathf.Lerp(expression.leftEyeOpen_Close, 0, delta);
                expression.rightEyeOpen_Close = Mathf.Lerp(expression.rightEyeOpen_Close, 0, delta);
                break;
        }
    }

    // Helper method to set UMA blend shape weights by name

    // Method to trigger body animation based on sentiment
    void UpdateBodyAnimation()
    {
        // Reset all triggers to avoid conflicting animations
        ResetAllTriggers();

        // Trigger the corresponding body animation for the current sentiment
        animator.SetTrigger(currentSentiment.ToString()); // Ensure Animator triggers match enum names
    }

    // Helper method to reset all triggers to avoid animation conflicts
    void ResetAllTriggers()
    {
        foreach (SentimentState sentiment in System.Enum.GetValues(typeof(SentimentState)))
        {
            animator.ResetTrigger(sentiment.ToString());
        }
    }
}