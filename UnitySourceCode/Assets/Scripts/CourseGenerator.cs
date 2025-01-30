using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseGenerator : MonoBehaviour
{
    [Header("Objects")]
    public List<GameObject> initialBumps; //place bumps from first block here for size configuration
    public GameObject block;
    

    [Header("Config")]
    public int blocksToSpawn;
    public int blocksPerTurn = 10;
    public int turnBuffer = 3;
    public int raiseLength = 2;
    public float stairHeight = 4.1f;

    public List<string> stairsPattern; 
    public List<string> flatPattern; 
    bool turned = false;

    //Tracking
    Transform courseStart;

    void Start(){

        //Generate();
    }

    public Transform GetStartOfCourse(){
        return courseStart;
    }

    public void GenerateWithStairs(){
        Generate(stairsPattern);
    }

    public void GenerateFlat(){
        Generate(flatPattern);
    }
    //FFUFFFDFFT
    //FUFDFUFDFT
    void Generate(List<string> pattern){
        Vector3 trackingPos = block.transform.position;
        Vector3 velocity = new Vector3(25f,0,0);
        Vector3 stairsOffset = new Vector3(0,0,0);
        int tracker = 0;
        bool raised = false; //are we going up stairs
        for(int i = 0; i<blocksToSpawn; i++){

            if(pattern[tracker] == "T"){
                turned = !turned;
                tracker = 0;
                //perform a turn
                if(velocity.x !=0){
                    trackingPos += velocity;
                    velocity = new Vector3(0,0,25f);

                    GameObject tempBlock = Instantiate(block,trackingPos,Quaternion.identity);
                    tempBlock.GetComponent<AirportBlock>().rightBotWall.SetActive(false);
                    tempBlock.GetComponent<AirportBlock>().rightTopWall.SetActive(false);


                    tempBlock.GetComponent<AirportBlock>().backBotWall.SetActive(true);
                    tempBlock.GetComponent<AirportBlock>().backTopWall.SetActive(true);

                    trackingPos += velocity;
                    block = Instantiate(block,trackingPos,Quaternion.identity);
                    block.GetComponent<AirportBlock>().downStairs.SetActive(false);
                    block.GetComponent<AirportBlock>().backBotWall.SetActive(false);
                    block.GetComponent<AirportBlock>().upStairs.SetActive(false);
                    block.GetComponent<AirportBlock>().frontBotWall.SetActive(false);
                    block.transform.localEulerAngles = new Vector3(0,-90,0);
                }else{
                    trackingPos += velocity;
                    velocity = new Vector3(25f,0,0);

                    GameObject tempBlock = Instantiate(block,trackingPos,Quaternion.identity);
                    tempBlock.GetComponent<AirportBlock>().leftBotWall.SetActive(false);
                    tempBlock.GetComponent<AirportBlock>().leftTopWall.SetActive(false);
                    tempBlock.GetComponent<AirportBlock>().backTopWall.SetActive(true);
                    tempBlock.GetComponent<AirportBlock>().backBotWall.SetActive(true);
                    tempBlock.transform.localEulerAngles = new Vector3(0,-90,0);
                    
                    trackingPos += velocity;
                    block = Instantiate(block,trackingPos,Quaternion.identity);
                    block.GetComponent<AirportBlock>().downStairs.SetActive(false);
                    block.GetComponent<AirportBlock>().backBotWall.SetActive(false);
                    block.GetComponent<AirportBlock>().upStairs.SetActive(false);
                    block.GetComponent<AirportBlock>().frontBotWall.SetActive(false);
                    block.transform.localEulerAngles = new Vector3(0,0,0);
                }
            }else{
                trackingPos += velocity;
                block = Instantiate(block,trackingPos+stairsOffset,Quaternion.identity);
                if(velocity.x == 0){
                    block.transform.localEulerAngles = new Vector3(0,-90,0);
                }
                if(pattern[tracker] == "U"){
                    block.GetComponent<AirportBlock>().downStairs.SetActive(true);
                    block.GetComponent<AirportBlock>().backBotWall.SetActive(true);
                    stairsOffset = new Vector3(0,stairHeight,0);
                    
                }else if(pattern[tracker] == "D"){
                    
                    block.GetComponent<AirportBlock>().upStairs.SetActive(true);
                    block.GetComponent<AirportBlock>().frontBotWall.SetActive(true);
                    block.transform.position -= stairsOffset;
                    stairsOffset = Vector3.zero;
                }else{
                    block.GetComponent<AirportBlock>().downStairs.SetActive(false);
                    block.GetComponent<AirportBlock>().backBotWall.SetActive(false);
                    block.GetComponent<AirportBlock>().upStairs.SetActive(false);
                    block.GetComponent<AirportBlock>().frontBotWall.SetActive(false);
                }
                
                
            }
            block.name = "Block(Clone)";

            tracker+=1;   
        }

        block.GetComponent<AirportBlock>().backBotWall.SetActive(true);
        block.GetComponent<AirportBlock>().backTopWall.SetActive(true);
        block.GetComponent<AirportBlock>().downStairs.SetActive(false);
        courseStart = block.transform;
    }
}
