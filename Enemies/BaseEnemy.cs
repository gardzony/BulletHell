using UnityEngine;

public class BaseEnemy : Enemy
{
    protected override void Update()
    {
        base.Update();

        if (attackTimer < attackFrequency)
        {
            attackTimer += Time.deltaTime;
        }
        else
        {
            if (IsPlayerInside)
            {
                Attack();
            }
        }
    }
    protected override void Attack()
    {
        base.Attack();
        damagebleTarget.TakeDamage(damage, damageType);
        attackTimer = 0f;
        Debug.Log("Enemy Attaced Heatlh" + health);
    }

    protected override void EnemyMovement()
    {
        base.EnemyMovement();

        if (player != null && !IsEnemyDead)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            selfRB.linearVelocity = direction * (moveSpeed + (moveSpeed * Mathf.Clamp(speedMultiplayer, -0.7f, 1)));
        }
        else
        {
            selfRB.linearVelocity = Vector2.zero;
        }
    }
    protected override void Death()
    {
        base.Death();

        damagebleTarget = null;
    }
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IsPlayerInside = true;
            damagebleTarget = collision.gameObject.GetComponent<IDamageable>();
        }
    }

    protected void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IsPlayerInside = false;
            damagebleTarget = null;
        }
    }
}
