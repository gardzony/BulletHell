using UnityEngine;

public class ShotGun : ProjectileWeapon
{
    [SerializeField] private float spread = 25f;
    [SerializeField] private float bulletCount = 12f;
    
    protected override void Attack(GameObject target)
    {
        Vector2 baseDirection = (target.transform.position - bulletSpawnPosition.position).normalized;

        for (int i = 0; i < bulletCount; i++)
        {
            GameObject bullet = ObjectPool.Instance.GetObjectFromPool(BulletPrefab);
            if (bullet != null)
            {
                bullet.transform.position = bulletSpawnPosition.position;
                bullet.transform.rotation = Quaternion.identity;
                bullet.SetActive(true);

                float randomSpread = Random.Range(-spread, spread);
                Quaternion spreadRotation = Quaternion.Euler(0, 0, randomSpread);
                Vector2 spreadDirection = spreadRotation * baseDirection;

                bullet.GetComponent<Bullet>().SetTarget(spreadDirection, targetTag);
            }
        }
    }
}
