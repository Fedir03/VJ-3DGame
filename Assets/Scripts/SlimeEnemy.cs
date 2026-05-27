using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEnemy : MonoBehaviour
{
    public float moveInterval = 3.0f;
    public float speed = 1.0f;
    public float heightJump = 0.5f;
    public float tileSize = 2.0f;
    public GameObject slimeTrailPrefab;

    private float timer = 0.0f;
    private bool isMoving = false;
    private float timeInMove = 0.0f;

    private Vector3 initialPos;
    private Vector3 vecMove;

    private static HashSet<Vector3> reservedDestinations = new HashSet<Vector3>();

    void Update()
    {
        if (!isMoving)
        {
            timer += Time.deltaTime;
            if (timer >= moveInterval)
            {
                timer = 0.0f;
                PrepareRandomMove();
            }
        }
        else
        {
            UpdateMovement();
        }
    }

    private void PrepareRandomMove()
    {
        initialPos = transform.position;

        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        for (int i = 0; i < directions.Length; i++)
        {
            Vector3 temp = directions[i];
            int randomIndex = Random.Range(i, directions.Length);
            directions[i] = directions[randomIndex];
            directions[randomIndex] = temp;
        }

        bool foundValidMove = false;
        Vector3 chosenDir = Vector3.zero;

        foreach (Vector3 dir in directions)
        {
            Vector3 targetPos = initialPos + (dir * tileSize);

            if (reservedDestinations.Contains(targetPos))
            {
                continue;
            }

            bool isBlockedByPhysics = false;
            Vector3 rayOrigin = initialPos + (Vector3.up * 0.5f);
            RaycastHit[] hits = Physics.RaycastAll(rayOrigin, dir, tileSize);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Box") || hit.collider.CompareTag("Enemy"))
                {
                    isBlockedByPhysics = true;
                    break;
                }
            }

            if (!isBlockedByPhysics)
            {
                foundValidMove = true;
                chosenDir = dir;
                reservedDestinations.Add(targetPos);
                break;
            }
        }

        LeaveSlimeTrail();

        if (foundValidMove)
        {
            vecMove = chosenDir * tileSize;
            transform.rotation = Quaternion.LookRotation(chosenDir);
        }
        else
        {
            vecMove = Vector3.zero;
            transform.rotation = Quaternion.LookRotation(directions[0]);
        }

        isMoving = true;
        timeInMove = 0.0f;
    }

    private void LeaveSlimeTrail()
    {
        if (slimeTrailPrefab == null) return;

        bool alreadyHasTrail = false;
        RaycastHit[] hits = Physics.RaycastAll(initialPos + Vector3.up * 0.5f, Vector3.down, 1.0f);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("SlimeTrail"))
            {
                alreadyHasTrail = true;
                break;
            }
        }

        if (!alreadyHasTrail)
        {
            Instantiate(slimeTrailPrefab, initialPos, Quaternion.identity);
        }
    }

    private void UpdateMovement()
    {
        timeInMove += Time.deltaTime;

        if (timeInMove > 1.0f / speed)
        {
            transform.position = initialPos + vecMove;
            isMoving = false;

            if (vecMove != Vector3.zero)
            {
                reservedDestinations.Remove(initialPos + vecMove);
            }
        }
        else
        {
            Vector3 jumpMove = heightJump * Mathf.Sin(speed * timeInMove * Mathf.PI) * Vector3.up;
            transform.position = initialPos + (speed * timeInMove * vecMove) + jumpMove;
        }
    }

    private void OnDestroy()
    {
        if (isMoving && vecMove != Vector3.zero)
        {
            reservedDestinations.Remove(initialPos + vecMove);
        }
    }
}