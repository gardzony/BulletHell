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
    [SerializeField] private int baseRerollCost = 1;
    [SerializeField] TextMeshProUGUI rerollCostText;
    [SerializeField] Button rerollButton;

    private Dictionary<Rarity, List<ISellable>> _sellableItemsDicttionary;

    private int _currentWaveMoneyIncome;
    private GameManager _gameManager;
    private WeaponManager _weaponManager;
    private int _rerollCost = 1;
    private ShopUI _shopUI;
    private List<Weapon> _currentWeapons = new List<Weapon>();
    private ISellable[] _currentSellable = new ISellable[3];
    private List<ShopItem> _chosenItems;
    private Rarity[] _chosenRarity = new Rarity[3];
    private bool[] _cardsLockState = new bool[3];
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
        _gameManager = GameManager.Instance;
        _gameManager.EnemySpawner.OnWaveCompleted += WaveUpdate;
        _weaponManager = _gameManager.WeaponManager;
        _weaponManager.AddWeapon(pistol);
        FastCheck();
        _weaponManager.OnAddRemoveWeapon += FastCheck;
        _rerollCost = baseRerollCost;
        rerollCostText.text = _rerollCost.ToString();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _weaponManager.AddWeapon(pistol);
            _weaponManager.AddWeapon(shotgun, Rarity.Legendary);
        }
    }

    private void CheckWEaponCount()
    {
        _currentWeapons = _weaponManager.GetWeapons();
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
        _weaponManager.OnAddRemoveWeapon -= FastCheck;
        _gameManager.EnemySpawner.OnWaveCompleted -= WaveUpdate;
    }

    private IEnumerator ConvertItemListsToDictionary()
    {
        _sellableItemsDicttionary = Enum.GetValues(typeof(Rarity)).Cast<Rarity>().ToDictionary(type => type, type => new List<ISellable>());

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
        Debug.Log(_sellableItemsDicttionary[Rarity.Common].Count);
    }

    public List<ShopItem> GetRandomCardsItems()
    {
        int itemsCount = 0;
        _cardsLockState = GetComponent<ShopUI>().GetCardsLockState();
        foreach (var item in _currentSellable)
        {
            if (item != null) itemsCount++;
        }

        if (itemsCount > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                if (!_cardsLockState[i])
                {
                    _currentSellable[i] = null;
                    itemsCount--;
                }
            }
        }

        if(itemsCount > 0)
        {
            for(int i = 0; i < 3; i++)
            {
                if(_currentSellable[i] != null) _chosenRarity[i] = _chosenItems[i].ItemRarity;
            }
        }
        
        while (itemsCount < 3)
        {
            var currentIndex = 0;
            for (int i = 0; i < 3; i++)
            {
                if (_currentSellable[i] == null) currentIndex = i;
            }
            var availableItems = new Dictionary<Rarity, List<ISellable>>();
            foreach (var pair in _sellableItemsDicttionary)
            {
                availableItems[pair.Key] = new List<ISellable>(pair.Value);
            }

            Rarity selectedRarity = _chosenRarity[currentIndex];
            if (!_cardsLockState[currentIndex])
            {
                selectedRarity = GetRandomRarity();
                _chosenRarity[currentIndex] = selectedRarity;
            }

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
                continue;
            }
            ISellable selectedItem = eligibleItems[Random.Range(0, eligibleItems.Count)];
            _currentSellable[currentIndex] = selectedItem;
            itemsCount++;
        }

        _chosenItems = ConvertISellableToShopItems(_currentSellable, _chosenRarity);

        return _chosenItems;
    }

    public bool BuyItem(int cardId)
    {
        bool flag = false;
        if (_currentSellable[cardId] is Weapon weapon)
        {
            if (_weaponManager.GetWeaponCount() < _weaponManager.MaxWeaponCount)
            {
                _weaponManager.AddWeapon(weapon, _chosenItems[cardId].ItemRarity);
                IncreasePlayerMoney(-_chosenItems[cardId].ItemPrice);
                flag = true;
            }
            else if (MergeAvailableCheck(weapon, _chosenItems[cardId].ItemRarity))
            {
                _weaponManager.AddWeapon(weapon, _chosenItems[cardId].ItemRarity + 1);
                IncreasePlayerMoney(-_chosenItems[cardId].ItemPrice);
                flag = true;
            }
        } else if (_currentSellable[cardId] is Bonus bonus)
        {
            BonusManager.Instance.AddBonus(bonus);
            IncreasePlayerMoney(-_chosenItems[cardId].ItemPrice);
            flag = true;
        }
        return flag;
    }

    public void IncreasePlayerMoney(int money)
    {
        PlayerMoney += money;
        if(money > 0 && Time.timeScale > 0) _currentWaveMoneyIncome += money;
        OnMoneyChange?.Invoke();
        UpdateShopUI();
    }

    public void RerollShopCards()
    {
        PlayerMoney -= _rerollCost;
        _rerollCost *= 2;
        _shopUI.ShopReroll();
        UpdateShopUI();
    }

    private void UpdateShopUI()
    {
        CurrentMoneyUpdateUI();
        CheckRerollAvailable();
        rerollCostText.text = _rerollCost.ToString();
    }

    private void CurrentMoneyUpdateUI()
    {
        foreach (var item in moneyTextsTMP)
        {
            item.text = PlayerMoney.ToString();
        }
    }

    public void WaveLoseShopUpdate()
    {
        PlayerMoney -= _currentWaveMoneyIncome;
        _currentWaveMoneyIncome = 0;
        OnMoneyChange?.Invoke();
        UpdateShopUI();
    }

    private void CheckRerollAvailable()
    {
        if (PlayerMoney < _rerollCost)
        {
            rerollButton.interactable = false;
        }
        else rerollButton.interactable = true;
    }

    public void WaveUpdate(int waveIndex)
    {
        CalculateRerollPrice(waveIndex);
        _currentWaveMoneyIncome = 0;
    }

    public void CalculateRerollPrice(int waveIndex)
    {
        _rerollCost = GetItemPriceByWaveIndex(baseRerollCost, waveIndex * 10);
        rerollCostText.text = _rerollCost.ToString();
    }

    private bool MergeAvailableCheck(Weapon weapon, Rarity rarity)
    {
        if (rarity == Rarity.Legendary) return false;
        var tmp = _weaponManager.GetWeapons();
        for (int i = 0; i < tmp.Count; i++)
        {
            if (tmp[i].WeaponName == weapon.WeaponName && tmp[i].Rarity == rarity)
            {
                Debug.Log("Check");
                _weaponManager.RemoveWeapon(i);
                return true;
            }
        }
        return false;
    }

    private List<ShopItem> ConvertISellableToShopItems(ISellable[] sellablesList, Rarity[] chosenRarity)
    {
        var tmp = new List<ShopItem>();
        int i = 0;
        foreach (var item in sellablesList)
        {
            ShopItem shopItem = new ShopItem();
            shopItem.ItemName = item.Name;
            shopItem.ItemSprite = item.Icon;
            shopItem.ItemRarity = chosenRarity[i];
            i++;
            if (item is Weapon) shopItem.ItemPrice = GetItemPriceByRarityWaveIndex(item.Price, shopItem.ItemRarity, _gameManager.CurrentWave);
            else shopItem.ItemPrice = GetItemPriceByWaveIndex(item.Price, _gameManager.CurrentWave);

            shopItem.ItemDescription = item.Description;

            tmp.Add(shopItem);
        }
        return tmp;
    }
}