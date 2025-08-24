using UnityEngine;

public class Bullet : BaseBullet
{
    public void SetDamage(float damageMultiplier)
    {
        var buffManager = BuffManager.Instance;
        damage = (baseDamage + buffManager.DamageAdditionalBuffs[damageType]) * damageMultiplier * (buffManager.DamageCoeffBuffs[damageType] + 1);
        Debug.Log(damage);
    }
}