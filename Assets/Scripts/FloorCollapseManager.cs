using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorCollapseManager : MonoBehaviour
{
    [Header("Tiempos")]
    public float initialWait = 3.0f;
    public float shakeDuration = 1.0f;
    public float timeBetweenColumns = 1.0f;

    [Header("Velocidades de caida")]
    public float minFallSpeed = 4.0f;
    public float maxFallSpeed = 8.0f;

    void Start()
    {
        StartCoroutine(CollapseSequence());
    }

    IEnumerator CollapseSequence()
    {
        yield return new WaitForSeconds(initialWait);

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        foreach (Transform child in transform)
        {
            float xPos = Mathf.Round(child.position.x * 10f) / 10f;
            float zPos = Mathf.Round(child.position.z * 10f) / 10f;

            if (xPos < minX) minX = xPos;
            if (xPos > maxX) maxX = xPos;
            if (zPos < minZ) minZ = zPos;
            if (zPos > maxZ) maxZ = zPos;
        }

        Dictionary<float, List<Transform>> columns = new Dictionary<float, List<Transform>>();

        foreach (Transform child in transform)
        {
            float xPos = Mathf.Round(child.position.x * 10f) / 10f;
            float zPos = Mathf.Round(child.position.z * 10f) / 10f;

            if (xPos == minX || xPos == maxX || zPos == minZ || zPos == maxZ)
            {
                continue;
            }

            if (!columns.ContainsKey(xPos))
            {
                columns[xPos] = new List<Transform>();
            }
            columns[xPos].Add(child);
        }

        List<float> sortedX = columns.Keys.ToList();
        sortedX.Sort();

        foreach (float x in sortedX)
        {
            List<Transform> currentColumn = columns[x];

            foreach (Transform tile in currentColumn)
            {
                if (tile != null)
                {
                    TileDropper dropper = tile.gameObject.AddComponent<TileDropper>();
                    float randomSpeed = Random.Range(minFallSpeed, maxFallSpeed);
                    dropper.StartDrop(shakeDuration, randomSpeed);
                }
            }

            yield return new WaitForSeconds(shakeDuration + timeBetweenColumns);
        }
    }
}