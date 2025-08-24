using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour, ISellable
{
    [SerializeField] protected string targetTag;
    [SerializeField] protected float attackRange = 5f;
    [SerializeField] protected string description;
    [SerializeField] protected G.Rarity weaponRarity;
    [SerializeField] protected Sprite weaponImage;
    [SerializeField] protected int weaponPrice;

    protected float damageMultiplier;
    public string WeaponName;

    public G.Rarity Rarity { get => weaponRarity; set => weaponRarity = value; }
    public Sprite Icon { get => weaponImage; set => weaponImage = value; }
    public string Name { get => name; }
    public string Description { get => description; set => description = value; }
    public int Price { get => weaponPrice; set => weaponPrice = value; }

    protected virtual void Start()
    {
        
    }

    protected virtual List<Collider2D> FindEnemiesInAttackRange()
    {
        var hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Enemy")).ToList();

        return hitEnemies;
    }

    protected virtual void Attack(GameObject target) { }

    public void SetWeaponRarity(G.Rarity rarity)
    {
        Rarity = rarity;
        SetDamageMultiplier();
        SetWeaponPriceByRarity();
    }

    protected void SetDamageMultiplier()
    {
        damageMultiplier = 1 + (int)weaponRarity / 5.0f;
        Debug.Log("DamageMultiplaire " + damageMultiplier);
    }

    protected void SetWeaponPriceByRarity()
    {
        weaponPrice = G.GetItemPriceByRarityWaveIndex(weaponPrice, weaponRarity, GameManager.Instance.CurrentWave);
    }
}
