using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EGun
{
    Pistol, Shotgun, SuperShotgun, Chaingun, Unmaker, RocketLauncher,
}

public class WeaponMgr : MonoBehaviour
{
    public static WeaponMgr Instance { get; private set; }

    private GunUI m_GunID;
    public GunUI GunID
    {
        get => m_GunID;
    }
    private int m_SelectedWeapon = 0;

    private List<GunUI> m_Weapons = new List<GunUI>();

    private bool m_WeaponDisabled = false;
    private bool m_IsSwitching = false;

    private float m_SwitchTimer = 0.5f;
    private float m_ResetSwitchTimer;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            throw new System.Exception("you can only have one Weapon Manager in the scene");
        }

        Instance = this;

        m_ResetSwitchTimer = m_SwitchTimer;
        foreach (Transform gun in transform)
        {
            m_Weapons.Add(gun.gameObject.GetComponent<GunUI>());
        }
    }

    private void Start()
    {
        SelectWeapon();
    }

    private void Update()
    {
        //if (m_Player.IsDeath || m_WeaponDisabled)
        //    return;
        if (m_IsSwitching)
        {
            m_SwitchTimer -= Time.deltaTime;
            if (m_SwitchTimer <= 0f)
            {
                m_SwitchTimer = m_ResetSwitchTimer;
                m_IsSwitching = false;
            }
        }

        if (!m_IsSwitching && m_GunID.ReadyToShoot)
        {
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
                m_IsSwitching = true;
                SelectWeapon();
            }
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
                m_GunID = weapon.gameObject.GetComponent<GunUI>();
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }

    public void AddAmmos(EGun weapon, int ammo)
    {
        foreach (GunUI gun in m_Weapons)
        {
            if (gun.EquipedGun == weapon)
            {
                gun.AddAmmos(ammo);
                break;
            }
        }
    }

    public void DisableWeapon()
    {
        m_WeaponDisabled = true;
        foreach (GunUI gun in m_Weapons)
        {
            gun.gameObject.SetActive(false);
        }
    }
}