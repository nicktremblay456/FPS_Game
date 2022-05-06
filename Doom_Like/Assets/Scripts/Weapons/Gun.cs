using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private float m_Range = 20f;
    [SerializeField] private float m_VerticalRange = 20f;
    [Space]
    [SerializeField] private bool allowInvoke = true;
    [Space]
    [SerializeField] private LayerMask m_RaycastLayer;

    private int m_BulletsShot;

    private bool m_Shooting, m_ReadyToShoot;

    [SerializeField] private GunStats m_GunStats;
    private BoxCollider m_GunTrigger; 
    private List<GameObject> m_Enemies = new List<GameObject>();

    private void Awake()
    {
        m_ReadyToShoot = true;

        m_GunTrigger = GetComponent<BoxCollider>();
        m_GunTrigger.size = new Vector3(1, m_VerticalRange, m_Range);
        m_GunTrigger.center = new Vector3(0, 0, m_Range * 0.5f);
    }

    private void Update()
    {
        GetInput();
    }

    public void UpdateGunStats(GunStats stats)
    {
        m_GunStats = stats;
    }

    private void GetInput()
    {
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

        foreach(GameObject go in m_Enemies)
        {
            Vector3 dir = go.transform.position - transform.position;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dir, out hit, m_Range * 1.5f, m_RaycastLayer))
            {
                if (hit.transform == go.transform)
                {
                    go.SetActive(false);
                }
            }
        }

        m_GunStats.totalAmmos--;
        m_BulletsShot++;

        if(allowInvoke)
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (0 != (LayerMask.GetMask("Enemy") & 1 << other.gameObject.layer))
        {
            m_Enemies.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (0 != (LayerMask.GetMask("Enemy") & 1 << other.gameObject.layer))
        {
            m_Enemies.Remove(other.gameObject);
        }
    }
}