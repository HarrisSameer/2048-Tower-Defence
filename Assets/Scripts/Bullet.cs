using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 5f;
    private Enemy _target;
    private Vector2 _lastTargetPosition;  // Store the last known position of the target
    [SerializeField] private float _bulletDamage = 1f;
    private Transform _weapon;
    private int _towerValue;
    private bool _targetLost = false; // Track if the target has been lost

    Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Set target and reference to the weapon
    public void SetTarget(Enemy target, Transform weapon, int towerValue)
    {
        _target = target;
        _weapon = weapon;
        _towerValue = towerValue; // Store the tower value to identify the pool
        _targetLost = false; // Reset the target lost state
        if (_target != null)
        {
            _lastTargetPosition = _target.transform.position; // Initialize the last target position
        }
    }

    void Update()
    {
        // If the target has been destroyed or lost, set to move towards the last known position
        if (_target == null && !_targetLost)
        {
            _targetLost = true; // Mark the target as lost
        }

        // Determine the position to move towards (either target or last known position)
        Vector2 targetPosition = _targetLost ? _lastTargetPosition : _target.transform.position;

        // Calculate the direction towards the target or last known position
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // Rotate the weapon to face the direction of the target or last position
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (_weapon != null)
        {
            _weapon.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        // Rotate the bullet to face the direction of movement
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Move the bullet towards the target or last known position
        transform.Translate(Vector2.right * Speed * Time.deltaTime);

        // Check if the bullet has reached the target or the last known position
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        // Implement damage logic here if the target wasn't lost
        if (!_targetLost && _target != null)
        {
            _target.TakeDamage(_bulletDamage);
        }
        ReturnToPool();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_targetLost && _target != null)
        {
            _target.TakeDamage(_bulletDamage);
        }

        //_animator.SetTrigger("IsImpact");

        //Invoke("ReturnToPool",0.5f);

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        // Return the bullet to the pool instead of destroying it
        if (BulletPoolingManager.Instance != null)
        {
            BulletPoolingManager.Instance.ReturnBullet(_towerValue, this);
        }
        else
        {
            Destroy(gameObject); // Fallback in case the pooling manager isn't found
        }
    }
}
