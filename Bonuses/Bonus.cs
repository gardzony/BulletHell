using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bonus", menuName = "Game/Bonuses/Base Bonus")]

public abstract class Bonus : ScriptableObject, ISellable
{
    [SerializeField] private Sprite bonusIcon;
    [SerializeField] private string description;
    [SerializeField] private G.Rarity rarity;
    [SerializeField] private int bonusPrice;
    public G.Rarity Rarity { get => rarity; set => rarity = value; }
    public Sprite Icon { get => bonusIcon; set => bonusIcon = value; }
    public string Name { get => name; }
    public string Description { get => description; set => description = value; }
    public int Price { get => bonusPrice; set => bonusPrice = value; }

    public virtual void Activate() { }
    public virtual void Deactivate() { }

}
