using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Coeff Bonus", menuName = "Game/Bonuses/Damage Coeff Bonus")]

public class DamageCoeffBonus : Bonus
{
    public float CoeffDamage = 0.2f;
    public G.DamageType DamageType = G.DamageType.Physical;

    public override void Activate()
    {
        BuffManager.Instance.UpdateCoeffDamage(DamageType, CoeffDamage);
    }

    public override void Deactivate()
    {
        BuffManager.Instance.UpdateCoeffDamage(DamageType, -CoeffDamage);
    }
}