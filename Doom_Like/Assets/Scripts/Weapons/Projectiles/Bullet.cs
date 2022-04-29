using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PoolableObject
{
    private TrailRenderer m_Trail;
    private Rigidbody m_Ribidbody;
    [SerializeField] private GameObject m_ImpactEffect;
    [SerializeField] private LayerMask m_ContactLayer;

    [Range(0f, 1f)]
    [SerializeField] private float m_Bounciness;
    public bool useGravity;

    [SerializeField] private int m_ExplosionDamage;
    [SerializeField] private float m_ExplosionRange;
    [SerializeField] private float m_ExplosionForce;

    [SerializeField] private int m_MaxCollisions;
    //[SerializeField] private float m_MaxLifeTime;
    [SerializeField] private bool m_ExplodeOnTouch = true;

    private int m_Collisions;
    private PhysicMaterial m_PhysicMat;
    private bool m_HasExploded = false;

    private Damageable.DamageMessage m_Data;
    public Damageable.DamageMessage data
    {
        get => m_Data;
    }

    protected override void Awake()
    {
        base.Awake();
        m_Ribidbody = GetComponent<Rigidbody>();
        m_Trail = GetComponent<TrailRenderer>();
        SetData();
    }

    public void SetData()
    {
        m_Data.amount = m_ExplosionDamage;
        m_Data.damager = this;
    }

    private void Start()
    {
        SetUp();
    }

    protected override void Update()
    {
        if (m_Collisions >= m_MaxCollisions)
        {
            Explode();
        }

        m_LifeTime -= Time.deltaTime;
        if (m_LifeTime <= 0f)
        {
            Explode();
        }
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        m_Collisions = 0;
        m_Ribidbody.velocity = Vector3.zero;
        m_HasExploded = false;
        if (m_Trail) m_Trail.Clear();
    }

    private void Explode()
    {
        if (m_HasExploded)
        {
            return;
        }

        if (m_ImpactEffect != null)
        {
            PoolMgr.Instance.Spawn(m_ImpactEffect.name, transform.position, Quaternion.identity);//Instantiate(m_ImpactEffect, transform.position, Quaternion.identity);
        }

        Collider[] enemies = Physics.OverlapSphere(transform.position, m_ExplosionRange, m_ContactLayer);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].GetComponent<Damageable>())
            {
                enemies[i].GetComponent<Damageable>().ApplyDamage(m_Data, false);
            }
            if (enemies[i].GetComponent<Rigidbody>())
            {
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRange);
            }
        }

        m_HasExploded = true;
        //Invoke("ClearObject", 0.05f);
        ClearObject();
    }

    //private void Delay()
    //{
    //    Destroy(gameObject);
    //}

    private void OnCollisionEnter(Collision other) 
    {
        m_Collisions++;
        if (0 != (m_ContactLayer.value & 1 << other.gameObject.layer) && m_ExplodeOnTouch)
        {
            Explode();
        }
    }

    private void SetUp()
    {
        m_PhysicMat = new PhysicMaterial();
        m_PhysicMat.bounciness = m_Bounciness;
        m_PhysicMat.frictionCombine = PhysicMaterialCombine.Minimum;
        m_PhysicMat.bounceCombine = PhysicMaterialCombine.Maximum;
        GetComponent<Collider>().material = m_PhysicMat;

        m_Ribidbody.useGravity = useGravity;
    }
}
