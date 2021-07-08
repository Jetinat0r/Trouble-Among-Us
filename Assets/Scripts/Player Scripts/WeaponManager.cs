using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    private PlayerShoot playerShoot;
    [SerializeField]
    private PlayerUI playerUI;
    [SerializeField]
    private List<Weapon> weapons;
    
    [SerializeField]
    private Player player;
    private int currentGun = 0;

    private void Update()
    {
        if (player.isAlive)
        {
            //HOLSTERED
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetWeapon(0);
            }

            //PISTOL
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetWeapon(1);
            }

            //SHOTGUN
            if (Input.GetKeyDown(KeyCode.Alpha3) && player.GetGameRole() == Player.Role.Traitor)
            {
                SetWeapon(2);
            }
        }
    }

    private void HideWeapons()
    {
        foreach (Weapon _weapon in weapons)
        {
            _weapon.graphics.SetActive(false);
        }
    }

    public void SetWeapon(int _index)
    {
        if(currentGun != _index)
        {
            ClientSend.WeaponSwap(_index);

            HideWeapons();

            weapons[_index].graphics.SetActive(true);
            currentGun = _index;

            playerShoot.SwapWeapons();
            playerUI.SetSelectedWeapon(_index);
        }
    }

    public Weapon GetCurrentWeapon()
    {
        return weapons[currentGun];
    }

    //Used for death / restarting
    public void ResetWeapons()
    {
        SetWeapon(0);
    }
}
