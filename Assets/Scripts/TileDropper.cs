using System.Collections;
using UnityEngine;

public class TileDropper : MonoBehaviour
{
    private float shakeDuration;
    private float fallSpeed;
    private Vector3 originalPos;

    public void StartDrop(float shake, float speed)
    {
        shakeDuration = shake;
        fallSpeed = speed;
        StartCoroutine(DropRoutine());
    }

    IEnumerator DropRoutine()
    {
        originalPos = transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-0.06f, 0.06f);
            float offsetZ = Random.Range(-0.06f, 0.06f);
            transform.position = originalPos + new Vector3(offsetX, 0, offsetZ);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Collider[] entitiesOnTop = Physics.OverlapSphere(originalPos + Vector3.up * 0.5f, 0.8f);
        foreach (Collider entity in entitiesOnTop)
        {
            if (entity.CompareTag("Player") || entity.CompareTag("Enemy"))
            {
                entity.transform.SetParent(this.transform);
            }
        }

        GameObject holeBlocker = new GameObject("HoleBlocker");
        holeBlocker.transform.position = originalPos;
        BoxCollider boxCol = holeBlocker.AddComponent<BoxCollider>();
        boxCol.size = new Vector3(2f, 2f, 2f);
        holeBlocker.tag = "Wall";

        while (transform.position.y > -15f)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}