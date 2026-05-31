using System.Collections;
using UnityEngine;

public class RatEnemy : MonoBehaviour
{
    public float tileSize = 2.0f;
    public float jumpHeight = 0.5f;
    public float fastMoveDuration = 0.5f;
    public float slowMoveDuration = 1.0f;
    public float alertDuration = 0.5f;
    public float attackCooldown = 1.0f;

    [Header("Ajuste de Modelo")]
    public float modelRotationOffset = 90f;

    public Animator animator;

    private int fastStepsTaken = 0;
    private bool isDead = false;
    private bool isActing = false;

    private Vector3 initialPos;
    private Vector3 targetPos;
    private float moveTimer = 0f;
    private float currentMoveDuration = 1f;

    private Vector3 currentFaceDir = Vector3.forward;

    void Start()
    {
        animator.SetTrigger("Idle");
    }

    void Update()
    {
        if (isDead) return;

        if (!isActing)
        {
            CheckLineOfSight();
        }
    }

    private void CheckLineOfSight()
    {
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

        foreach (Vector3 dir in directions)
        {
            RaycastHit hit;
            Vector3 rayStart = transform.position + Vector3.up * 0.5f;

            if (Physics.Raycast(rayStart, dir, out hit, tileSize * 5.0f))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    currentFaceDir = dir;
                    transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(0, modelRotationOffset, 0);

                    float distance = Vector3.Distance(transform.position, hit.collider.transform.position);

                    if (distance <= tileSize + 0.1f)
                    {
                        StartCoroutine(AttackRoutine());
                    }
                    else
                    {
                        StartCoroutine(AlertAndRunRoutine(dir));
                    }
                    return;
                }
            }
        }

        fastStepsTaken = 0;
        animator.SetTrigger("Idle");
    }

    private IEnumerator AlertAndRunRoutine(Vector3 direction)
    {
        isActing = true;

        if (fastStepsTaken == 0)
        {
            animator.SetTrigger("Alert");
            yield return new WaitForSeconds(alertDuration);
        }

        while (true)
        {
            RaycastHit hit;
            bool playerStillInSight = false;
            float distanceToPlayer = 0f;

            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, out hit, tileSize * 5.0f))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    playerStillInSight = true;
                    distanceToPlayer = Vector3.Distance(transform.position, hit.collider.transform.position);
                }
            }

            if (!playerStillInSight)
            {
                break;
            }

            if (distanceToPlayer <= tileSize + 0.1f)
            {
                StartCoroutine(AttackRoutine());
                yield break;
            }

            Vector3 nextPos = transform.position + direction * tileSize;
            bool canMove = true;

            Collider[] hits = Physics.OverlapSphere(nextPos + Vector3.up * 0.5f, tileSize * 0.4f);
            foreach (Collider c in hits)
            {
                if (c.CompareTag("Wall") || c.CompareTag("Box") || c.CompareTag("Enemy"))
                {
                    canMove = false;
                    break;
                }
            }

            if (!canMove) break;

            currentMoveDuration = (fastStepsTaken < 3) ? fastMoveDuration : slowMoveDuration;
            float animSpeed = (fastStepsTaken < 3) ? 1.0f : 0.5f;

            animator.SetFloat("RunSpeed", animSpeed);
            animator.SetTrigger("Run");

            initialPos = transform.position;
            targetPos = nextPos;
            moveTimer = 0f;

            while (moveTimer < currentMoveDuration)
            {
                moveTimer += Time.deltaTime;
                float t = moveTimer / currentMoveDuration;

                Vector3 horizontalPos = Vector3.Lerp(initialPos, targetPos, t);
                float verticalPos = Mathf.Sin(t * Mathf.PI) * jumpHeight;

                transform.position = horizontalPos + Vector3.up * verticalPos;

                yield return null;
            }

            transform.position = targetPos;
            fastStepsTaken++;
        }

        fastStepsTaken = 0;
        isActing = false;
        animator.SetTrigger("Idle");
    }

    private IEnumerator AttackRoutine()
    {
        isActing = true;
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackCooldown);

        isActing = false;
    }

    public void PerformAttackDamage()
    {
        if (isDead) return;

        Vector3 checkCenter = transform.position + currentFaceDir * tileSize + Vector3.up * 0.5f;
        Collider[] hits = Physics.OverlapSphere(checkCenter, tileSize * 0.4f);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                MovePlayer player = hit.GetComponentInParent<MovePlayer>();
                if (player != null)
                {
                    player.TakeDamage(1);
                }
                break;
            }
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        StopAllCoroutines();

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        animator.SetTrigger("Die");
        Destroy(gameObject, 2.0f);
    }
}