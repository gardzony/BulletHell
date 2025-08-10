using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

[Serializable]
public class GameState
{
    private static GameState _instance;

    public Player player;
    public int waveNumber;
    public EnemySpawner.Wave currentWave;
    public int playerLevel;
    public int money;

    public List<Bonus> bonuses;
    public List<Weapon> weapons;

    private GameState()
    {
        player = null;
        waveNumber = 0;
        playerLevel = 0;
        money = 0;
        currentWave = null;
        bonuses = new List<Bonus>();
        weapons = new List<Weapon>();
    }

    public static GameState Instance
    {
        get
        {
            if (_instance == null)
                _instance = new GameState();
            return _instance;
        }
    }

    public void SaveBonuses(List<Bonus> bonusesList)
    {
        bonuses.Clear();
        foreach(Bonus bonus in bonusesList)
        {
            bonuses.Add(bonus);
        }
    }

    public bool SaveToFile(string fileName = "gameState.json")
    {
        try
        {
            string json = JsonUtility.ToJson(this, true);
            string path = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllText(path, json);
            Debug.Log($"Игра сохранена в: {path}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка при сохранении: {e.Message}");
            return false;
        }
    }

    public static void LoadFromFile(string fileName = "gameState.json")
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        
        try
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                _instance = JsonUtility.FromJson<GameState>(json);
                if (_instance.bonuses == null)
                {
                    _instance.bonuses = new List<Bonus>();
                }
                Debug.Log("Состояние игры загружено");
            }
            else
            {
                Debug.Log("Файл сохранения не найден, используется стандартное состояние");
                _instance = new GameState();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка при загрузке: {e.Message}");
            _instance = new GameState();
        }
    }

    public void PrintState()
    {
        Debug.Log($"Player: {player.name}");
        Debug.Log($"Wave: {waveNumber}");
        Debug.Log($"Bonuses: {string.Join(", ", bonuses)}");
    }

    public void Reset()
    {
        player = null;
        waveNumber = 0;
        playerLevel = 0;
        money = 0;
        bonuses.Clear();
        weapons.Clear();
    }

    public void SaveFromGame(Player player, EnemySpawner.Wave wave, int money, int level, int waveNumber)
    {
        this.player = player;
        this.money = money;
        this.playerLevel = level;
        this.waveNumber = waveNumber;
        this.currentWave = wave;
        this.SaveBonuses(BonusManager.Instance.Bonuses);
        this.weapons.Clear();
        foreach (var weapon in WeaponManager.Instance.GetWeapons())
        {
            this.weapons.Add(weapon);
        }
    }
}
