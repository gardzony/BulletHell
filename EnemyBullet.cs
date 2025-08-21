using UnityEngine;

public class EnemyBullet : BaseBullet
{
    public void SetTarget(Vector2 direction, string targetTag, float enemyDamage)
    {
        base.SetTarget(direction, targetTag);
        damage = enemyDamage;
    }
}
