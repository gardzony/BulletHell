using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] public List<Wave> waves;
    [SerializeField] private float spawnInterval;
    [SerializeField] private Vector2 spawnAreaMin;
    [SerializeField] private Vector2 spawnAreaMax;
    [SerializeField] private float spawnWarningTime = 1f;
    [SerializeField] private GameObject warningPrefab;
    [SerializeField] private float baseWaveTime = 20f;

    private Wave _currentWave;
    private int _currentWaveIndex = -1;
    private float _spawnTimer;
    private float _waveTime;
    private bool _isSpawningStarted;
    private List<Warning> _activeWarnings = new List<Warning>();
    private GameObject _currentEnemyPrefab;
    private Player _player;
    
    public Wave CurrentWave { get { return _currentWave; } }

    public delegate void WaveCompletedHandler(int waveIndex);
    public event WaveCompletedHandler OnWaveCompleted;

    [System.Serializable]
    public class Wave
    {
        public List<EnemyType> enemies;
    }

    [System.Serializable]
    public class EnemyType
    {
        public GameObject prefab;
        public int weight;
    }

    void Start()
    {
        _waveTime = baseWaveTime;
        _player = GameManager.Instance.Player;
        foreach (Wave wave in waves)
        {
            foreach (EnemyType enemyType in wave.enemies)
            {
                ObjectPool.Instance.InitializePool(enemyType.prefab, 10);
            }
        }
    }

    void Update()
    {
        if (_currentWaveIndex < 0) return;

        if (_isSpawningStarted)
        {
            _spawnTimer -= Time.deltaTime;
            _waveTime -= Time.deltaTime;
            GameManager.Instance.WaveTimerUpdate(_waveTime);

            if (_spawnTimer <= 0)
            {
                PrepareSpawn();
                _spawnTimer = Mathf.Max(0.1f, spawnInterval + (Random.Range(-spawnInterval, spawnInterval) * 0.25f) - _currentWaveIndex / 50f);
            }
        }
        
        for (int i = _activeWarnings.Count - 1; i >= 0; i--)
        {
            Warning warning = _activeWarnings[i];
            warning.Timer -= Time.deltaTime;
            if (warning.Timer <= 0)
            {
                TrySpawnEnemy(warning);
                _activeWarnings.RemoveAt(i);
            }
        }
    }

    private void PrepareSpawn()
    {
        if (_currentWaveIndex < 0) return;

        

        if (_waveTime <= 0)
        {
            OnWaveCompleted?.Invoke(_currentWaveIndex);
            _isSpawningStarted = false;
            EndWave();
            return;
        } 
        else
        {
            TrySpawnWarning();
        }
    }

    private EnemyType GetRandomEnemyType(List<EnemyType> enemyTypes)
    {
        if (enemyTypes == null || enemyTypes.Count == 0)
        {
            Debug.LogWarning("Список enemyTypes пуст!");
            return null;
        }

        int totalWeight = 0;
        foreach (EnemyType type in enemyTypes)
        {
            totalWeight += type.weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        foreach (EnemyType type in enemyTypes)
        {
            if (randomValue < type.weight)
            {
                return type;
            }
            randomValue -= type.weight;
        }
        return enemyTypes[enemyTypes.Count - 1];
    }

    private void TrySpawnEnemy(Warning warning)
    {
        int playerLayerMask = 1 << LayerMask.NameToLayer("Player");
        if (!Physics2D.OverlapCircle(warning.Position, 0.5f, playerLayerMask))
        {
            _currentEnemyPrefab = warning.EnemyPrefab;
            GameObject enemy = ObjectPool.Instance.GetObjectFromPool(_currentEnemyPrefab);
            if (enemy != null)
            {
                enemy.transform.position = warning.Position;
                enemy.GetComponent<Enemy>().IsEnemyDead = false;
                enemy.GetComponent<Enemy>().DifficultAdoptation(_currentWaveIndex);
                enemy.SetActive(true);
            }
        }
        if (warning.WarningObject != null)
        {
            Destroy(warning.WarningObject);
        }
    }

    private void TrySpawnWarning()
    {
        EnemyType enemyType = GetRandomEnemyType(_currentWave.enemies);
        if (enemyType == null) return;

        Vector3 spawnPosition;
        do
        {
            spawnPosition = GetRandomSpawnPosition();
        } while (Vector2.Distance(spawnPosition, _player.transform.position) < 2);

        GameObject warningObject = Instantiate(warningPrefab, spawnPosition, Quaternion.identity);
        _activeWarnings.Add(new Warning(warningObject, spawnPosition, spawnWarningTime, enemyType.prefab));
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        return new Vector3(x, y, 0);
    }

    public void StartWave(int waveIndex)
    {
        if (waveIndex < 0)
        {
            Debug.LogWarning("Некорректный индекс волны!");
            return;
        }

        //_currentWave = waves[Random.Range(0, waves.Count)];
        _currentWave = waves[0];
        _currentWaveIndex = waveIndex;
        RefreshEnemiesWeight(_currentWave);
        _waveTime = CalculateCurrentWaveTime();
        _isSpawningStarted = true;
        _spawnTimer = spawnInterval;
        Debug.Log($"Запуск волны {waveIndex + 1}");
    }

    public void RestartWave()
    {
        _waveTime = CalculateCurrentWaveTime();
        _isSpawningStarted = true;
        _spawnTimer = spawnInterval;
        Debug.Log($"Перезапуск волны.");
    }

    public void EndWave()
    {
        foreach (var warning in _activeWarnings)
        {
            Destroy(warning.WarningObject);
        }
        _activeWarnings.Clear();

        foreach (EnemyType enemyType in _currentWave.enemies)
        {
            ObjectPool.Instance.ReturnObjectsToPool(enemyType.prefab);
        }
    }

    private float CalculateCurrentWaveTime()
    {
        if ((_currentWaveIndex + 1) % 5 == 0)
        {
            return 60f;
        }
        return Mathf.Clamp(baseWaveTime + (5 * _currentWaveIndex), 20, 45);
    }

    private void RefreshEnemiesWeight(Wave wave)
    {
        foreach(var enemy in wave.enemies)
        {
            enemy.weight += 5;
        }
    }

    private class Warning
    {
        public GameObject WarningObject;
        public Vector3 Position;
        public float Timer;
        public GameObject EnemyPrefab;

        public Warning(GameObject warningObject, Vector3 position, float timer, GameObject enemyPrefab)
        {
            this.WarningObject = warningObject;
            this.Position = position;
            this.Timer = timer;
            this.EnemyPrefab = enemyPrefab;
        }
    }
}