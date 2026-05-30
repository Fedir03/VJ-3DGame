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
    public float rotationDuration = 0.2f;
    public Animator animator;

    private float timer = 0.0f;
    private bool isMoving = false;
    private bool isAttacking = false;
    private bool isRotating = false;
    private bool willAttack = false;
    private float timeInMove = 0.0f;
    private float timeInRotation = 0.0f;

    private Vector3 initialPos;
    private Vector3 vecMove;
    private Quaternion startRot;
    private Quaternion targetRot;
    private MovePlayer targetPlayerToDamage;

    private static HashSet<Vector3> reservedDestinations = new HashSet<Vector3>();

    void Start()
    {
        if (animator != null)
        {
            animator.SetTrigger("Idle");
        }
    }

    void Update()
    {
        if (!isMoving && !isAttacking && !isRotating)
        {
            timer += Time.deltaTime;
            if (timer >= moveInterval)
            {
                timer = 0.0f;
                PrepareRandomMove();
            }
        }
        else if (isRotating)
        {
            UpdateRotation();
        }
        else if (isMoving)
        {
            UpdateMovement();
        }
        else if (isAttacking)
        {
            UpdateAttack();
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
        bool attackingPlayer = false;
        MovePlayer targetPlayer = null;

        foreach (Vector3 dir in directions)
        {
            Vector3 targetPos = initialPos + (dir * tileSize);

            if (reservedDestinations.Contains(targetPos))
            {
                continue;
            }

            bool isBlockedByPhysics = false;

            Vector3 checkCenter = targetPos + (Vector3.up * 0.5f);
            Collider[] hits = Physics.OverlapSphere(checkCenter, tileSize * 0.4f);

            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    attackingPlayer = true;
                    targetPlayer = hit.GetComponentInParent<MovePlayer>();
                    chosenDir = dir;
                    break;
                }

                if (hit.gameObject != this.gameObject &&
                   (hit.CompareTag("Wall") || hit.CompareTag("Box") || hit.CompareTag("Enemy")))
                {
                    isBlockedByPhysics = true;
                    break;
                }
            }

            if (attackingPlayer)
            {
                break;
            }

            if (!isBlockedByPhysics)
            {
                foundValidMove = true;
                chosenDir = dir;
                reservedDestinations.Add(targetPos);
                break;
            }
        }

        if (attackingPlayer)
        {
            vecMove = chosenDir * tileSize;
            startRot = transform.rotation;
            targetRot = Quaternion.LookRotation(chosenDir);

            isRotating = true;
            willAttack = true;
            timeInRotation = 0.0f;
            targetPlayerToDamage = targetPlayer;

            return;
        }

        LeaveSlimeTrail();

        if (foundValidMove)
        {
            vecMove = chosenDir * tileSize;
            startRot = transform.rotation;
            targetRot = Quaternion.LookRotation(chosenDir);
        }
        else
        {
            vecMove = Vector3.zero;
            startRot = transform.rotation;
            targetRot = Quaternion.LookRotation(directions[0]);
        }

        isRotating = true;
        willAttack = false;
        timeInRotation = 0.0f;
    }

    private void UpdateRotation()
    {
        timeInRotation += Time.deltaTime;
        float t = timeInRotation / rotationDuration;

        if (t >= 1.0f)
        {
            transform.rotation = targetRot;
            isRotating = false;
            timeInMove = 0.0f;

            if (willAttack)
            {
                isAttacking = true;
                if (animator != null) animator.SetTrigger("Attack");

                if (targetPlayerToDamage != null)
                {
                    targetPlayerToDamage.TakeDamage(1);
                }
            }
            else
            {
                isMoving = true;
                if (animator != null) animator.SetTrigger("Jump");
            }
        }
        else
        {
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
        }
    }

    private void LeaveSlimeTrail()
    {
        if (slimeTrailPrefab == null) return;

        bool alreadyHasTrail = false;
        Collider[] hits = Physics.OverlapSphere(initialPos + Vector3.up * 0.5f, tileSize * 0.4f);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("SlimeTrail"))
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

            if (animator != null) animator.SetTrigger("Idle");

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

    private void UpdateAttack()
    {
        timeInMove += Time.deltaTime;

        if (timeInMove > 1.0f / speed)
        {
            transform.position = initialPos;
            isAttacking = false;

            if (animator != null) animator.SetTrigger("Idle");
        }
        else
        {
            float forwardBackward = Mathf.Sin(speed * timeInMove * Mathf.PI);
            Vector3 bumpMove = vecMove * 0.5f * forwardBackward;
            transform.position = initialPos + bumpMove;
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