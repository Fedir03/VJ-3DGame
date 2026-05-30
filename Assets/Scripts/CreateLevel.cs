using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CreateLevel : MonoBehaviour
{
    public GameObject player;
    public GameObject ground, wall, box, goal;
    public float tileSize = 2.0f;

    public GameObject[] wallPrefabs;
    public GameObject doorPrefab;
    public GameObject invisibleBorder;

    void Start()
    {
        string filename = Application.dataPath + "/Maps/map.txt";

        if (File.Exists(filename))
        {
            TextReader reader = File.OpenText(filename);
            string line = reader.ReadLine();
            string[] tokens = line.Split(' ');
            int width = int.Parse(tokens[0]);
            int height = int.Parse(tokens[1]);

            for (int y = 0; y < height; y++)
            {
                line = reader.ReadLine();
                tokens = line.Split(" ");
                for (int x = 0; x < width; x++)
                {
                    int tile = int.Parse(tokens[x]);

                    GameObject obj = Instantiate(ground, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                    obj.transform.parent = transform;

                    switch (tile)
                    {
                        case 2:
                            obj = Instantiate(wall, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                            obj.transform.parent = transform;
                            break;
                        case 3:
                            obj = Instantiate(goal, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                            obj.transform.parent = transform;
                            break;
                        case 5:
                            obj = Instantiate(box, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                            obj.transform.parent = transform;
                            break;
                        case 6:
                            player.transform.position = new Vector3(x * tileSize, player.transform.position.y, y * tileSize);
                            break;
                    }
                }
            }

            int doorX = width / 2;

            for (int x = -1; x <= width; x++)
            {
                for (int y = -1; y <= height; y++)
                {
                    if ((x == -1 && y == -1) || (x == -1 && y == height) || (x == width && y == -1) || (x == width && y == height))
                    {
                        continue;
                    }

                    if (x == -1 || x == width || y == -1 || y == height)
                    {
                        GameObject borderPrefab = null;
                        Quaternion spawnRotation = transform.rotation;

                        if (x == width)
                        {
                            if (wallPrefabs != null && wallPrefabs.Length > 0)
                            {
                                borderPrefab = wallPrefabs[Random.Range(0, wallPrefabs.Length)];
                            }
                            spawnRotation *= Quaternion.Euler(0, 90, 0);
                        }
                        else if (x == -1)
                        {
                            borderPrefab = invisibleBorder;
                            spawnRotation *= Quaternion.Euler(0, 90, 0);
                        }
                        else if (y == height)
                        {
                            if (x == doorX && doorPrefab != null)
                            {
                                borderPrefab = doorPrefab;
                            }
                            else if (wallPrefabs != null && wallPrefabs.Length > 0)
                            {
                                borderPrefab = wallPrefabs[Random.Range(0, wallPrefabs.Length)];
                            }
                        }
                        else if (y == -1)
                        {
                            borderPrefab = invisibleBorder;
                        }

                        if (borderPrefab != null)
                        {
                            GameObject borderObj = Instantiate(borderPrefab, new Vector3(x * tileSize, 0.0f, y * tileSize), spawnRotation);
                            borderObj.transform.parent = transform;
                            borderObj.tag = "Wall";
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("Map file could not be found!!!");
        }
    }
}