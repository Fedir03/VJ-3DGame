using UnityEngine;

public class SlimeEnemy : MonoBehaviour
{
    public float moveInterval = 3.0f;   
    public float speed = 1.0f;          
    public float heightJump = 0.5f;     
    public float tileSize = 2.0f;       

    private float timer = 0.0f;
    private bool isMoving = false;
    private float timeInMove = 0.0f;

    public GameObject slimeTrailPrefab;

    private Vector3 initialPos;
    private Vector3 vecMove;

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
        
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        Vector3 randomDir = directions[Random.Range(0, directions.Length)];

        initialPos = transform.position;

        bool isBlocked = false;

        Vector3 rayOrigin = initialPos + (Vector3.up * 0.5f);
        RaycastHit[] hits = Physics.RaycastAll(rayOrigin, randomDir, tileSize);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Box"))
            {
                isBlocked = true;
                break;
            }
        }

        LeaveSlimeTrail();

        if (isBlocked)
        {
            vecMove = Vector3.zero; 
        }
        else
        {
            vecMove = randomDir * tileSize; 
        }

        transform.rotation = Quaternion.LookRotation(randomDir);

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
        }
        else
        {
            Vector3 jumpMove = heightJump * Mathf.Sin(speed * timeInMove * Mathf.PI) * Vector3.up;
            transform.position = initialPos + (speed * timeInMove * vecMove) + jumpMove;
        }
    }
}