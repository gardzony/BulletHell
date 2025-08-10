using System;
using UnityEngine;

public abstract class Enemy : MonoBehaviour ,IDamageable, IEffectable
{
    [Header("Базовые настройки")]
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float baseHealth = 10f;
    [SerializeField] protected float baseDamage = 10f;
    [SerializeField] protected ParticleSystem poisonParticle;
    [SerializeField] protected float attackFrequency = 1f;
    
    [Header("Сопротивления урону по типам")]
    [SerializeField] protected float armor = 0f;
    [SerializeField] protected float fireReduction = 0f;
    [SerializeField] protected float poisonReduction = 0f;
    
    [Header("Эффекты/баффы/состояния")]
    
    protected float _speedMultiplayer = 0f;
    
    //[SerializeField] private GameManager gameManager;
    protected CharacterEffects CharacterEffects;
    protected GameObject Player;
    protected float Health;
    protected Rigidbody2D Rb;
    public bool IsEnemyDead;
    protected bool IsPlayerInside;
    private IDamageable _attackTarget;
    private float _attackTimer;
    protected float damage;
    
    public ParticleSystem PoisonParticleSystem => poisonParticle;
    
    protected virtual void Start()
    {
        _attackTimer = attackFrequency;
        IsPlayerInside = false;
        IsEnemyDead = false;
        Player = GameObject.FindGameObjectWithTag("Player");
        //.
        CharacterEffects = this.GetComponent<CharacterEffects>();
        //.
        Health = baseHealth;
        damage = baseDamage;
        Rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        EnemyMovement();
        if (Input.GetKeyDown(KeyCode.L))
        {
            CharacterEffects.ApplyPoisonEffect(1, 5);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            CharacterEffects.ApplySpeedBoostEffect(-0.5f, 5);
        }

        if (_attackTimer < attackFrequency)
        {
            _attackTimer += Time.deltaTime;
        }
        else
        {
            if (IsPlayerInside)
            {
                Attack(_attackTarget);
            }
        }
    }
    
    public virtual void TakeDamage(float damage, G.DamageType damageType)
    {
        if (IsEnemyDead) return;
        switch (damageType)
        {
            case G.DamageType.Physical:
                Health -= CalculateDamageWithReduction(armor, damage);
                break;
            case G.DamageType.Heavy:
                Health -= CalculateDamageWithReduction(armor/3, damage);
                break;
            case G.DamageType.Fire:
                Health -= CalculateDamageWithReduction(fireReduction, damage);
                break;
            case G.DamageType.Poison:
                Health -= CalculateDamageWithReduction(poisonReduction, damage);
                break;
            case G.DamageType.Pure:
                Health -= damage;
                break;
        }
        
        if (Health <= 0)
        {
            Death();
        }
    }
    
    protected virtual void Death()
    {
        IsEnemyDead = true;
        IsPlayerInside = false;
        _attackTarget = null;
        Health = baseHealth;
        StopAllCoroutines();
        CharacterEffects.StopAllEffectCoroutines();
        CharacterEffects.ClearAllEffects();
        _speedMultiplayer = 0f;
        ObjectPool.Instance.ReturnObjectToPool(gameObject);
        Debug.Log("Enemy is death");
    }

    public virtual void ResetEnemy()
    {
        IsPlayerInside = false;
        Health = baseHealth;
        damage = baseDamage;
        _speedMultiplayer = 0f;
        StopAllCoroutines();
    }

    public void DifficultAdoptation(int waveIndex)
    {
        ResetEnemy();
        Health = Mathf.Pow(1.3f, waveIndex) * baseHealth;
        damage = Mathf.Pow(1.15f, waveIndex) * baseDamage;
    }

    protected virtual float CalculateDamageWithReduction(float reduction, float damage)
    {
        float damageReduction = Mathf.Log(reduction + 1) / Mathf.Log(reduction + 1 + 20000);
        damageReduction = Mathf.Clamp(damageReduction, 0, 1);

        return damage * (1 - damageReduction);
    }

    public virtual void SpeedMultiplierChange(float speedMultiplier, G.OperationType operationType)
    {
        if(operationType == G.OperationType.Encreas) _speedMultiplayer += speedMultiplier;
        else _speedMultiplayer -= speedMultiplier;
    }

    public virtual void RemoveSpeedMultiplier(float speedMultiplier)
    {
        _speedMultiplayer -= speedMultiplier;
    }

    protected virtual void EnemyMovement()
    {
        if (Player != null && !IsEnemyDead)
        {
            Vector2 direction = (Player.transform.position - transform.position).normalized;
            Rb.linearVelocity = direction * (moveSpeed + (moveSpeed * Mathf.Clamp(_speedMultiplayer, -0.7f, 1)));
        }
        else
        {
            Rb.linearVelocity = Vector2.zero;
        }
    }

    protected virtual void Attack(IDamageable target)
    {
        target.TakeDamage(damage, G.DamageType.Physical);
        _attackTimer = 0f;
        Debug.Log("Enemy Attaced Heatlh" + Health);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IsPlayerInside = true;
            _attackTarget = collision.gameObject.GetComponent<IDamageable>();
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IsPlayerInside = false;
            _attackTarget = null;
        }
    }
}