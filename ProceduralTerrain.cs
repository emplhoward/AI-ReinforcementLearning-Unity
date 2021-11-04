//Codes by Leo Howard
//Copyrighted 07-30-2021

using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]

public class ProceduralTerrain : MonoBehaviour {

    //Terrain data:
    public Terrain terrain;
    public TerrainData terrainData;
    public float terrainLength;
    public float terrainHeight;

    //Texture vars:
    public Vector2 tileSize = new Vector2(50, 50);
    Vector2 tileOffset;

    //Set size and position:
    public void SetSizePosition()
    {
        float length = terrainLength;
        float width = terrainLength;
        float height = terrainHeight;

        terrain = this.GetComponent<Terrain>();
        terrain.terrainData.size = new Vector3(length, height, width);
        terrain.transform.position = new Vector3(-length / 2, 0, -width / 2);
        Debug.Log("Set!");
    }

    //Generate hills function--------------------------------------------------------------------------------------:
    public void GenerateMountains()
    {
        //Reset terrain first:
        ResetTerrain();

        terrain = this.GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        //Set spacing properties, the higher the space the less hills:
        float minFactor = 0.5f;
        float maxFactor = 1f;
        float minWidthSpacing = terrain.terrainData.size.x * minFactor;
        float maxWidthSpacing = terrain.terrainData.size.x * maxFactor;
        float minHeightSpacing = terrain.terrainData.size.y * minFactor;
        float maxHeightSpacing = terrain.terrainData.size.y * maxFactor;
        float randomWidthSpacing = UnityEngine.Random.Range(0, maxWidthSpacing);
        float randomHeightSpacing = UnityEngine.Random.Range(0, maxHeightSpacing);
        //Set voronoi tessellation properties:
        //The higher the voronoi falloff, the sharper the hills will be, this could be randomly generated for varied slope angles:
        float voronoiFallOff = 3f;
        float voronoiMinHeight = 0f;
        //Set the max height low for hills, higher for mountains:
        float voronoiMaxHeight = 0.2f;
        //Get current height map:
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution,
                                                terrainData.heightmapResolution);

        //Iterate through the terrain with specified random spacing:
        for (int x = 0; x < terrainData.heightmapResolution; x += (int)randomWidthSpacing)
        {
            for (int z = 0; z < terrainData.heightmapResolution; z += (int)randomHeightSpacing)
            {
                //Set the peak to a random x and y:
                Vector3 peak = new Vector3(x, UnityEngine.Random.Range(voronoiMinHeight, voronoiMaxHeight), z);
                //Set height to the currently highest peak, or just continue if terrain is still flat:
                if (heightMap[(int)peak.x, (int)peak.z] < peak.y)
                    heightMap[(int)peak.x, (int)peak.z] = peak.y;
                else
                    continue;
                //Declare peak location, which is used to calculate heights:
                Vector2 peakLocation = new Vector2(peak.x, peak.z);
                //Declare a maximum distance, from the origin to the corner of the terrain heightmap. This is used to scale the terrain heights. This means the terrain origin must be (0,0,0):
                float maxDistance = Vector2.Distance(new Vector2(0, 0), new Vector2(terrainData.heightmapResolution,
                                                                                  terrainData.heightmapResolution));
                //Iterate through all points in the map, and set the height in reference to the current peak:
                for (int a = 0; a < terrainData.heightmapResolution; a++)
                {
                    for (int b = 0; b < terrainData.heightmapResolution; b++)
                    {
                        //If the reference point is not the peak, then set the height of reference point:
                        if (!(a == peak.x && b == peak.z))
                        {
                            //Get distance to peak:
                            float distanceToPeak = Vector2.Distance(peakLocation, new Vector2(a, b)) / maxDistance;
                            //Scramble voronoi falloff for varied slope angles:
                            voronoiFallOff = UnityEngine.Random.Range(1, 4);
                            //Declare height:
                            float height = peak.y - distanceToPeak * voronoiFallOff;
                            //Set reference height only if current reference height is lower than calculated height:
                            if (heightMap[a, b] < height)
                                heightMap[a, b] = height;
                        }

                    }
                }
                randomHeightSpacing = UnityEngine.Random.Range(minHeightSpacing, maxHeightSpacing);
            }

            randomWidthSpacing = UnityEngine.Random.Range(minWidthSpacing, maxWidthSpacing);
        }

        terrainData.SetHeights(0, 0, heightMap);

        //Smooth out:
        for (int x = 0; x < 8; x ++)
        {
            Smooth();
        }

    }
    
    //Smoothing function--------------------------------------------------------------------------------------:
    public void Smooth()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution,
                                                          terrainData.heightmapResolution);


        for (int y = 0; y < terrainData.heightmapResolution; y++)
        {
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                float avgHeight = heightMap[x, y];

                List<Vector2> neighbours = GenerateNeighbours(new Vector2(x, y),
                                                              terrainData.heightmapResolution,
                                                              terrainData.heightmapResolution);
                foreach (Vector2 n in neighbours)
                {
                    avgHeight += heightMap[(int)n.x, (int)n.y];
                }

                heightMap[x, y] = avgHeight / ((float)neighbours.Count + 1);
            }
        }

        terrainData.SetHeights(0, 0, heightMap);
    }
   
    //Get neighbour function--------------------------------------------------------------------------------------:
    List<Vector2> GenerateNeighbours(Vector2 pos, int width, int height)
    {
        List<Vector2> neighbours = new List<Vector2>();

        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (!(x == 0 && y == 0))
                {
                    Vector2 nPos = new Vector2(Mathf.Clamp(pos.x + x, 0, width - 1),
                                                Mathf.Clamp(pos.y + y, 0, height - 1));
                    if (!neighbours.Contains(nPos))
                        neighbours.Add(nPos);
                }
            }
        }
        return neighbours;
    }
    
    //Reset terrain function--------------------------------------------------------------------------------------:
    public void ResetTerrain()
    {
        terrain = this.GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        float[,] heightMap;
        heightMap = new float[terrainData.heightmapResolution,terrainData.heightmapResolution];
        for (int x = 0; x < terrainData.heightmapResolution; x++)
        {
            for (int z = 0; z < terrainData.heightmapResolution; z++)
            {
                heightMap[x, z] = 0;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);

    }
    
    //Generate texture function--------------------------------------------------------------------------------------:
    public void RandomizeTexture()
    {
        //Declare splat map data:
        float[,,] splatmapData = new float[terrainData.alphamapWidth,
                                               terrainData.alphamapHeight,
                                               terrainData.alphamapLayers];

        Texture2D[] textureArray = new Texture2D [4];

        //Check if enough terrain layers are created:
        if (terrainData.terrainLayers.Length > 0)
        {
            //Set properties of each layer:
            for (int x = 0; x < terrainData.terrainLayers.Length; x++)
            {
                
                if (terrainData.terrainLayers[x].diffuseTexture != null)
                {
                    textureArray[x] = terrainData.terrainLayers[x].diffuseTexture;
                    terrainData.terrainLayers[x].tileSize = tileSize;               
                    tileOffset.x = UnityEngine.Random.Range(0, 1f);
                    tileOffset.y = UnityEngine.Random.Range(0, 1f);
                    terrainData.terrainLayers[x].tileOffset = tileOffset;
                }
                else
                {
                    Debug.Log("You need to set the textures first, before randomizing the textures.");
                }

            }

            RandomizeTextureEvenly(splatmapData, textureArray);

            terrainData.SetAlphamaps(0, 0, splatmapData);         
        }
        //Prompt for creations of terrain layers, if they're not created:
        else
        {
            Debug.Log("You need to create at least 4 terrain texture layers first.");
        }

        
    }

    //Randomize texture with streaks--------------------------------------------------------------------------------------:
    public void RandomizeTextureStreaks(float[,,] splatmapData, Texture2D[] textureArray)
    {
        //First set all alpha to 0:
        //Iterate through the terrain:
        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                //Iterate through all the layers:
                for (int j = 0; j < textureArray.Length; j++)
                {
                    //Set the alpha of the layer i in position (x,y):
                    splatmapData[x, y, j] = 0f;
                }
            }
        }

        //Set the base textures to randomize evenly:
        //Iterate through the terrain:
        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                //Declare a random range to represent the chance of showing layer:
                int chosenLayer = UnityEngine.Random.Range(0, 2);
                //Set the alpha of the layer "chanceToShow" in position (x,y):
                splatmapData[x, y, chosenLayer] = 1;
            }
        }

        //Create random streaks of the last 2 layers:
        int minHeightSpacing = 1;
        int maxHeightSpacing = 30;
        int minWidthSpacing = 1;
        int maxWidthSpacing = 40;
        int randomHeightSpacing = UnityEngine.Random.Range(minHeightSpacing, maxHeightSpacing);
        int randomWidthSpacing = UnityEngine.Random.Range(minWidthSpacing, maxWidthSpacing);
        //Iterate in random steps:
        for (int y = 0; y < terrainData.alphamapHeight; y += randomHeightSpacing)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x += randomWidthSpacing)
            {
                //Iterate through all the layers and set them to 0 first:
                for (int j = 0; j < textureArray.Length; j++)
                {
                    //Clamp x and y to prevent out of bound error:
                    if (y >= terrainData.alphamapHeight)
                    {
                        y = terrainData.alphamapHeight - 1;
                    }

                    //Clamp x and y to prevent out of bound error:
                    if (x >= terrainData.alphamapWidth)
                    {
                        x = terrainData.alphamapWidth - 1;
                    }

                    //Set the alpha of the layer i in position (x,y):
                    splatmapData[x, y, j] = 0f;
                }

                //Choose a random layer from the last 2 textures:
                int chosenLayer = UnityEngine.Random.Range(2, 3);

                //Set the alpha of the layer in position (x,y) and adjacent tiles randomly:
                splatmapData[x, y, chosenLayer] = 1;
                for (int a = 0; a < UnityEngine.Random.Range(1, 20); a++)
                {
                    //Clamp x and y to prevent out of bound error:
                    if (x + a < terrainData.alphamapWidth)
                    {
                        //Choose layer:
                        chosenLayer = UnityEngine.Random.Range(1, 3);

                        //Iterate through all the layers and set them to 0 first:
                        for (int j = 0; j < textureArray.Length; j++)
                        {
                            //Set the alpha of the layer i in position (x,y):
                            splatmapData[x + a, y, j] = 0f;
                        }

                        //Then set the chosen layer:
                        splatmapData[x + a, y, chosenLayer] = 1;

                        //Set random y streaks:
                        for (int b = 0; b < UnityEngine.Random.Range(1, 10); b++)
                        {
                            //Clamp x and y to prevent out of bound error:
                            if (y + b < terrainData.alphamapHeight)
                            {
                                //Iterate through all the layers and set them to 0 first:
                                for (int j = 0; j < textureArray.Length; j++)
                                {
                                    //Set the alpha of the layer i in position (x,y):
                                    splatmapData[x + a, y + b, j] = 0f;
                                }

                                //Set the chosen layer:
                                splatmapData[x + a, y + b, chosenLayer] = 1;
                            }
                        }
                    }

                }

                //Choose the next x tile randomly:
                randomWidthSpacing = UnityEngine.Random.Range(0, maxWidthSpacing);
            }
            //Chose the next y tile randomly:
            randomWidthSpacing = UnityEngine.Random.Range(minWidthSpacing, maxWidthSpacing);
        }

    }

    //Randomize Texture Evenly--------------------------------------------------------------------------------------:
    public void RandomizeTextureEvenly(float[,,] splatmapData, Texture2D[] textureArray)
    {
        //First set all alpha to 0:
        //Iterate through the terrain:
        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                //Iterate through all the layers:
                for (int j = 0; j < textureArray.Length; j++)
                {
                    //Set the alpha of the layer i in position (x,y):
                    splatmapData[x, y, j] = 0f;
                }
            }
        }

        //Set the first combination of layers randomly:
        //Iterate through the terrain:
        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                //Declare a random range to represent the chance of showing layer:
                int chosenLayer = UnityEngine.Random.Range(0, 4);
                //Set the alpha of the layer "chanceToShow" in position (x,y):
                splatmapData[x, y, chosenLayer] = 1;
            }
        }

        //Randomize the terrain again, setting the current layer to 0 and making the base texture 1:
        int minHeightSpacing = 1;
        int maxHeightSpacing = 15;
        int minWidthSpacing = 1;
        int maxWidthSpacing = 30;
        int randomHeightSpacing = UnityEngine.Random.Range(minHeightSpacing, maxHeightSpacing);
        int randomWidthSpacing = UnityEngine.Random.Range(minWidthSpacing, maxWidthSpacing);
        //Iterate in random steps:
        for (int y = 0; y < terrainData.alphamapHeight; y += randomHeightSpacing)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x += randomWidthSpacing)
            {
                //Iterate through all the layers and set the active one to a random alpha:
                for (int j = 0; j < textureArray.Length; j++)
                {
                    if (splatmapData[x, y, j] != 0)
                    {
                        splatmapData[x, y, j] = 0;
                    }
                }

                //Choose a random layer:
                int chosenLayer = UnityEngine.Random.Range(0, 4);
                //Set the alpha of the layer "chanceToShow" in position (x,y):
                //Set the alpha randomly to blend with original texture:
                splatmapData[x, y, chosenLayer] = 1;

                //Increase x of the next tile randomly:
                randomWidthSpacing = UnityEngine.Random.Range(0, maxWidthSpacing);
            }
            //Increase y of the next tile randomly:
            randomWidthSpacing = UnityEngine.Random.Range(minWidthSpacing, maxWidthSpacing);
        }
    }
}
