using System.Collections.Generic;
using UnityEngine;

public class BulletPoolingManager : MonoBehaviour
{
    public static BulletPoolingManager Instance;

    [System.Serializable]
    public struct BulletPool
    {
        public int towerValue;
        public GameObject bulletPrefab;
        public int poolSize;
    }

    public BulletPool[] bulletPools;
    private Dictionary<int, ObjectPool<Bullet>> _pools;

    private void Awake()
    {
        Instance = this;
        _pools = new Dictionary<int, ObjectPool<Bullet>>();

        foreach (var pool in bulletPools)
        {
            // Pass the BulletPoolingManager's transform as the parent
            ObjectPool<Bullet> bulletPool = new ObjectPool<Bullet>(pool.bulletPrefab.GetComponent<Bullet>(), pool.poolSize, transform);
            _pools.Add(pool.towerValue, bulletPool);
        }
    }

    public Bullet GetBullet(int towerValue)
    {
        if (_pools.ContainsKey(towerValue))
        {
            return _pools[towerValue].Get();
        }
        return null;
    }

    public void ReturnBullet(int towerValue, Bullet bullet)
    {
        if (_pools.ContainsKey(towerValue))
        {
            _pools[towerValue].ReturnToPool(bullet);
        }
        else
        {
            Destroy(bullet.gameObject);
        }
    }
}
