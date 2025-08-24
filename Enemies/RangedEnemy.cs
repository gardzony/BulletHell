using UnityEngine;

public class RangedEnemy : Enemy
{
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected float attackDistance;
    [SerializeField] protected float attackRadius;
    protected float distanceToPlayer;
    protected bool isInAttackRange;

    protected override void Start()
    {
        base.Start();
        damagebleTarget = player.GetComponent<IDamageable>();
    }

    protected override void Update()
    {
        base.Update();
        distanceToPlayer = (player.transform.position - transform.position).magnitude;
        isInAttackRange = Mathf.Abs(distanceToPlayer - attackDistance) <= attackRadius;
        EnemyMovement();

        if (attackTimer < attackFrequency)
        {
            attackTimer += Time.deltaTime;
        }
        else
        {
            if (isInAttackRange)
            {
                Attack();
            }
        }
    }

    protected override void Attack()
    {
        base.Attack();
        var bullet = ObjectPool.Instance.GetObjectFromPool(projectilePrefab);
        if (bullet != null)
        {
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;
            bullet.SetActive(true);

            Vector2 direction = (player.transform.position - transform.position).normalized;
            bullet.GetComponent<EnemyBullet>().SetTarget(direction, "Player", damage);
        }
        Debug.Log("Дальняя атака");
        attackTimer = 0f;
    }

    protected override void EnemyMovement()
    {
        base.EnemyMovement();

        if (player != null && !IsEnemyDead && !isInAttackRange)
        {
            Vector2 direction;
            if (distanceToPlayer < attackDistance - attackRadius) direction = -(player.transform.position - transform.position).normalized;
            else direction = (player.transform.position - transform.position).normalized;
            selfRB.linearVelocity = direction * (moveSpeed + (moveSpeed * Mathf.Clamp(speedMultiplayer, -0.7f, 1)));
        }
        else
        {
            selfRB.linearVelocity = Vector2.zero;
        }
    }
}
