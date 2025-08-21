using UnityEngine;
using UnityEngine.UI;
using static G;
using System.Linq;

public class BonusUIManager : MonoBehaviour
{
    [SerializeField] private Transform bonusesGrid;
    [SerializeField] private GameObject bonusUIPrefab;
    private bool _isActive = false;
    private void Start()
    {
        BonusManager.Instance.OnBonusResearch += CreateBonusIcons;
        ObjectPool.Instance.InitializePool(bonusUIPrefab, 10);
    }

    private void LateUpdate()
    {
        if (_isActive == false && bonusesGrid.gameObject.activeInHierarchy) 
        { 
            CreateBonusIcons();
            _isActive = true;
        }
        if (_isActive && bonusesGrid.gameObject.activeInHierarchy == false)
        {
            _isActive = false;
        }
    }

    public void CreateBonusIcons()
    {
        foreach (Transform child in bonusesGrid)
        {
            child.gameObject.SetActive(false);
            ObjectPool.Instance.ReturnObjectToPool(child.gameObject);
        }

        var sortedBonuses = BonusManager.Instance.Bonuses
        .OrderByDescending(b => b.Rarity)
        .ToList();

        foreach (var bonus in sortedBonuses)
        {
            var bonusUI = ObjectPool.Instance.GetObjectFromPool(bonusUIPrefab);
            bonusUI.SetActive(true);
            SetupBonusUI(bonusUI, bonus);
            bonusUI.transform.SetParent(bonusesGrid);
        }
    }

    private void SetupBonusUI(GameObject uiElement, Bonus bonusData)
    {
        var icon = uiElement.transform.GetChild(0).GetComponent<Image>();
        var rarityBG = uiElement.GetComponent<Image>();

        icon.sprite = bonusData.Icon;
        rarityBG.color = GetRarityColor(bonusData.Rarity);
    }

    

    private void OnDestroy()
    {
        BonusManager.Instance.OnBonusResearch -= CreateBonusIcons;
    }
}