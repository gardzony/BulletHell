using System;
using System.Collections.Generic;
using UnityEngine;
using static G;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

   
    [SerializeField] private List<Transform> weaponAttachPoints = new List<Transform>();    
    
    private List<Weapon> _weaponList = new List<Weapon>();

    private int _currentCount;
    public int MaxWeaponCount = 6;

    public event Action OnAddRemoveWeapon;

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

    private void Start()
    {
        MaxWeaponCount = GameManager.Instance.Player.MaxWeaponCount;
    }

    public bool AddWeapon(Weapon weaponPrefab, Rarity rarity = Rarity.Default)
    {
        if(rarity == Rarity.Default)
        {
            rarity = weaponPrefab.Rarity;
        }

        if (_currentCount >= MaxWeaponCount)
        {
            Debug.LogWarning("Оружейные слоты заполнены!");
            return false;
        }

        int slotIndex = _currentCount;

        Weapon spawnedWeapon = Instantiate(weaponPrefab, 
            weaponAttachPoints[slotIndex].position,
            Quaternion.Euler(0, 0, 0),
            weaponAttachPoints[slotIndex]);
        spawnedWeapon.transform.localRotation = Quaternion.Euler(0, 0, 0);
        spawnedWeapon.transform.localScale = new Vector3(1, 1, 1);
        spawnedWeapon.SetWeaponRarity(rarity);
        spawnedWeapon.gameObject.SetActive(true);

        _weaponList.Add(spawnedWeapon);
        _currentCount++;

        OnAddRemoveWeapon?.Invoke();
        return true;
    }

    public bool RemoveWeapon(int index)
    {
        if (index < 0 || index >= _currentCount)
        {
            Debug.LogWarning("Неверный индекс оружия!");
            return false;
        }

        if (_weaponList[index] != null)
        {
            Destroy(_weaponList[index].gameObject);
            _weaponList.Remove(_weaponList[index]);
            _currentCount--;
        }

        for (int i = 0; i < _currentCount; i++)
        {
            if (_weaponList[i] != null)
            {
                _weaponList[i].transform.SetParent(weaponAttachPoints[i]);
                _weaponList[i].transform.position = weaponAttachPoints[i].position;
                _weaponList[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
                _weaponList[i].transform.localScale = new Vector3(1, 1, 1);
            }
        }

        OnAddRemoveWeapon?.Invoke();
        return true;
    }
    
    public void RemoveAllWeapons()
    {
        _weaponList.Clear();
        OnAddRemoveWeapon?.Invoke();
    }

    public Weapon GetWeapon(int index)
    {
        if (index < 0 || index >= _currentCount)
        {
            return null;
        }
        return _weaponList[index];
    }

    public int GetWeaponCount () => _currentCount;

    public bool HasWeapon (Weapon weapon) => _weaponList.Contains(weapon);
    
    public List<Weapon> GetWeapons() => _weaponList;
}

