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

    [Header("Traps")]
    public GameObject spikeTrapPrefab;
    public GameObject tridentTrapPrefab;
    public GameObject axeTrapPrefab;

    [Header("Enemies")]
    public GameObject skeletonPrefab;
    public GameObject slimePrefab;
    public GameObject ratPrefab;

    [Header("Props & Items")]
    public GameObject coinPrefab;
    public GameObject statue1Prefab;
    public GameObject statue2Prefab;
    public GameObject statue3Prefab;
    public GameObject vasePrefab;

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

                    switch (tile)
                    {
                        case 1:
                            GameObject floorObj = Instantiate(ground, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                            floorObj.transform.parent = transform;
                            break;
                        case 2:
                            GameObject wallObj = Instantiate(wall, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                            wallObj.transform.parent = transform;
                            break;
                        case 3:
                            GameObject goalObj = Instantiate(goal, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                            goalObj.transform.parent = transform;
                            break;
                        case 4:
                            if (spikeTrapPrefab != null)
                            {
                                GameObject trapObj = Instantiate(spikeTrapPrefab, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                                trapObj.transform.parent = transform;
                            }
                            break;
                        case 5:
                            GameObject floorForBox = Instantiate(ground, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                            floorForBox.transform.parent = transform;

                            GameObject boxObj = Instantiate(box, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                            boxObj.transform.parent = transform;
                            break;
                        case 6:
                            GameObject floorForPlayer = Instantiate(ground, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                            floorForPlayer.transform.parent = transform;

                            player.transform.position = new Vector3(x * tileSize, player.transform.position.y, y * tileSize);
                            break;
                        case 7: // Coin
                            GameObject floorForCoin = Instantiate(ground, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                            floorForCoin.transform.parent = transform;
                            if (coinPrefab != null)
                            {
                                GameObject coinObj = Instantiate(coinPrefab, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                                coinObj.transform.parent = transform;
                            }
                            break;
                        case 8: // TridentTrap
                            if (tridentTrapPrefab != null)
                            {
                                GameObject tridentObj = Instantiate(tridentTrapPrefab, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                                tridentObj.transform.parent = transform;
                            }
                            break;
                        case 9: // AxeTrap
                            if (axeTrapPrefab != null)
                            {
                                GameObject axeObj = Instantiate(axeTrapPrefab, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                                axeObj.transform.parent = transform;
                            }
                            break;
                        case 10: // Skeleton
                            GameObject floorForSkeleton = Instantiate(ground, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                            floorForSkeleton.transform.parent = transform;
                            if (skeletonPrefab != null)
                            {
                                GameObject skeletonObj = Instantiate(skeletonPrefab, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                                skeletonObj.transform.parent = transform;
                            }
                            break;
                        case 11: // Slime
                            GameObject floorForSlime = Instantiate(ground, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                            floorForSlime.transform.parent = transform;
                            if (slimePrefab != null)
                            {
                                GameObject slimeObj = Instantiate(slimePrefab, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                                slimeObj.transform.parent = transform;
                            }
                            break;
                        case 12: // Statue1
                            GameObject floorForStatue1 = Instantiate(ground, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                            floorForStatue1.transform.parent = transform;
                            if (statue1Prefab != null)
                            {
                                GameObject statue1Obj = Instantiate(statue1Prefab, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                                statue1Obj.transform.parent = transform;
                            }
                            break;
                        case 13: // Statue2
                            GameObject floorForStatue2 = Instantiate(ground, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                            floorForStatue2.transform.parent = transform;
                            if (statue2Prefab != null)
                            {
                                GameObject statue2Obj = Instantiate(statue2Prefab, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                                statue2Obj.transform.parent = transform;
                            }
                            break;
                        case 14: // Statue3
                            GameObject floorForStatue3 = Instantiate(ground, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                            floorForStatue3.transform.parent = transform;
                            if (statue3Prefab != null)
                            {
                                GameObject statue3Obj = Instantiate(statue3Prefab, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                                statue3Obj.transform.parent = transform;
                            }
                            break;
                        case 15: // Vase
                            GameObject floorForVase = Instantiate(ground, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                            floorForVase.transform.parent = transform;
                            if (vasePrefab != null)
                            {
                                GameObject vaseObj = Instantiate(vasePrefab, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                                vaseObj.transform.parent = transform;
                            }
                            break;
                        case 16: // Rat
                            GameObject floorForRat = Instantiate(ground, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                            floorForRat.transform.parent = transform;
                            if (ratPrefab != null)
                            {
                                GameObject ratObj = Instantiate(ratPrefab, new Vector3(x * tileSize, 0.0f, y * tileSize), transform.rotation);
                                ratObj.transform.parent = transform;
                            }
                            break;
                    }
                }
            }

            int doorY = height / 2;

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
                            if (y == doorY && doorPrefab != null)
                            {
                                borderPrefab = doorPrefab;
                            }
                            else if (wallPrefabs != null && wallPrefabs.Length > 0)
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
                            if (wallPrefabs != null && wallPrefabs.Length > 0)
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