using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResponsePanel : MonoBehaviour
{
    public Text resposneText;
    public Text feedbackText; 
    public List<string> categories;
    public void Fill(string response, string prediction){
        transform.localScale = Vector3.one;
        resposneText.text = response;
        if(prediction.Contains("1")){
            feedbackText.text = "Possible issues detected.";
        }else{
            feedbackText.text = "No issues detected.";
        }
        List<string> predictions = new List<string>(prediction.Split(' '));
        for(int i = 0; i<predictions.Count; i++){
            if(predictions[i].Contains("1")){
                feedbackText.text += "\n";
                feedbackText.text += categories[i];
            }
        }
    }
}
