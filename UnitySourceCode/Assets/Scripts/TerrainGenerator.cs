using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public bool flatten = false;
    public int depth = 2;
    public int width = 500;
    public int height = 500;

    public float scale = 20f;

    void Start(){
        Terrain terrain = GetComponent<Terrain>();
        if(flatten){
            terrain.terrainData = GetFlatTerrainData(terrain.terrainData);
        }else{
            terrain.terrainData = GenerateTerrain(terrain.terrainData);
        }
        
    }

    TerrainData GetFlatTerrainData(TerrainData terrainData){
        terrainData.heightmapResolution = width+1;
        terrainData.size = new Vector3(width,depth,height);
        
        terrainData.SetHeights(0,0,ZeroArray());
        return terrainData;
    }

    TerrainData GenerateTerrain(TerrainData terrainData){
        terrainData.heightmapResolution = width+1;
        terrainData.size = new Vector3(width,depth,height);
        
        terrainData.SetHeights(0,0,GenerateNoise());
        return terrainData;
    }

    float[,] ZeroArray(){
        float[,] noise = new float[width,height];
        for(int x = 0; x<width; x++){
            for(int y = 0; y<height; y++){
                noise[x,y] = 0;
            }
        }
        return noise;
    }

    float[,] GenerateNoise(){
        float[,] noise = new float[width,height];
        for(int x = 0; x<width; x++){
            for(int y = 0; y<height; y++){
                noise[x,y] = GetNoiseAtPos(x,y);
            }
        }
        return noise;
    }

    float GetNoiseAtPos(int x, int y){
        float xCood = (float)x/width * scale;
        float yCoord = (float)y/height * scale;
        return Mathf.PerlinNoise(xCood,yCoord);


    }
}
