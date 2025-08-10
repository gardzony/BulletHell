using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private Weapon pistol;
    [SerializeField] private Weapon shotgun;
    [SerializeField] private List<GameObject> weaponListUI;
    private List<Weapon> currentWeapons = new List<Weapon>();

    void Start()
    {
        WeaponManager.Instance.AddWeapon(pistol);
        FastCheck();
        WeaponManager.Instance.OnAddRemoveWeapon += FastCheck;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            WeaponManager.Instance.AddWeapon(pistol);
            WeaponManager.Instance.AddWeapon(shotgun);
        }
    }

    private void CheckWEaponCount()
    {
        currentWeapons = WeaponManager.Instance.GetWeapons();
    }

    public void PrintWeaponLayoutGrid()
    {
        foreach (var image in weaponListUI)
        {
            image.SetActive(false);
        }

        int i = 0;

        foreach (var weapon in currentWeapons)
        {
            var weaponSlot = weaponListUI[i].GetComponent<WeaponSlotUI>();
            weaponSlot.SetWeaponIcon(currentWeapons[i].WeaponRarity, currentWeapons[i].WeaponImage);
            weaponListUI[i].SetActive(true);
            i++;
        }
    }

    public void FastCheck()
    {
        CheckWEaponCount();
        PrintWeaponLayoutGrid();
    }

    private void OnDestroy()
    {
        WeaponManager.Instance.OnAddRemoveWeapon -= FastCheck;
    }
}
