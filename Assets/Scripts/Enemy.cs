using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float Health = 10f;

    public void TakeDamage(float damageAmount)
    {
        Health -= damageAmount;
        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Implement death logic here
        Destroy(gameObject);
    }
}
