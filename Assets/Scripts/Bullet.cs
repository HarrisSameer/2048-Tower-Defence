using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 5f;
    private Enemy _target;
    [SerializeField] private float _bulletDamage = 1f;
    private Transform _weapon;

    // Set target and reference to the weapon
    public void SetTarget(Enemy target, Transform weapon)
    {
        _target = target;
        _weapon = weapon;
    }

    void Update()
    {
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Calculate the direction towards the enemy
        Vector2 direction = (_target.transform.position - transform.position).normalized;

        // Rotate the weapon to face the direction of the enemy
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (_weapon != null)
        {
            _weapon.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        // Rotate the bullet to face the direction of movement
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Move the bullet towards the enemy
        transform.Translate(Vector2.right * Speed * Time.deltaTime);

        // Check if the bullet has reached the enemy
        if (Vector2.Distance(transform.position, _target.transform.position) < 0.1f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        // Implement damage logic here
        if (_target != null)
        {
            _target.TakeDamage(_bulletDamage);
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_target != null)
        {
            _target.TakeDamage(_bulletDamage);
        }
        Destroy(gameObject);
    }
}
