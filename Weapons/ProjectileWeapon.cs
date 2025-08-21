using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileWeapon : Weapon
{
    [SerializeField] protected float fireRate = 1f;
    [SerializeField] protected Transform bulletSpawnPosition;
    [SerializeField] protected int poolSize = 20;

    public GameObject BulletPrefab;

    private float _nextFireTime = 0f;
    private GameObject _currentTarget;
    private Quaternion _baseRotation;
    private Vector3 _baseScale;

    protected override void Start()
    {
        base.Start();
        _baseRotation = transform.localRotation;
        _baseScale = transform.localScale;

        ObjectPool.Instance.InitializePool(BulletPrefab, poolSize);
    }

    protected virtual void FixedUpdate()
    {
        var hitEnemies = FindEnemiesInAttackRange();
        if (hitEnemies.Count > 0) 
        {
            _currentTarget = FindNearestEnemy(hitEnemies).gameObject;
            LookAtTarget(_currentTarget);
        }
        else
        {
            _currentTarget = null;
            transform.localRotation = _baseRotation;
            transform.localScale = _baseScale;
        }

        if (_currentTarget != null && Time.time >= _nextFireTime)
        {
            Attack(_currentTarget);
            _nextFireTime = Time.time + 1f / fireRate;
        }
    }
    
    protected virtual Collider2D FindNearestEnemy(List<Collider2D> enemies)
    {
        Collider2D nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
    
    protected virtual void LookAtTarget(GameObject target)
    {
        Vector2 direction = (target.transform.position - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        
        Vector3 localScale = transform.localScale;
        if (direction.x < 0)
        {
            localScale.y = -Mathf.Abs(localScale.y);
        }
        else
        {
            localScale.y = Mathf.Abs(localScale.y);
        }
        transform.localScale = localScale;
    }

    protected override void Attack(GameObject target)
    {
        var bullet = ObjectPool.Instance.GetObjectFromPool(BulletPrefab);
        if (bullet != null)
        {
            bullet.transform.position = bulletSpawnPosition.position;
            bullet.transform.rotation = Quaternion.identity;
            bullet.SetActive(true);

            Vector2 direction = (target.transform.position - bulletSpawnPosition.position).normalized;
            var tmpBullet = bullet.GetComponent<Bullet>();
            tmpBullet.SetTarget(direction, targetTag);
            tmpBullet.SetDamage(damageMultiplier);
        }
    }
}