using UnityEngine;

public class SlimeTrail : MonoBehaviour
{
    public Animator animator;
    public float trapDuration = 0.5f;
    public float tileSize = 2.0f;

    private bool isTriggered = false;
    private float timer = 0.0f;

    void Update()
    {
        if (!isTriggered)
        {
            CheckPlayerStep();
        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= trapDuration)
            {
                Destroy(gameObject);
            }
        }
    }

    private void CheckPlayerStep()
    {
        Vector3 checkCenter = transform.position + (Vector3.up * 0.5f);
        Collider[] hits = Physics.OverlapSphere(checkCenter, tileSize * 0.4f);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                isTriggered = true;
                if (animator != null) animator.SetTrigger("Trap");
                break;
            }
        }
    }
}