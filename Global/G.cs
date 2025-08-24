using System;
using UnityEngine;

public static class G
{
    public enum DamageType
    {
        Physical,
        Fire,
        Poison,
        Heavy,
        Pure
    }

    public enum OperationType
    {
        Encreas,
        Decrease
    }

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Default
    }
    public static Rarity GetRandomRarity()
    {
        float rand = UnityEngine.Random.value;
        if (rand < 0.5f) return Rarity.Common;
        if (rand < 0.8f) return Rarity.Uncommon;
        if (rand < 0.95f) return Rarity.Rare;
        if (rand < 0.99f) return Rarity.Epic;
        return Rarity.Legendary;
    }

    public static Color32 GetRarityColor(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => new Color32(128, 128, 128, 175),
            Rarity.Uncommon => new Color32(0, 170, 35, 175),
            Rarity.Rare => new Color32(0, 65, 200, 175),
            Rarity.Epic => new Color32(115, 0, 115, 175),
            Rarity.Legendary => new Color32(255, 150, 0, 175),
            _ => new Color32(128, 128, 128, 75)
        };
    }

    public static int GetItemPriceByRarityWaveIndex(int basePrice, Rarity rarity, int waveIndex)
    {
        int price = basePrice + (basePrice * (int)rarity / 2) + (basePrice * (waveIndex) / 10);
        return price;
    }

    public static int GetItemPriceByWaveIndex(int basePrice, int waveIndex)
    {
        int price = basePrice + basePrice * waveIndex / 10;
        return price;
    }
}

/*
 ========Интерфейсы========
 */

public interface IEffectable
{
    public ParticleSystem PoisonParticleSystem { get; }
    public void SpeedMultiplierChange(float speedMultiplier, G.OperationType operationType);
}

public interface IDamageable
{
    public void TakeDamage(float damage, G.DamageType damageType);
}

public interface IPoolable
{
    public void InitializePool();
    public GameObject GetObjectFromPool();
    public void ReturnObjectToPool(GameObject target);
    public void ReturnAllObjectsToPool();
}

public interface ISellable
{
    public G.Rarity Rarity { get; set; }
    public Sprite Icon { get; set; }
    public string Name { get;}
    public string Description { get; set; }
    public int Price { get; set; }

}