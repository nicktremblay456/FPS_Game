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
    [SerializeField] private EGun m_GunID;
    public EGun GunID
    {
        get => m_GunID;
    }
    [Space]
    [SerializeField] private AudioClip m_FireSound;
    [Space]
    [SerializeField] private GunStats m_GunStats;

    private GameObject m_FireEffect;
    private Gun m_Gun;
    private Animator m_Animator;

    public bool ReadyToShoot 
    { 
        get
        {
            if (m_Gun == null)
                m_Gun = FindObjectOfType<Gun>();

            return m_Gun.ReadyToShoot;
        }
    }

    private readonly int m_HashFire = Animator.StringToHash("Fire");


    private void Awake()
    {
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
        m_Gun.UpdateGunStats(m_GunStats, this);
        SetAmmoUI();
    }

    public void Shoot()
    {
        m_Animator.SetTrigger(m_HashFire);
    }

    private void SetAmmoUI()
    {
        //PlayerHud.Instance?.UpdateAmmoText(m_BulletsLeft.ToString() + "/" + m_TotalAmmos);
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