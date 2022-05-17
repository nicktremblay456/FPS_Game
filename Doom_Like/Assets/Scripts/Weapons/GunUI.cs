using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GunStats
{
    public GameObject projectile;
    public Damageable.DamageMessage damageableData;
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
    [SerializeField] private float m_ProjectileForce = 350f;
    [Space]
    [SerializeField] private GunStats m_GunStats;
    [SerializeField] private int m_Damage;

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
        m_GunStats.damageableData.amount = m_Damage;
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
        if (m_FireEffect != null)
            m_FireEffect.SetActive(true);
        if (m_GunStats.projectile != null)
        {
            GameObject projectile = PoolMgr.Instance.Spawn(m_GunStats.projectile.name, m_Gun.transform.position, Quaternion.identity);
            Rigidbody projBody = projectile.GetComponent<Rigidbody>();
            if (projBody != null)
                projBody.AddForce(m_Gun.ProjectileSpawn.forward * m_ProjectileForce);
        }
    }

    public void FireEventEnd()
    {
        if (m_FireEffect != null)
            m_FireEffect.SetActive(false);
    }
}