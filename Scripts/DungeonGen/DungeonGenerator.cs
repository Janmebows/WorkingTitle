using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    [Range(0, 100)]
    public int wallPercent;
    [Range(1,1080)]
    public int dungeonWidth;
    [Range(1, 1080)]
    public int dungeonHeight;
    //used for aligning of tiles
    public float tileSize;
    //reproducibility
    public string seed;
    public bool randomSeed;
    System.Random rng;

    //number of times to smooth the map
    public int numSmoothingIterations;
    //how strong the smooth is 
    [Range(0, 8)]
    public int smoothStrength;
    //we will let 0 = empty, 1 = wall (possibly add variation to that later?
    int[,] map;
    // Use this for initialization
    void Start() {
        GenerateDungeon();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GenerateDungeon();
    }

    void GenerateDungeon()
    {
        map = new int[dungeonWidth, dungeonHeight];
        rng = new System.Random(seed.GetHashCode());
        //fills the map with 1s and 0s
        RandomFillMap();
        //smooths based on neighbouring tiles
        for (int i = 0; i < numSmoothingIterations; i++)
        {
            SmoothMap();
        }
    }


    void RandomFillMap()
    {
        if (randomSeed)
        {
            seed = Time.time.ToString();
        }
        for (int x=0;x<dungeonWidth;x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                if (x == 0 || x == dungeonWidth - 1 || y == 0 || y == dungeonHeight - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (rng.Next(0, 100) < wallPercent) ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap()
    {

            for (int x = 0; x < dungeonWidth ; x++)
            {
                for (int y = 0; y < dungeonHeight; y++)
                {
                    if (NumAdjacentWalls(x, y) > smoothStrength)
                    {
                        map[x, y] = 1;
                    }
                    else if (NumAdjacentWalls(x, y) < smoothStrength)
                    { 
                        map[x, y] = 0;
                    }
               
            }
        }
    }

    //Check all adjacent tiles for smoothing
    //Includes diagonals
    int NumAdjacentWalls(int xindex, int yindex)
    {
        int total = 0;
        for (int x = xindex-1; x <=xindex+1 ; x++)
        {
            for (int y = yindex-1; y <=yindex+1; y++)
            {
                if (x >= 0 && x < dungeonWidth && y >= 0 && y < dungeonHeight)
                {
                    if (x != xindex || y != yindex)
                    {
                        total += map[x, y];
                    }
                }
                else
                {
                    total++;
                }
            }
        }
        return total;
    }


    private void OnDrawGizmos()
    {
        //just avoid some fun errors on startup
        if(map!=null)
        {
            for (int x = 0; x < dungeonWidth; x++)
            {
                for (int y = 0; y < dungeonHeight; y++)
                {
                    Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
                    Vector3 position = new Vector3(-dungeonWidth / 2 + x + 0.5f, -dungeonHeight / 2 + y + 0.5f,0) * tileSize;
                    Gizmos.DrawCube(position, new Vector3(tileSize,tileSize));
                }
            }
        }
    }
}
