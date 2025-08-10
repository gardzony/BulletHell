using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    private Dictionary<GameObject, List<GameObject>> _objectPools;

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
        _objectPools = new Dictionary<GameObject, List<GameObject>>();
    }

    public void InitializePool(GameObject prefabExample, int count)
    {
        if (!_objectPools.ContainsKey(prefabExample))
        {
            _objectPools[prefabExample] = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(prefabExample);
                obj.SetActive(false);
                _objectPools[prefabExample].Add(obj);
            }
        }
    }
    public GameObject GetObjectFromPool(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("Префаб для пула не найден!");
            return null;
        }

        if (_objectPools.ContainsKey(prefab))
        {
            foreach (GameObject obj in _objectPools[prefab])
            {
                if (!obj.activeInHierarchy)
                {
                    return obj;
                }
            }
            GameObject newObject = Instantiate(prefab);
            newObject.SetActive(false);
            if (_objectPools.ContainsKey(prefab))
            {
                _objectPools[prefab].Add(newObject);
            } 
            return newObject;
        } else
        {
            InitializePool(prefab, 10);
            return GetObjectFromPool(prefab);
        }
    }

    public void ReturnObjectsToPool(GameObject prefabExample)
    {
        List<GameObject> objectList;

        if (_objectPools.ContainsKey(prefabExample))
        {
            objectList = _objectPools[prefabExample];
        } else return;

        foreach (var obj in objectList)
        {
            if(obj.TryGetComponent<CharacterEffects>(out var effects))
            {
                effects.ClearAllEffects();
            }
            if (obj.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.ResetEnemy();
            }
            ReturnObjectToPool(obj);
        }
        
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
    }
}
