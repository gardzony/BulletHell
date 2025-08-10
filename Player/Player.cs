using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour, IDamageable, IEffectable
{
    [Header("Базовые настройки персонажа")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private float baseHealth = 25f;
    [SerializeField] private ParticleSystem poisonParticle;
    public int MaxWeaponCount = 6;

    [Header("Сопротивления урону по типам")]
    [SerializeField] private float armor = 0f;
    [SerializeField] private float fireReduction = 0f;
    [SerializeField] private float poisonReduction = 0f;

    //[Header("Эффекты/баффы/состояния")]
    //[SerializeField] private float shield = 0f;
    private float _speedEffectMultiplayer = 0f;
    [SerializeField] private float evasionChance = 30f;
    //[SerializeField] private float speedBuff = 0f;
    //[SerializeField] private float criticalChance = 0f;

    private const float ImmortalityTime = 0.25f;
    private GameManager _gameManager;
    private CharacterEffects _characterEffects;
    private bool _isImmortal = false;
    private float _health;
    private Rigidbody2D _rb;
    private float _moveX;
    private float _moveY;
    private Vector2 _lastMoveDirection;
    private Coroutine _resetImmortalityCoroutine;

    public float BaseHealth
    {
        get { return baseHealth; }
        private set { baseHealth = value; }
    }

    private void Start()
    {
        _characterEffects = this.GetComponent<CharacterEffects>();

        _health = baseHealth;
        _rb = GetComponent<Rigidbody2D>();
        _gameManager = GameManager.Instance;
    }

    private void Update()
    {
        //Для теста
        //.
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TakeDamage(5f, G.DamageType.Physical);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            TakeDamage(5f, G.DamageType.Fire);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(5f, G.DamageType.Heavy);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(25f, G.DamageType.Poison);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            _characterEffects.ApplyPoisonEffect(1, 5);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            _characterEffects.ApplySpeedBoostEffect(0.5f, 5);
        }
        //.
        
        _moveX = Input.GetAxisRaw("Horizontal");
        _moveY = Input.GetAxisRaw("Vertical");
        if (_moveX != 0 || _moveY != 0)
        {
            _lastMoveDirection = new Vector2(_moveX, _moveY).normalized;
        }
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = GetCurrentSpeed();
    }

    private IEnumerator ResetImmortalityCoroutine()
    {
        yield return new WaitForSeconds(ImmortalityTime);
        _isImmortal = false;
    }

    public void SetPlayerImmortality()
    {
        _isImmortal = true;
        ResetImmortality();
    }
    
    private void ResetImmortality()
    {
        if (_resetImmortalityCoroutine != null)
        {
            StopCoroutine(_resetImmortalityCoroutine);
        }
        _resetImmortalityCoroutine = StartCoroutine(ResetImmortalityCoroutine());
        Debug.Log("resetImmortality");
    }
    
    private void Death()
    {
        StopAllCoroutines();
        _characterEffects.StopAllEffectCoroutines();
        _characterEffects.ClearAllEffects();
        _speedEffectMultiplayer = 0f;
        _isImmortal = false;
        _gameManager.GameLose();
        Debug.Log("Player is death");
    }

    public void ResetPlayer()
    {
        _health = baseHealth;
        healthBar.UpdateHealthBar(_health);
    }
    
    private float CalculateDamageWithReduction(float reduction, float damage)
    {
        float damageReduction = Mathf.Log(reduction + 1) / Mathf.Log(reduction + 1 + 20000);
        damageReduction = Mathf.Clamp(damageReduction, 0, 1);

        return damage * (1 - damageReduction);
    }

    public ParticleSystem PoisonParticleSystem => poisonParticle;

    public virtual void SpeedMultiplierChange(float speedMultiplier, G.OperationType operationType)
    {
        if (operationType == G.OperationType.Encreas) _speedEffectMultiplayer += speedMultiplier;
        else _speedEffectMultiplayer -= speedMultiplier;
    }

    public void TakeDamage(float damage, G.DamageType damageType)
    {
        switch (damageType)
        {
            case G.DamageType.Physical:
                if (!_isImmortal && GetPlayerEvasionState())
                {
                    _health -= CalculateDamageWithReduction(armor, damage);
                    _isImmortal = true;
                    ResetImmortality();
                }
                else
                {
                    Debug.Log("Immortal");
                }
                break;
            case G.DamageType.Heavy:
                if (!_isImmortal && GetPlayerEvasionState())
                {
                    _health -= CalculateDamageWithReduction(armor / 3, damage);
                    _isImmortal = true;
                    ResetImmortality();
                }
                else
                {
                    Debug.Log("Immortal");
                }
                
                break;
            case G.DamageType.Fire:
                _health -= CalculateDamageWithReduction(fireReduction, damage);
                break;
            case G.DamageType.Poison:
                _health -= CalculateDamageWithReduction(poisonReduction, damage);
                break;
            case G.DamageType.Pure:
                _health -= damage;
                break;
        }
        Debug.Log(_health + "  " + damageType.ToString());
        
        healthBar.UpdateHealthBar(_health);
        if (_health <= 0)
        {
            Death();
        }
    }

    public Vector2 GetLastMoveDirection() => _lastMoveDirection;

    private Vector2 GetCurrentSpeed() => new Vector2(_moveX, _moveY).normalized
        * (moveSpeed + (_speedEffectMultiplayer + (BuffManager.Instance.SpeedBuff / 100f)) * moveSpeed);

    private bool GetPlayerEvasionState() => Random.Range(0, 101) >= Mathf.Clamp(evasionChance, 0, 70);
}
