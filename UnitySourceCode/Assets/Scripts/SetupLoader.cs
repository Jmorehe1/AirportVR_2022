using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetupLoader : MonoBehaviour
{
    public GameObject speedBumps;
    public TerrainGenerator terrainGenerator;
    public DataLogger dataLogger;
    // Start is called before the first frame update
    void Awake(){
        dataLogger.testName = PlayerPrefs.GetString("save name");
        if(PlayerPrefs.GetInt("condition")==0){
            speedBumps.SetActive(false);
            terrainGenerator.flatten = true;
        }else if(PlayerPrefs.GetInt("condition")==1){
            speedBumps.SetActive(true);
            terrainGenerator.flatten = true;
        }else if(PlayerPrefs.GetInt("condition")==2){
            speedBumps.SetActive(false);
            terrainGenerator.flatten = false;
        }
    }

    void Update(){
        if(Input.GetAxisRaw("Cancel") != 0){
            SceneManager.LoadScene("StudySelection");
        }
    }
}
