using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MovePlayer player = other.GetComponentInParent<MovePlayer>();

            if (player != null)
            {
                player.AddCoin(1);
                Destroy(gameObject);
            }
        }
    }
}