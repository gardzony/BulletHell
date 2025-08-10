using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Bonus", menuName = "Game/Bonuses/Damage Bonus")]

public class DamageBonus : Bonus
{
    public float AdditionalDamage = 5;
    public G.DamageType DamageType = G.DamageType.Physical;

    public override void Activate()
    {
        BuffManager.Instance.UpdateAdditionalDamage(DamageType, AdditionalDamage);
    }

    public override void Deactivate()
    {
        BuffManager.Instance.UpdateAdditionalDamage(DamageType, -AdditionalDamage);
    }
}
