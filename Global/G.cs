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
        Legendary
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