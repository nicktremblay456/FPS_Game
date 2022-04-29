using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameWeapon
{
    PISTOL, RIFLE, SHOTGUN, ROCKET, DARTBURST, MARCHINEGUN, PLASMARIFLE
}

public class WeaponMgr : MonoBehaviour
{
    public static WeaponMgr Instance { get; private set; }

    private BasicWeapon m_CurrentWeapon;
    public BasicWeapon currentWeapon
    {
        get => m_CurrentWeapon;
    }
    private int m_SelectedWeapon = 0;

    private List<BasicWeapon> m_Weapons = new List<BasicWeapon>();
    private PlayerController m_Player;

    private bool m_WeaponDisabled = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            throw new System.Exception("you can only have one Weapon Manager in the scene");
        }

        Instance = this;

        foreach (Transform gun in transform)
        {
            m_Weapons.Add(gun.gameObject.GetComponent<BasicWeapon>());
        }
    }

    private void Start()
    {
        SelectWeapon();
        m_Player = PlayerController.Instance;
    }

    private void Update()
    {
        //if (m_Player.IsDeath || m_WeaponDisabled)
        //    return;

        int previousSelectedWeapon = m_SelectedWeapon;
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (m_SelectedWeapon >= transform.childCount - 1)
            {
                m_SelectedWeapon = 0;
            }
            else
            {
                m_SelectedWeapon++;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (m_SelectedWeapon <= 0)
            {
                m_SelectedWeapon = transform.childCount - 1;
            }
            else
            {
                m_SelectedWeapon--;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_SelectedWeapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
        {
            m_SelectedWeapon = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3)
        {
            m_SelectedWeapon = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && transform.childCount >= 4)
        {
            m_SelectedWeapon = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && transform.childCount >= 5)
        {
            m_SelectedWeapon = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6) && transform.childCount >= 6)
        {
            m_SelectedWeapon = 5;
        }


        if (previousSelectedWeapon != m_SelectedWeapon)
        {
            SelectWeapon();
        }
    }

    private void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == m_SelectedWeapon)
            {
                weapon.gameObject.SetActive(true);
                m_CurrentWeapon = weapon.gameObject.GetComponent<BasicWeapon>();
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }

    public void AddAmmos(GameWeapon weapon, int ammo)
    {
        foreach (BasicWeapon gun in m_Weapons)
        {
            if (gun.weapon == weapon)
            {
                gun.AddAmmos(ammo);
                break;
            }
        }
    }

    public void DisableWeapon()
    {
        m_WeaponDisabled = true;
        foreach (BasicWeapon gun in m_Weapons)
        {
            gun.gameObject.SetActive(false);
        }
    }
}