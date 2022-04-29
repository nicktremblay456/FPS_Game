using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWeapon : MonoBehaviour
{
    [SerializeField] private GameWeapon m_Weapon;
    public GameWeapon weapon
    {
        get => m_Weapon;
    }
    [Space]
    [SerializeField] private Sprite m_CrosshairSprite;
    [SerializeField] private float m_CrosshairScale = 1f;
    [Space]
    [SerializeField] private GameObject m_Bullet;
    [Space]
    [SerializeField] private AudioClip m_FireSound;
    [SerializeField] private AudioClip m_ClipIn;
    [SerializeField] private AudioClip m_ClipOut;
    [Space]
    [SerializeField] private float m_ShootForce, m_UpwardForce;
    [SerializeField] private float m_TimeBetweenShooting, m_Spread, m_TimeBetweenShots;
    [SerializeField] private int m_MagazineSize, m_TotalAmmos, m_BulletsPerTap;
    [SerializeField] private bool m_AllowButtonHold;
    [Space]
    private int m_BulletsLeft, m_BulletsShot;

    private bool m_Shooting, m_readyToShoot, m_Reloading;

    private Camera m_Camera;
    [SerializeField] private Transform m_AttackPoint;
    [SerializeField] private ParticleSystem m_MuzzleFlash; 

    public bool allowInvoke = true;

    private PlayerController m_Player;
    private Animator m_Animator;

    private Vector3 m_InitialPosition;
    private Quaternion m_InitialRotation;

    private Quaternion m_CurrentRotation;
    private Quaternion m_LocalRotation;
    [SerializeField] private float m_RotationSpeed = 15f;
    [Space]
    [SerializeField] private float m_Recoil = 0.01f;
    private WeaponRecoil m_WeaponRecoil;

    readonly int m_HashRunning = Animator.StringToHash("IsRunning");
    readonly int m_HashReload = Animator.StringToHash("Reload");


    private void Awake()
    {
        m_BulletsLeft = m_MagazineSize;
        m_readyToShoot = true;

        m_Animator = GetComponent<Animator>();
        m_Player = GetComponentInParent<PlayerController>();
        m_WeaponRecoil = GetComponentInParent<WeaponRecoil>();

        m_InitialPosition = transform.localPosition;
        m_InitialRotation = transform.localRotation;

        m_LocalRotation = transform.localRotation;
        m_CurrentRotation = transform.rotation;
    }

    private void Start()
    {
        m_Camera = Camera.main;
    }

    private void Update()
    {
        WeaponRotation();
        GetInput();
    }

    private void OnEnable()
    {
        SetAmmoUI();
    }

    private void OnDisable()
    {
        if (m_Reloading)
            m_Reloading = false;
        transform.localPosition = m_InitialPosition;
        transform.localRotation = m_InitialRotation;
    }

    private void WeaponRotation()
    {
        Quaternion targetRotation = transform.parent.rotation;
        m_CurrentRotation = Quaternion.Lerp(m_CurrentRotation, targetRotation, m_RotationSpeed * Time.deltaTime);
        transform.rotation = m_CurrentRotation;
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

        if (m_AllowButtonHold)
        {
            m_Shooting = Input.GetButton("Fire1");
        }
        else
        {
            m_Shooting = Input.GetButtonDown("Fire1");
        }

        if (Input.GetKeyDown(KeyCode.R) && m_BulletsLeft < m_MagazineSize && !m_Reloading && m_TotalAmmos > 0)
        {
            Reload();
        }

        if (m_readyToShoot && m_Shooting && !m_Reloading && m_BulletsLeft > 0)
        {
            m_BulletsShot = 0;
            Shoot();
            if (m_FireSound != null && m_Weapon == GameWeapon.SHOTGUN)
            {
                PlaySoundEffect(m_FireSound);
            }
        }
        else if (!m_Reloading && m_BulletsLeft <= 0 && m_TotalAmmos > 0)
        {
            Reload();
        }
    }

    private void Shoot()
    {
        m_readyToShoot = false;

        Ray ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        m_WeaponRecoil.Recoil = m_Recoil;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
        }

        Vector3 directionWithoutSpread = targetPoint - m_AttackPoint.position;

        float x = Random.Range(-m_Spread, m_Spread);
        float y = Random.Range(-m_Spread, m_Spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x,y,0);

        GameObject currentBullet = PoolMgr.Instance.Spawn(m_Bullet.name, m_AttackPoint.position, Quaternion.identity);//Instantiate(m_Bullet, m_AttackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * m_ShootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(m_Camera.transform.up * m_UpwardForce, ForceMode.Impulse);
    
        if (m_MuzzleFlash != null)
        {
            m_MuzzleFlash.Play();
        }

        if ( m_FireSound != null && m_Weapon != GameWeapon.SHOTGUN)
        {
            PlaySoundEffect(m_FireSound);
        }

        m_BulletsLeft--;
        m_BulletsShot++;

        // Set Ammo text
        SetAmmoUI();

        if (allowInvoke)
        {
            Invoke("ResetShot", m_TimeBetweenShooting);
            allowInvoke = false;   
        }

        if (m_BulletsShot < m_BulletsPerTap && m_BulletsLeft > 0)
        {
            Invoke("Shoot", m_TimeBetweenShots);
        }
    }

    private void Reload()
    {
        m_Reloading = true;
        m_Animator.SetTrigger(m_HashReload);
    }

    private void ResetShot()
    {
        m_readyToShoot = true;
        allowInvoke = true;
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
        if (m_Weapon == GameWeapon.MARCHINEGUN)
        {
            m_BulletsLeft += ammo;
        }
        else
        {
            m_TotalAmmos += ammo;
        }

        if (gameObject.activeSelf)
            SetAmmoUI();
    }

    /*
     * 
     * Functions below are only called in animation event
     * 
     */

    public void ReloadFinished()
    {
        int ammo = m_MagazineSize - m_BulletsLeft; ;
        if (m_TotalAmmos >= m_MagazineSize)
        {
            m_BulletsLeft += ammo;
            m_TotalAmmos -= ammo;
        }
        else
        {
            m_BulletsLeft = m_TotalAmmos;
            m_TotalAmmos = 0;
        }
        
        m_Reloading = false;
        SetAmmoUI();
        m_Animator.ResetTrigger(m_HashReload);
    }

    public void ClipInSound()
    {
        PlaySoundEffect(m_ClipIn);
    }

    public void ClipOutSound()
    {
        PlaySoundEffect(m_ClipOut);
    }
}