using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager Instance { get; private set; }

    public Dictionary<G.DamageType, float> DamageAdditionalBuffs;
    public Dictionary<G.DamageType, float> DamageCoeffBuffs;

    [NonSerialized] public float SpeedBuff;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        DamageAdditionalBuffs = Enum.GetValues(typeof(G.DamageType)).Cast<G.DamageType>().ToDictionary(type => type, type => 0f);
        DamageCoeffBuffs = Enum.GetValues(typeof(G.DamageType)).Cast<G.DamageType>().ToDictionary(type => type, type => 0f);
    }

    public void UpdateAdditionalDamage(G.DamageType type, float damage)
    {
        foreach (var key in DamageAdditionalBuffs.Keys.ToList())
        {
            if (key == type)
            {
                DamageAdditionalBuffs[key] += damage;
                return;
            }
        }
    }

    public void UpdateCoeffDamage(G.DamageType type, float damageCoeff)
    {
        foreach (var key in DamageCoeffBuffs.Keys.ToList())
        {
            if (key == type)
            {
                DamageCoeffBuffs[key] += damageCoeff;
                return;
            }
        }
    }
}
