using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float AttackRange;
    public float AttackSpeed;
    public GameObject BulletPrefab;
    public float rotationSpeed = 5f; // Speed of rotation

    private float _attackCooldown;
    private List<Enemy> enemiesInRange = new List<Enemy>();
    private Transform weaponTransform;
    private Animator weaponAnimator;
    private Block blockStats;

    void Start()
    {
        _attackCooldown = 0f;

        // Create a trigger collider for enemy detection
        CircleCollider2D rangeCollider = gameObject.AddComponent<CircleCollider2D>();
        rangeCollider.isTrigger = true;
        rangeCollider.radius = AttackRange;

        weaponTransform = transform.GetChild(0).transform.GetChild(0).transform;
        weaponAnimator = transform.GetChild(0).transform.GetChild(0).GetComponent<Animator>();
        blockStats = this.gameObject.GetComponent<Block>();
    }

    void Update()
    {
        _attackCooldown -= Time.deltaTime;

        if (enemiesInRange.Count > 0)
        {
            Enemy targetEnemy = enemiesInRange[0];
            if (targetEnemy != null)
            {
                // Check if the tower's value is one that should rotate (e.g., 2, 4, 8)
                if (ShouldRotateWeapon(blockStats.Value))
                {
                    RotateWeaponTowards(targetEnemy.transform.position);
                }
                else
                {
                    // Optional: Reset weapon rotation if it shouldn't rotate
                    weaponTransform.rotation = Quaternion.identity;
                }

                if (_attackCooldown <= 0)
                {
                    FireAtEnemy(targetEnemy);
                    _attackCooldown = AttackSpeed;
                }
            }
        }
    }

    private bool ShouldRotateWeapon(int value)
    {
        // Define which tower values should have rotating weapons
        int[] rotatingValues = { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048}; // Add any other values that should rotate here
        return System.Array.IndexOf(rotatingValues, value) >= 0;
    }

    private void RotateWeaponTowards(Vector3 targetPosition)
    {
        // Calculate the direction to the target
        Vector3 direction = targetPosition - weaponTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Create the target rotation
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Smoothly rotate towards the target rotation
        weaponTransform.rotation = Quaternion.Lerp(weaponTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void FireAtEnemy(Enemy targetEnemy)
    {
        if (BulletPoolingManager.Instance != null)
        {
            Bullet bullet = BulletPoolingManager.Instance.GetBullet(blockStats.Value);
            if (bullet != null)
            {
                weaponAnimator.SetTrigger("Fire");
                bullet.transform.position = weaponTransform.position;
                bullet.SetTarget(targetEnemy, weaponTransform, blockStats.Value);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemiesInRange.Add(enemy);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemiesInRange.Remove(enemy);
            }
        }
    }

    // Visualize the tower's detection radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}
