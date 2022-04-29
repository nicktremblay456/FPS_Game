using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Message;

public partial class Damageable : MonoBehaviour
{
    [SerializeField] private GameObject m_DamageEffect;

    public int maxHitPoints;
    public int currentHitPoints { get; private set; }

    public UnityEvent OnDeath, OnReceiveDamage, OnResetDamage;

    [Tooltip("When this gameObject is damaged, these other gameObjects are notified.")]
    //[EnforceType(typeof(Message.IMessageReceiver))]
    public List<MonoBehaviour> onDamageMessageReceivers;

    protected Collider m_Collider;

    private System.Action schedule;

    private void Start()
    {
        ResetDamage();
        m_Collider = GetComponent<Collider>();
    }

    public void ResetDamage()
    {
        currentHitPoints = maxHitPoints;
        OnResetDamage.Invoke();
    }

    public void SetColliderState(bool enabled)
    {
        m_Collider.enabled = enabled;
    }

    public void ApplyDamage(DamageMessage data, bool headShot)
    {
        if (m_DamageEffect != null)
        {
            PoolMgr.Instance.Spawn(m_DamageEffect.name, transform.position, Quaternion.identity);
        }
        if (currentHitPoints <= 0)
        {
            return;
        }

        currentHitPoints -= headShot ? data.amount * 2 : data.amount;

        if (currentHitPoints <= 0)
        {
            //This avoid race condition when objects kill each other.
            schedule += OnDeath.Invoke;
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