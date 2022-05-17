using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Message;

public partial class Damageable : MonoBehaviour
{
    [SerializeField] private GameObject m_DamageEffect;

    [SerializeField] private int m_MaxHitPoints;
    public int currentHitPoints { get; private set; }

    public UnityEvent OnDeath, OnReceiveDamage, OnResetDamage;

    [Tooltip("When this gameObject is damaged, these other gameObjects are notified.")]
    //[EnforceType(typeof(Message.IMessageReceiver))]
    public List<MonoBehaviour> onDamageMessageReceivers;

    protected Collider m_Collider;
    private System.Action schedule;

    private bool m_IsDead = false;
    public bool IsDead { get => m_IsDead; }

    private void Start()
    {
        ResetDamage();
        m_Collider = GetComponent<Collider>();
    }

    public void ResetDamage()
    {
        currentHitPoints = m_MaxHitPoints;
        OnResetDamage.Invoke();
    }

    public void ApplyDamage(DamageMessage data)
    {
        if (m_DamageEffect != null)
        {
            GameObject hitEffect = PoolMgr.Instance.Spawn(m_DamageEffect.name, transform.position, Quaternion.identity);
            hitEffect.transform.SetParent(transform);
        }
        if (currentHitPoints <= 0)
        {
            return;
        }

        currentHitPoints -= data.amount;

        if (currentHitPoints <= 0)
        {
            //This avoid race condition when objects kill each other.
            schedule += OnDeath.Invoke;
            m_Collider.enabled = false;
            m_IsDead = true;
        }
        else
        {
            OnReceiveDamage.Invoke();
        }

        MessageType messageType = currentHitPoints <= 0 ? MessageType.DEAD : MessageType.DAMAGED;

        for (int i = 0; i < onDamageMessageReceivers.Count; ++i)
        {
            var receiver = onDamageMessageReceivers[i] as IMessageReceiver;
            receiver.OnReceiveMessage(messageType, this, data);
        }
    }

    private void LateUpdate()
    {
        if (schedule != null)
        {
            schedule();
            schedule = null;
        }
    }

//#if UNITY_EDITOR
//    private void OnDrawGizmosSelected()
//    {
//
//    }
//#endif
}