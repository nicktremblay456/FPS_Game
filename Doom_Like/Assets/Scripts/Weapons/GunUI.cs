using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GunStats
{
    public float timeBetweenShooting, timeBetweenShots;
    public int totalAmmos, maxAmmos, bulletsPerTap;
    public bool allowButtonHold;
}

public class GunUI : MonoBehaviour
{
    [SerializeField] private EGun m_EquipedGun;
    public EGun EquipedGun
    {
        get => m_EquipedGun;
    }
    [Space]
    [SerializeField] private AudioClip m_FireSound;
    [Space]
    [SerializeField] private GunStats m_GunStats;

    private int m_BulletsShot;
    private bool m_Shooting, m_ReadyToShoot, allowInvoke = true;
    private GameObject m_FireEffect;
    private Gun m_Gun;
    private Animator m_Animator;

    public Gun Gun { get => m_Gun; }
    public bool ReadyToShoot { get => m_ReadyToShoot; }

    private readonly int m_HashFire = Animator.StringToHash("Fire");


    private void Awake()
    {
        m_ReadyToShoot = true;

        m_FireEffect = transform.Find("Fire").gameObject;
        m_Animator = GetComponent<Animator>(); 
    }

    private void Start()
    {
        m_Gun = FindObjectOfType<Gun>();
    }

    private void OnEnable()
    {
        if (m_Gun == null)
            m_Gun = FindObjectOfType<Gun>();
        m_Gun.UpdateGunStats(m_GunStats);
        SetAmmoUI();
    }

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        //if (m_Player.m_MovementInputData.IsRunning && m_Player.CurrentSpeed >= m_Player.RunSpeed)
        //{
        //    m_Animator.SetBool(m_HashRunning, true);
        //    return;
        //}
        //
        //if (m_Player.m_MovementInputData.RunReleased || m_Player.CurrentSpeed <= 0f)
        //{
        //    m_Animator.SetBool(m_HashRunning, false);
        //}

        if (m_GunStats.allowButtonHold)
        {
            m_Shooting = Input.GetButton("Fire1");
        }
        else
        {
            m_Shooting = Input.GetButtonDown("Fire1");
        }

        if (m_ReadyToShoot && m_Shooting && m_GunStats.totalAmmos > 0)
        {
            m_BulletsShot = 0;
            Shoot();
        }
        else if (m_GunStats.totalAmmos <= 0)
        {
            // TO DO
            // Switch weapon
        }
    }

    private void Shoot()
    {
        m_ReadyToShoot = false;
        Debug.Log("Fire");
        m_Animator.SetTrigger(m_HashFire);

        // Set Ammo text
        SetAmmoUI();

        if (allowInvoke)
        {
            Invoke("ResetShot", m_GunStats.timeBetweenShooting);
            allowInvoke = false;   
        }

        if (m_BulletsShot < m_GunStats.bulletsPerTap && m_GunStats.totalAmmos > 0)
        {
            Invoke("Shoot", m_GunStats.timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        m_ReadyToShoot = true;
        allowInvoke = true;
        m_Animator.ResetTrigger(m_HashFire);
    }

    private void SetAmmoUI()
    {
        //PlayerHud.Instance?.UpdateAmmoText(m_BulletsLeft.ToString() + "/" + m_TotalAmmos);
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        SFX_Audio fireSound = PoolMgr.Instance.Spawn("SFX_Audio", transform.position, Quaternion.identity).GetComponent<SFX_Audio>();
        fireSound.SetUp(clip);
        fireSound.Play();
    }

    public void AddAmmos(int ammo)
    {
        m_GunStats.totalAmmos += ammo;

        if (gameObject.activeSelf)
            SetAmmoUI();
    }

    public void FireEventStart()
    {
        m_FireEffect.SetActive(true);
    }

    public void FireEventEnd()
    {
        m_FireEffect.SetActive(false);
    }
}