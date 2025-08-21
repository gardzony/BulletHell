using UnityEngine;

public abstract class BaseBullet : MonoBehaviour
{
    [SerializeField] protected float baseDamage = 5f;
    [SerializeField] protected float speed = 3f;
    [SerializeField] protected int maxPenetrations = 3;
    [SerializeField] protected float bulletLifeTime = 2f;
    [SerializeField] protected G.DamageType damageType = G.DamageType.Physical;

    protected string _targetTag;
    protected int _penetrations;
    protected Vector2 _direction;
    protected float _timeAlive;
    protected float damage;

    protected virtual void OnEnable()
    {
        damage = baseDamage;
    }

    protected virtual void Start()
    {
        _penetrations = 0;
        _timeAlive = 0f;
    }

    protected virtual void Update()
    {
        transform.Translate(_direction.normalized * (speed * Time.deltaTime));
        _timeAlive += Time.deltaTime;
        if (_timeAlive >= bulletLifeTime)
        {
            ObjectPool.Instance.ReturnObjectToPool(gameObject);
        }
    }

    public virtual void SetTarget(Vector2 direction, string targetTag)
    {
        _direction = direction;
        _targetTag = targetTag;
        _penetrations = 0;
        _timeAlive = 0f;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_targetTag))
        {
            if (collision.TryGetComponent<Enemy>(out var enemy))
            {
                if (enemy.IsEnemyDead) return;
            }

            if (_penetrations < maxPenetrations)
            {
                collision.GetComponent<IDamageable>().TakeDamage(damage, damageType);
                _penetrations++;
            }
            else if (_penetrations == maxPenetrations)
            {
                collision.GetComponent<IDamageable>().TakeDamage(damage, damageType);
                _penetrations++;
                ObjectPool.Instance.ReturnObjectToPool(gameObject);
            }
            else
            {
                ObjectPool.Instance.ReturnObjectToPool(gameObject);
            }
        }
    }
}
