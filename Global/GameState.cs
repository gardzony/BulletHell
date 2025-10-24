using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class GameState
{
    private static GameState _instance;

    public string PlayerPrefabName;
    public int WaveNumber;
    public int PlayerLevel;
    public int PlayerMoney;

    public List<string> Bonuses;
    public List<string> Weapons;

    public GameState()
    {
        PlayerPrefabName = null;
        WaveNumber = 0;
        PlayerLevel = 0;
        PlayerMoney = 0;
        Bonuses = new List<string>();
        Weapons = new List<string>();
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
}
