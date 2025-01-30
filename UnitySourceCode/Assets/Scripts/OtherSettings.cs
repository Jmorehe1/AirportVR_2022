using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherSettings : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Screen.SetResolution(384,216,true,-1); 
        Screen.SetResolution(1280,720,true,-1);
        UnityEngine.XR.XRSettings.gameViewRenderMode = UnityEngine.XR.GameViewRenderMode.OcclusionMesh;
    }
}
