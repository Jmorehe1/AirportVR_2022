using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassportGrabber : MonoBehaviour
{
    bool grabbedPassport = false;
    public Vector3 endPosition;
    public float grabTime =1f;
    private void OnTriggerEnter(Collider other) {
        if(other.name == "Passport"){
            //do something idk
            if(!grabbedPassport){
                Debug.Log("GRABBED PASSPORT");
                grabbedPassport = true;
                other.transform.parent = null;
                StartCoroutine(PassportGrabRoutine(other.transform));
            }
            other.transform.parent = null;
            //move to the position
        }
    }

    IEnumerator PassportGrabRoutine(Transform pt){
        Vector3 cachedPos = pt.position;
        Vector3 cachedAng = pt.eulerAngles;
         pt.eulerAngles = new Vector3(-90,0,0);
        float t = 0;
        while(t<grabTime){
            yield return null;
            t+=Time.deltaTime;
            pt.position = Vector3.Lerp(cachedPos,endPosition,t/grabTime);
            
            
        }
        yield return null;
    }
}
