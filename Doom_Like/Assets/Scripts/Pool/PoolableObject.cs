using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : MonoBehaviour, IPoolable
{
    private float m_ResetTimer;
    [SerializeField] protected float m_LifeTime = 5.0f;
    
    protected virtual void Awake() 
    {
        m_ResetTimer = m_LifeTime;
    }

    protected virtual void Update() 
    {
        m_LifeTime -= Time.deltaTime;
        if (m_LifeTime <= 0.0f)
        {
            ClearObject();
        }
    }

    protected void ClearObject()
    {
        if (IsInPool())
        {
            PoolMgr.Instance.Despawn(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void ResetTimer ()
    {
        m_LifeTime = m_ResetTimer;
    }

    public virtual bool IsInPool ()
    {
        return (PoolMgr.Instance.IsInPool(gameObject.name));
    }

    public virtual void OnSpawn()
    {

    }

    public virtual void OnDespawn()
    {
        ResetTimer();
    }
}