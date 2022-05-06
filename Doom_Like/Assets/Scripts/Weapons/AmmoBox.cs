using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    private Collider m_Collider;
    private LayerMask m_TriggerLayer;
    private GameObject m_AmmoBox;

    [SerializeField] private EGun m_WeaponAmmo;
    [SerializeField] private bool m_CanRespawn = false;
    [SerializeField] private float m_RespawnTimer = 30f;
    [SerializeField] private int m_TotalAmmo;
    private float m_ResetRespawnTimer;

    private bool m_IsAvailable = true;

    [Space]
    [SerializeField] private float m_RotateSpeed;
    [SerializeField] private float m_Frequency, m_Amplitude;
    private Vector3 m_StartPosition;

    private void Awake()
    {
        m_Collider = GetComponent<Collider>();
        m_TriggerLayer = LayerMask.GetMask("Player");
        m_AmmoBox = transform.GetChild(0).gameObject;
        m_ResetRespawnTimer = m_RespawnTimer;
        m_StartPosition = m_AmmoBox.transform.position;
    }

    private void Update()
    {
        if (!m_IsAvailable && m_CanRespawn)
        {
            m_RespawnTimer -= Time.deltaTime;
            if (m_RespawnTimer <= 0f)
            {
                Respawn();
            }
        }

        m_AmmoBox.transform.Rotate(0, m_RotateSpeed * Time.deltaTime, 0);

        Vector3 tempPos = m_StartPosition;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * m_Frequency) * m_Amplitude;

        m_AmmoBox.transform.position = tempPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (0 != (m_TriggerLayer.value &  1 << other.gameObject.layer))
        {
            Collected();
        }
    }

    private void Collected()
    {
        m_IsAvailable = false;
        m_AmmoBox.SetActive(false);
        WeaponMgr.Instance?.AddAmmos(m_WeaponAmmo, m_TotalAmmo);
    }

    private void Respawn()
    {
        m_IsAvailable = true;
        m_AmmoBox.SetActive(true);
        m_RespawnTimer = m_ResetRespawnTimer;
    }
}