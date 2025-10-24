using System;
using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
    public static BonusManager Instance { get; private set; }

    public List<Bonus> Bonuses = new List<Bonus>();

    [SerializeField] private Bonus testBonus;
    [SerializeField] private Bonus testBonus2;
    [SerializeField] private Bonus testBonus3;

    public event Action OnBonusResearch;

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

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.H)) AddBonus(testBonus);
        if (Input.GetKeyUp(KeyCode.P)) AddBonus(testBonus2);
        if (Input.GetKeyUp(KeyCode.M)) AddBonus(testBonus3);
        if (Input.GetKeyUp(KeyCode.V)) RemoveBonus(testBonus);
    }

    public void AddBonus(Bonus bonus)
    {
        Bonuses.Add(bonus);
        bonus.Activate();
        OnBonusResearch?.Invoke();
    }

    public void RemoveBonus(Bonus bonus)
    {
        if(!Bonuses.Contains(bonus)) return;
        Bonuses.Remove(bonus);
        bonus.Deactivate();
        OnBonusResearch?.Invoke();
    }

    public List<Bonus> GetBonuses()
    {
        return Bonuses;
    }
}