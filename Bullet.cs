using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float baseDamage = 5f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private int maxPenetrations = 3;
    [SerializeField] private float bulletLifeTime = 2f;
    [SerializeField] private G.DamageType damageType = G.DamageType.Physical;
    
    private string _targetTag;
    private int _penetrations;
    private Vector2 _direction;
    private float _timeAlive;
    private float damage;

    private void OnEnable()
    {
        damage = baseDamage + baseDamage * BuffManager.Instance.DamageBuffs[damageType];
    }

    private void Start()
    {
        _penetrations = 0;
        _timeAlive = 0f;
    }

    private void Update()
    {
        transform.Translate(_direction.normalized * (speed * Time.deltaTime));
        _timeAlive += Time.deltaTime;
        if (_timeAlive >= bulletLifeTime)
        {
            ObjectPool.Instance.ReturnObjectToPool(gameObject);
        }
    }

    public void SetTarget(Vector2 direction, string targetTag)
    {
        _direction = direction;
        _targetTag = targetTag;
        _penetrations = 0;
        _timeAlive = 0f;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_targetTag))
        {
            if(collision.gameObject.GetComponent<Enemy>().IsEnemyDead) return;
            
            _penetrations++;
            collision.GetComponent<IDamageable>().TakeDamage(damage, damageType);
            if (_penetrations >= maxPenetrations)
            {
                ObjectPool.Instance.ReturnObjectToPool(gameObject);
            }
        }
    }
}