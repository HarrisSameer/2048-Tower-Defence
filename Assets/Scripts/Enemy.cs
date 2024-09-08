using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public float Health = 10f;
    public float moveSpeed = 3f;          // Speed of the enemy
    public float attackRange = 1f;        // Range within which the enemy starts attacking the center tower
    public float detectionRadius = 5f;    // Distance at which enemies start heading towards the center tower
    public Transform centerTower;         // Reference to the center tower

    private bool isAttacking = false;
    private bool movingTowardsCenter = false;
    private Vector2 straightLineDirection;  // Initial direction in which enemies move (straight line)
    private Rigidbody2D rb;                 // Reference to the Rigidbody2D component

    private void Start()
    {
        centerTower = GameManager.Instance.centreTowerTransform;

        if (centerTower == null)
        {
            Debug.LogError("Center Tower not assigned in Enemy script.");
            return;
        }

        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        // Set Rigidbody2D to Kinematic to avoid interference from collisions
        rb.isKinematic = true;

        // Determine the straight line direction based on the enemy's position relative to the center tower
        SetInitialDirection();
    }

    private void SetInitialDirection()
    {
        // Check whether the enemy is to the left/right (horizontal movement) or above/below (vertical movement)
        if (Mathf.Abs(transform.position.x - centerTower.position.x) > Mathf.Abs(transform.position.y - centerTower.position.y))
        {
            // Horizontal movement (left or right)
            straightLineDirection = (transform.position.x < centerTower.position.x) ? Vector2.right : Vector2.left;
        }
        else
        {
            // Vertical movement (up or down)
            straightLineDirection = (transform.position.y < centerTower.position.y) ? Vector2.up : Vector2.down;
        }
    }

    private void FixedUpdate()
    {
        if (centerTower == null) return;

        if (isAttacking)
        {
            // Stop moving and attack
            AttackCenterTower();
        }
        else if (movingTowardsCenter)
        {
            // Move towards the center tower
            MoveTowards(centerTower.position);
        }
        else
        {
            // Move in a straight line
            MoveForward();

            // Check the distance to the center tower
            float distanceToCenter = Vector2.Distance(transform.position, centerTower.position);
            if (distanceToCenter <= detectionRadius)
            {
                movingTowardsCenter = true; // Start moving towards the center tower when within detection radius
            }
        }
    }

    private void MoveForward()
    {
        // Apply velocity in the determined straight line direction (either X or Y axis)
        rb.velocity = straightLineDirection * moveSpeed;
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        // Calculate the direction towards the center tower
        Vector2 direction = ((Vector2)targetPosition - rb.position).normalized;
        rb.velocity = direction * moveSpeed;

        // Check if the enemy has reached the attack range
        if (Vector2.Distance(rb.position, centerTower.position) <= attackRange)
        {
            isAttacking = true;
            rb.velocity = Vector2.zero;  // Stop the enemy's movement
        }
    }

    private void AttackCenterTower()
    {
        // Attack logic here (e.g., reduce tower's health)
        Debug.Log("Attacking the center tower");
        // Implement attack behavior like damaging the tower over time
    }

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
        // Handle the enemy's death
        Destroy(gameObject);
    }
}
