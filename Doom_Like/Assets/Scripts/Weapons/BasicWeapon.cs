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
    [SerializeField] private AudioClip m_FireSound;
    [Space]
    [SerializeField] private float m_TimeBetweenShooting, m_Spread, m_TimeBetweenShots;
    [SerializeField] private int m_TotalAmmos, m_MaxAmmos, m_BulletsPerTap;
    [SerializeField] private bool m_AllowButtonHold;
    [Space]
    private int m_BulletsShot;

    private bool m_Shooting, m_readyToShoot;
    public bool allowInvoke = true;

    private GameObject m_FireEffect;

    private Camera m_Camera;
    private Animator m_Animator;

    private readonly int m_HashFire = Animator.StringToHash("Fire");


    private void Awake()
    {
        m_readyToShoot = true;

        m_FireEffect = transform.Find("Fire").gameObject;
        m_Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_Camera = Camera.main;
    }

    private void Update()
    {
        GetInput();
    }

    private void OnEnable()
    {
        SetAmmoUI();
    }

    //private void OnDisable()
    //{
    //    transform.localPosition = m_InitialPosition;
    //    transform.localRotation = m_InitialRotation;
    //}

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

        if (m_readyToShoot && m_Shooting && m_TotalAmmos > 0)
        {
            m_BulletsShot = 0;
            m_Animator.SetTrigger(m_HashFire);
            Shoot();
        }
        else if (m_TotalAmmos <= 0)
        {
            // TO DO
            // Switch weapon
        }
    }

    private void Shoot()
    {
        m_readyToShoot = false;

        Ray ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
        }

        m_TotalAmmos--;
        m_BulletsShot++;

        // Set Ammo text
        SetAmmoUI();

        if (allowInvoke)
        {
            Invoke("ResetShot", m_TimeBetweenShooting);
            allowInvoke = false;   
        }

        if (m_BulletsShot < m_BulletsPerTap && m_TotalAmmos > 0)
        {
            Invoke("Shoot", m_TimeBetweenShots);
        }
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
        m_TotalAmmos += ammo;

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