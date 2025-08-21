using UnityEngine;

public class Bullet : BaseBullet
{
    public void SetDamage(float damageMultiplier)
    {
        damage = (baseDamage + baseDamage * BuffManager.Instance.DamageBuffs[damageType]) * damageMultiplier;
        Debug.Log(damage);
    }
}