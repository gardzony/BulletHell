using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static G;

public class WeaponSlotUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button mergeButton;
    public Image WeaponImage;
    public short SlotId;

    private Weapon _availibleToMergeWeapon;
    private int _availibleToMergeId;

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
            MergeButtonInteractableCheck();
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
        var weapon = WeaponManager.Instance.GetWeapon(SlotId);
        if (weapon is ISellable sellable)
        {
            Shop.Instance.IncreasePlayerMoney(sellable.Price / 2);
            DeleteWeapon();
        }
    }

    public void MergeWeapon()
    {
        if(SlotId < _availibleToMergeId)
        {
            DeleteWeapon();
            _availibleToMergeId -= 1;
        } else
        {
            DeleteWeapon();
        }
        _availibleToMergeWeapon.transform.rotation = Quaternion.identity;
        WeaponManager.Instance.AddWeapon(_availibleToMergeWeapon, _availibleToMergeWeapon.Rarity + 1);
        WeaponManager.Instance.RemoveWeapon(_availibleToMergeId);
    }

    public void SetWeaponIcon(Rarity rarity, Sprite icon)
    {
        var image = GetComponent<Image>();
        image.color = GetRarityColor(rarity);
        WeaponImage.sprite = icon;
    }

    public void MergeButtonInteractableCheck()
    {
        mergeButton.interactable = false;
        if(MergeAvailableCheck()) mergeButton.interactable = true;
        
    }

    private bool MergeAvailableCheck()
    {
        var tmp = WeaponManager.Instance.GetWeapons();
        mergeButton.interactable = false;
        if (tmp[SlotId].Rarity == Rarity.Legendary) return false;

        for (int i = 0; i < tmp.Count; i++)
        {
            if (i != SlotId)
            {
                if (tmp[i].WeaponName == tmp[SlotId].WeaponName && tmp[i].Rarity == tmp[SlotId].Rarity)
                {
                    mergeButton.interactable = true;
                    _availibleToMergeWeapon = tmp[i];
                    _availibleToMergeId = i;
                    return true;
                }
            }
        }
        return false;
    }
}