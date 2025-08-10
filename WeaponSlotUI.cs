using UnityEngine;
using UnityEngine.UI;
using static G;

public class WeaponSlotUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    public Image WeaponImage;
    public short SlotId;

    private void Start()
    {
        panel.SetActive(false);
    }

    public void ShowHidePanel()
    {
        if (panel.activeSelf)
        {
            panel.SetActive(false);
        }else 
        {
            panel.SetActive(true); 
        }
    }

    private void DeleteWeapon()
    {
        WeaponManager.Instance.RemoveWeapon(SlotId);
        ShowHidePanel();
    }
    
    public void SellWeapon()
    {
        Debug.Log("+Money");
        DeleteWeapon();
    }

    public void MergeWeapon()
    {
        Debug.Log("+WeaponStrange");
        DeleteWeapon();
    }
    
    public void SetWeaponIcon(Rarity rarity, Sprite icon)
    {
        var image = GetComponent<Image>();

        image.color = GetRarityColor(rarity);
        WeaponImage.sprite = icon;
    }

    private Color32 GetRarityColor(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => new Color32(128, 128, 128, 175),
            Rarity.Uncommon => new Color32(0, 170, 35, 175),
            Rarity.Rare => new Color32(0, 65, 200, 175),
            Rarity.Epic => new Color32(115, 0, 115, 175),
            Rarity.Legendary => new Color32(255, 150, 0, 175),
            _ => new Color32(128, 128, 128, 75)
        };
    }
}