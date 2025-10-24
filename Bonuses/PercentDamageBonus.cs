using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Coeff Bonus", menuName = "Game/Bonuses/Damage Coeff Bonus")]

public class PercentDamageBonus : Bonus
{
    public float CoeffDamage = 0.2f;
    public G.DamageType DamageType = G.DamageType.Physical;

    public override void Activate()
    {
        BuffManager.Instance.UpdatePercentDamage(DamageType, CoeffDamage);
    }

    public override void Deactivate()
    {
        BuffManager.Instance.UpdatePercentDamage(DamageType, -CoeffDamage);
    }
}