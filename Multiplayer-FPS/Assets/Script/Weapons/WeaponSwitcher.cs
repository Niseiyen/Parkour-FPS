using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    private int selectedWeapon = 0;

    private void Start()
    {
        SelectWeapon();
    }

    private void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        if (IsWeaponBusy()) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeapon = 1;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon++;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (selectedWeapon <= 0)
            {
                selectedWeapon = transform.childCount - 1;
            }
            else
            {
                selectedWeapon--;
            }
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    private bool IsWeaponBusy()
    {
        foreach (Transform weaponTransform in transform)
        {
            Weapon weapon = weaponTransform.GetComponent<Weapon>();
            if (weapon != null && weapon.isBusy)
            {
                return true;
            }
        }
        return false;
    }

    public void SelectWeapon()
    {
        int i = 0;
        foreach (Transform _weapon in transform)
        {
            Weapon weapon = _weapon.GetComponent<Weapon>();

            if (i == selectedWeapon)
            {
                _weapon.gameObject.SetActive(true);

                if (weapon != null)
                {
                    weapon.InitializeWeapon();
                    weapon.UpdateWeaponUI();
                }
            }
            else
            {
                if (weapon != null)
                {
                    weapon.transform.localPosition = weapon.originalPosition;
                }
                _weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }


}
