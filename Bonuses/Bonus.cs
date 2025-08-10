using UnityEngine;

[CreateAssetMenu(fileName = "New Bonus", menuName = "Game/Bonuses/Base Bonus")]

public abstract class Bonus : ScriptableObject
{
    public Sprite BonusIcon;
    public G.Rarity Rarity;

    public virtual void Activate() { }
    public virtual void Deactivate() { }
}
