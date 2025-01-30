using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public float fadeTime = 1f;
    public Image screenImage;
    
    void Start(){
        FadeIn();
    }
    public void FadeOut(){
         StartCoroutine(Fade(Color.clear,Color.black));
    }
    
    public void FadeIn(){
        StartCoroutine(Fade(Color.black,Color.clear));
    }

    IEnumerator Fade(Color from, Color to){
        screenImage.color = from;
        //Debug.Log("FADING");
        float t = 0; 
        while(t<fadeTime){
            t+=Time.deltaTime;
            screenImage.color = Color.Lerp(from,to,t/fadeTime);
            yield return null;
        }
        screenImage.color = to;
        yield return null;
    }
}