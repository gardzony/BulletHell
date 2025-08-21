using System;
using UnityEngine;
using UnityEngine.Rendering;
using static G;
public abstract class Enemy : MonoBehaviour ,IDamageable, IEffectable
{
    [Header("Базовые настройки")]
    [SerializeField] protected float baseMoveSpeed = 3f;
    [SerializeField] protected float baseHealth = 10f;
    [SerializeField] protected float baseDamage = 10f;
    [SerializeField] protected ParticleSystem poisonParticle;
    [SerializeField] protected float attackFrequency = 1f;
    [SerializeField] protected DamageType damageType;

    [Header("Сопротивления урону по типам")]
    [SerializeField] protected float armor = 0f;
    [SerializeField] protected float fireReduction = 0f;
    [SerializeField] protected float poisonReduction = 0f;
    [Header("Эффекты/баффы/состояния/прочее")]
    [SerializeField] protected GameObject coinPrefab;
    [SerializeField] protected int enemyKillCost = 1;

    protected float speedMultiplayer = 0f;
    protected float moveSpeed = 3f;
    protected CharacterEffects CharacterEffects;
    protected GameObject player;
    protected float health;
    protected Rigidbody2D selfRB;
    public bool IsEnemyDead;
    protected bool IsPlayerInside;
    protected IDamageable damagebleTarget;
    protected float attackTimer;
    protected float damage;

    public int SpawnWeight = 5;

    public ParticleSystem PoisonParticleSystem => poisonParticle;
    
    protected virtual void Start()
    {
        attackTimer = attackFrequency;
        IsPlayerInside = false;
        IsEnemyDead = false;
        moveSpeed = baseMoveSpeed;
        health = baseHealth;
        damage = baseDamage;

        player = GameManager.Instance.Player.gameObject;
        CharacterEffects = GetComponent<CharacterEffects>();
        selfRB = GetComponent<Rigidbody2D>();
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
    }
    
    public virtual void TakeDamage(float damage, DamageType damageType)
    {
        if (IsEnemyDead) return;
        switch (damageType)
        {
            case DamageType.Physical:
                health -= CalculateDamageWithReduction(armor, damage);
                break;
            case DamageType.Heavy:
                health -= CalculateDamageWithReduction(armor/3, damage);
                break;
            case DamageType.Fire:
                health -= CalculateDamageWithReduction(fireReduction, damage);
                break;
            case DamageType.Poison:
                health -= CalculateDamageWithReduction(poisonReduction, damage);
                break;
            case DamageType.Pure:
                health -= damage;
                break;
        }
        if (health <= 0) Death();
    }
    
    protected virtual void Death()
    {
        var coinObj = ObjectPool.Instance.GetObjectFromPool(coinPrefab);
        coinObj.SetActive(true);
        coinObj.transform.position = transform.position;
        var coin = coinObj.GetComponent<Coin>();
        coin.Target = player;
        coin.CoinValue = enemyKillCost;

        IsEnemyDead = true;
        IsPlayerInside = false;
        health = baseHealth;
        StopAllCoroutines();
        CharacterEffects.StopAllEffectCoroutines();
        CharacterEffects.ClearAllEffects();
        speedMultiplayer = 0f;
        ObjectPool.Instance.ReturnObjectToPool(gameObject);
        Debug.Log("Enemy is death");
    }

    public virtual void ResetEnemy()
    {
        IsPlayerInside = false;
        health = baseHealth;
        damage = baseDamage;
        speedMultiplayer = 0f;
        StopAllCoroutines();
    }

    public void DifficultAdoptation(int waveIndex)
    {
        ResetEnemy();
        health = Mathf.Pow(1.3f, waveIndex) * baseHealth;
        damage = Mathf.Pow(1.15f, waveIndex) * baseDamage;
        moveSpeed = baseMoveSpeed * (1 + waveIndex / 100f);
    }

    protected virtual float CalculateDamageWithReduction(float reduction, float damage)
    {
        float damageReduction = Mathf.Log(reduction + 1) / Mathf.Log(reduction + 1 + 20000);
        damageReduction = Mathf.Clamp(damageReduction, 0, 1);

        return damage * (1 - damageReduction);
    }

    public virtual void SpeedMultiplierChange(float speedMultiplier, G.OperationType operationType)
    {
        if(operationType == G.OperationType.Encreas) speedMultiplayer += speedMultiplier;
        else speedMultiplayer -= speedMultiplier;
    }

    protected virtual void EnemyMovement() { }

    protected virtual void Attack() { }
}