using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [SerializeField] private GameObject m_Bullet;
    [Space]
    [SerializeField] private AudioClip m_FireSound;
    [SerializeField] private AudioClip m_ClipIn;
    [SerializeField] private AudioClip m_ClipOut;
    [Space]
    [SerializeField] private float m_ShootForce, m_UpwardForce;
    [SerializeField] private float m_TimeBetweenShooting, m_Spread, m_TimeBetweenShots;
    [SerializeField] private int m_MagazineSize, m_TotalAmmos, m_BulletsPerTap;
    [SerializeField] private float m_ReloadTime, m_ResetReloadTime;
    [Space]
    private int m_BulletsLeft, m_BulletsShot;

    private bool m_readyToShoot, m_Reloading, m_Shooting;

    [SerializeField] private Transform m_AttackPoint;
    [SerializeField] private ParticleSystem m_MuzzleFlash;

    public bool allowInvoke = true;

    public bool Shooting
    {
        get => m_Shooting;
    }

    private void Awake()
    {
        m_BulletsLeft = m_MagazineSize;
        m_readyToShoot = true;
        m_ResetReloadTime = m_ReloadTime;
    }

    private void Update()
    {
        if (m_Reloading)
        {
            m_ReloadTime -= Time.deltaTime;
            if (m_ReloadTime <= 0f)
            {
                ReloadFinished();
            }
        }
    }

    public void Shoot(Vector3 target)
    {   
        if (!m_Reloading && m_BulletsLeft > 0)
        {
            m_BulletsShot = 0;
            ProcessShoot(target);
        }
        else if (!m_Reloading && m_BulletsLeft <= 0 && m_TotalAmmos > 0)
        {
            Reload();
        }
    }

    private void ProcessShoot(Vector3 target)
    {
        m_readyToShoot = false;
        m_AttackPoint.LookAt(PlayerController.Instance.transform);
        m_Shooting = true;

        Ray ray = new Ray(m_AttackPoint.position, target + new Vector3(0, 0.65f, 0));//m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
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

        Vector3 directionWithoutSpread = targetPoint - m_AttackPoint.position;

        float x = Random.Range(-m_Spread, m_Spread);
        float y = Random.Range(-m_Spread, m_Spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        GameObject currentBullet = PoolMgr.Instance.Spawn(m_Bullet.name, m_AttackPoint.position, Quaternion.identity);//Instantiate(m_Bullet, m_AttackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * m_ShootForce, ForceMode.Impulse);
        //currentBullet.GetComponent<Rigidbody>().AddForce(m_AttackPoint.transform.up * m_UpwardForce, ForceMode.Impulse);

        if (m_MuzzleFlash != null)
        {
            m_MuzzleFlash.Play();
        }

        //if (m_FireSound != null && m_Weapon != GameWeapon.SHOTGUN)
        //{
        //    PlaySoundEffect(m_FireSound);
        //}

        m_BulletsLeft--;
        m_BulletsShot++;

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
    }

    private void ResetShot()
    {
        m_readyToShoot = true;
        m_Shooting = false;
        allowInvoke = true;
    }

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

        m_ReloadTime = m_ResetReloadTime;
        m_Reloading = false;
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        SFX_Audio fireSound = PoolMgr.Instance.Spawn("SFX_Audio", transform.position, Quaternion.identity).GetComponent<SFX_Audio>();
        fireSound.SetUp(clip);
        fireSound.Play();
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