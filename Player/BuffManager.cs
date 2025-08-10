using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager Instance { get; private set; }

    public Dictionary<G.DamageType, float> DamageBuffs;

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
        DamageBuffs = Enum.GetValues(typeof(G.DamageType)).Cast<G.DamageType>().ToDictionary(type => type, type => 0f);
    }

    public void UpdateAdditionalDamage(G.DamageType type, float damage)
    {
        foreach (var key in DamageBuffs.Keys.ToList())
        {
            if (key == type)
            {
                DamageBuffs[key] += damage;
                return;
            }
        }
    }
}
