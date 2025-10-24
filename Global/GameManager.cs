using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Ссылки на основные компоненты")]
    public Player Player { get; private set; }
    public WeaponManager WeaponManager { get; private set; }
    public EnemySpawner EnemySpawner { get; private set; }

    [Header("UI и меню")]
    [SerializeField] private GameObject deathMenu;
    [SerializeField] private GameObject completedRoundMenu;
    [SerializeField] private GameObject gameStateUI;
    [SerializeField] private TextMeshProUGUI waveTimerText;
    [SerializeField] private TextMeshProUGUI waveIndexText;

    [Header("Текущее состояние игры")]
    public string PlayerName;
    public int CurrentWave { get; private set; }
    public int PlayerLevel { get; private set; }
    public int Money { get; private set; }
    public List<string> CurrentBonuses { get; private set; } = new List<string>();


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
        Player = FindFirstObjectByType<Player>();
        PlayerName = Player.Name;
    }

    private void Start()
    {
        WeaponManager = WeaponManager.Instance;
        EnemySpawner = FindFirstObjectByType<EnemySpawner>();

        if (EnemySpawner != null)
            EnemySpawner.OnWaveCompleted += HandleWaveCompleted;

        if (EnemySpawner != null)
            EnemySpawner.StartWave(CurrentWave);

        if (deathMenu != null) deathMenu.SetActive(false);
        if (completedRoundMenu != null) completedRoundMenu.SetActive(false);
        waveIndexText.text = "Волна: " + (CurrentWave + 1).ToString();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            if(Time.timeScale == 1)
            {
                Time.timeScale = 5;
            } else if (Time.timeScale != 0) 
            {
                Time.timeScale = 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.B)) TestLoadGame();
    }

    public void StartNextWave()
    {
        gameStateUI.SetActive(true);
        if (EnemySpawner != null)
        {
            CurrentWave++;
            EnemySpawner.StartWave(CurrentWave);
            if (completedRoundMenu != null) completedRoundMenu.SetActive(false);
            if (Player != null) Player.ResetPlayer();
            waveIndexText.text = "Волна: " + (CurrentWave + 1).ToString();
            Time.timeScale = 1f;
            if (CurrentWave > 0)
            {
                var gameState = new GameState
                {
                    PlayerPrefabName = PlayerName,
                    WaveNumber = CurrentWave,
                    PlayerLevel = PlayerLevel,
                    PlayerMoney = Money,
                    Bonuses = GetBonusesState(),
                    Weapons = GetWeaponsState()
                };
                SaveLoadService.SaveGame(gameState);
            }
        }
    }

    public void RestartWave()
    {
        Shop.Instance.WaveLoseShopUpdate();
        gameStateUI.SetActive(true);
        if (EnemySpawner != null)
            EnemySpawner.RestartWave();
        Time.timeScale = 1f;
        if (deathMenu != null) deathMenu.SetActive(false);
        if (Player != null) Player.ResetPlayer();
    }
    
    public void GameLose()
    {
        gameStateUI.SetActive(false);
        if (EnemySpawner != null)
            EnemySpawner.EndWave();
        ObjectPool.Instance.ReturnAllIbjectsToPool();
        Time.timeScale = 0f;
        if (deathMenu != null) deathMenu.SetActive(true);
    }

    public void TestLoadGame()
    {
        Time.timeScale = 0f;
        var gameState = SaveLoadService.LoadGame();
        Debug.Log(gameState.PlayerPrefabName);
        Debug.Log(gameState.Bonuses.Count);
        Debug.Log(gameState.Weapons.Count);
        Debug.Log(gameState.Weapons[0]);
        Debug.Log(gameState.Weapons[1]);
        Debug.Log(gameState.Weapons[2]);
    }

    public void WaveTimerUpdate(float waveTime)
    {
        int currentTime = Mathf.RoundToInt(waveTime);
        if (waveTimerText != null && int.Parse(waveTimerText.text) != currentTime && currentTime >= 0)
        {
            waveTimerText.text = currentTime.ToString();
        }
    }
    
    private void HandleWaveCompleted(int waveIndex)
    {
        Debug.Log($"Волна {waveIndex + 1} завершена!");
        ObjectPool.Instance.ReturnAllIbjectsToPool();
        if (completedRoundMenu != null) completedRoundMenu.SetActive(true);
        Time.timeScale = 0f;
        gameStateUI.SetActive(false);
    }
    
    private List<string> GetBonusesState()
    {
        var bonuses = new List<string>();
        var currentBonuses = BonusManager.Instance.GetBonuses();
        foreach (var bonus in currentBonuses)
        {
            bonuses.Add(bonus.Name);
        }
        return bonuses;
    }

    private List<string> GetWeaponsState()
    {
        var list = new List<string>();
        var currentWeapons = WeaponManager.Instance.GetWeapons();
        foreach (var weapon in currentWeapons)
        {
            list.Add(weapon.WeaponName + " " + weapon.Rarity.ToString());
        }
        return list;
    }
}