using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static G;
using Random = UnityEngine.Random;

public class Shop : MonoBehaviour
{
    [SerializeField] private Weapon pistol;
    [SerializeField] private Weapon shotgun;
    [SerializeField] private List<GameObject> weaponListUI;
    [SerializeField] private List<GameObject> sellableItemsListMono;
    [SerializeField] private List<ScriptableObject> sellableItemsListSO;
    [SerializeField] private List<TextMeshProUGUI> moneyTextsTMP;

    [SerializeField] TextMeshProUGUI rerollCostText;
    [SerializeField] Button rerollButton;

    private Dictionary<Rarity, List<ISellable>> _sellableItemsDicttionary;

    private int _rerollCost = 10;
    private ShopUI _shopUI;
    private List<Weapon> _currentWeapons = new List<Weapon>();
    private List<ISellable> _currentSellable = new List<ISellable>();
    private List<Rarity> _selectedRarityList;

    public int PlayerMoney = 100;

    public static Shop Instance { get; private set; }


    public event Action OnMoneyChange;

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
        _shopUI = GetComponent<ShopUI>();
        StartCoroutine(ConvertItemListsToDictionary());
        WeaponManager.Instance.AddWeapon(pistol);
        FastCheck();
        WeaponManager.Instance.OnAddRemoveWeapon += FastCheck;
        rerollCostText.text = _rerollCost.ToString();
        _selectedRarityList = new List<Rarity>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            WeaponManager.Instance.AddWeapon(pistol);
            WeaponManager.Instance.AddWeapon(shotgun, Rarity.Rare);
        }
    }

    private void CheckWEaponCount()
    {
        _currentWeapons = WeaponManager.Instance.GetWeapons();
    }

    public void PrintWeaponLayoutGrid()
    {
        foreach (var image in weaponListUI)
        {
            image.SetActive(false);
        }

        int i = 0;

        foreach (var weapon in _currentWeapons)
        {
            var weaponSlot = weaponListUI[i].GetComponent<WeaponSlotUI>();
            weaponSlot.SetWeaponIcon(_currentWeapons[i].Rarity, _currentWeapons[i].Icon);
            weaponListUI[i].SetActive(true);
            i++;
        }
    }

    public void FastCheck()
    {
        CheckWEaponCount();
        PrintWeaponLayoutGrid();
        UpdateShopUI();
    }

    private void OnDestroy()
    {
        WeaponManager.Instance.OnAddRemoveWeapon -= FastCheck;
    }

    private IEnumerator ConvertItemListsToDictionary()
    {
        _sellableItemsDicttionary = Enum.GetValues(typeof(G.Rarity)).Cast<G.Rarity>().ToDictionary(type => type, type => new List<ISellable>());

        foreach (var item in sellableItemsListMono)
        {
            var tmp = item.GetComponent<ISellable>();
            _sellableItemsDicttionary[tmp.Rarity].Add(tmp);
            yield return null;
        }
        foreach (var item in sellableItemsListSO)
        {
            if (item is ISellable so)
            {
                var tmp = so;
                _sellableItemsDicttionary[tmp.Rarity].Add(tmp);
                yield return null;
            }
        }
        Debug.Log(_sellableItemsDicttionary[G.Rarity.Common].Count);
    }

    public List<ISellable> GetRandomCardsItems(out List<Rarity> selectedRarityListTmp)
    {
        _selectedRarityList.Clear();
        _currentSellable.Clear();
        selectedRarityListTmp = new List<Rarity>();

        while (_currentSellable.Count < 3)
        {
            var availableItems = new Dictionary<Rarity, List<ISellable>>();
            foreach (var pair in _sellableItemsDicttionary)
            {
                availableItems[pair.Key] = new List<ISellable>(pair.Value);
            }

            Rarity selectedRarity = GetRandomRarity();
            selectedRarityListTmp.Add(selectedRarity);
            Debug.Log(selectedRarity);

            var eligibleItems = availableItems
                .Where(kvp => kvp.Key == selectedRarity)
                .SelectMany(kvp => kvp.Value)
                .ToList();
            var weaponLowerKey = availableItems
                .Where(kvp => kvp.Key < selectedRarity)
                .SelectMany(kvp => kvp.Value)
                .Where(item => item is Weapon)
                .ToList();
            eligibleItems.AddRange(weaponLowerKey);

            if (eligibleItems.Count == 0)
            {
                Debug.LogWarning($"No items available for rarity {selectedRarity}");
                selectedRarityListTmp.Remove(selectedRarityListTmp[_currentSellable.Count]);
                continue;
            }
            ISellable selectedItem = eligibleItems[Random.Range(0, eligibleItems.Count)];

            _currentSellable.Add(selectedItem);
        }
        _selectedRarityList.AddRange(selectedRarityListTmp);
        return _currentSellable;
    }

    public bool BuyItem(int cardId)
    {
        bool flag = false;
        if(_currentSellable[cardId] is Weapon weapon) 
        {
            if (WeaponManager.Instance.GetWeaponCount() < WeaponManager.Instance.MaxWeaponCount)
            {
                WeaponManager.Instance.AddWeapon(weapon, _selectedRarityList[cardId]);
                PlayerMoney -= _currentSellable[cardId].Price;
                flag = true;
            }
            else if (MergeAvailableCheck(weapon, _selectedRarityList[cardId])) 
            { 
                WeaponManager.Instance.AddWeapon(weapon, _selectedRarityList[cardId] + 1);
                PlayerMoney -= _currentSellable[cardId].Price;
                flag = true;
            }
        } else if(_currentSellable[cardId] is Bonus bonus)
        {
            BonusManager.Instance.AddBonus(bonus);
            PlayerMoney -= _currentSellable[cardId].Price;
            flag = true;
        }
        UpdateShopUI();
        OnMoneyChange?.Invoke();
        return flag;
    }

    public void IncreasePlayerMoney(int money)
    {
        PlayerMoney += money;
        OnMoneyChange?.Invoke();
        UpdateShopUI();
    }

    public void RerollShopCards()
    {
        PlayerMoney -= _rerollCost;
        _shopUI.ShopReroll();
        UpdateShopUI();
    }

    private void UpdateShopUI()
    {
        CurrentMoneyUpdateUI();
        CheckRerollAvailable();
    }

    private void CurrentMoneyUpdateUI()
    {
        foreach (var item in moneyTextsTMP)
        {
            item.text = PlayerMoney.ToString();
        }
    }

    private void CheckRerollAvailable()
    {
        if (PlayerMoney < _rerollCost)
        {
            rerollButton.interactable = false;
        }
        else rerollButton.interactable = true;
    }

    private bool MergeAvailableCheck(Weapon weapon, Rarity rarity)
    {
        if (rarity == Rarity.Legendary) return false;
        var tmp = WeaponManager.Instance.GetWeapons();
        for (int i = 0; i < tmp.Count; i++)
        {
            if (tmp[i].WeaponName == weapon.WeaponName && tmp[i].Rarity == rarity)
            {
                Debug.Log("Check");
                WeaponManager.Instance.RemoveWeapon(i);
                return true;
            }
        }
        return false;
    }
}

