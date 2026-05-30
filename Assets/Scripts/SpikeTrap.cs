using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public float idleDuration = 2.0f;
    public float warningDuration = 1.0f;
    public float activeDuration = 0.5f;

    public Animator animator;
    public GameObject blockingCollider;

    private float timer;
    private int currentState = 0;

    void Start()
    {
        timer = idleDuration;
        if (blockingCollider != null)
        {
            blockingCollider.SetActive(false);
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            AdvanceState();
        }
    }

    private void AdvanceState()
    {
        if (currentState == 0)
        {
            currentState = 1;
            timer = warningDuration;
            animator.SetTrigger("Warning");
        }
        else if (currentState == 1)
        {
            currentState = 2;
            timer = activeDuration;
            animator.SetTrigger("Active");

            if (blockingCollider != null)
            {
                blockingCollider.SetActive(true);
            }

            DealDamage();
        }
        else if (currentState == 2)
        {
            currentState = 0;
            timer = idleDuration;
            animator.SetTrigger("Idle");

            if (blockingCollider != null)
            {
                blockingCollider.SetActive(false);
            }
        }
    }

    private void DealDamage()
    {
        Vector3 checkCenter = transform.position + (Vector3.up * 0.5f);
        Collider[] hits = Physics.OverlapSphere(checkCenter, 0.4f);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                MovePlayer player = hit.GetComponentInParent<MovePlayer>();
                if (player != null)
                {
                    player.TakeDamage(1);
                }
            }
            else if (hit.CompareTag("Enemy"))
            {
                SlimeEnemy slime = hit.GetComponentInParent<SlimeEnemy>();
                if (slime != null)
                {
                    Destroy(slime.gameObject);
                }
            }
        }
    }
}