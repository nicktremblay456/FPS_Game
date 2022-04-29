using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Part
{
    Normal,
    Critical,
}

public class DamageablePart : MonoBehaviour
{
    [SerializeField] private Part m_Part;
    private bool m_IsCritPart;
    private Damageable m_Damageable;
    private LayerMask m_DamageSourceLayer;

    private void Awake()
    {
        m_Damageable = GetComponentInParent<Damageable>();
        m_DamageSourceLayer = LayerMask.GetMask("Bullet");
        m_IsCritPart = m_Part == Part.Critical ? true : false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (0 != (m_DamageSourceLayer.value & 1 << collision.gameObject.layer))
        {
            Damageable.DamageMessage data = collision.gameObject.GetComponent<Bullet>().data;
            m_Damageable.ApplyDamage(data, m_IsCritPart);
        }
    }
}