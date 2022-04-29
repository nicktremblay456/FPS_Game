using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class TriggerEvent : MonoBehaviour
{
    [SerializeField] protected UnityEvent m_OnCollisionEnterActions;
    [SerializeField] protected UnityEvent m_OnTriggerEnterActions;
    [SerializeField] protected UnityEvent m_OnTriggerExitActions;
    protected Action m_OnTriggerArea;
    protected Action m_OnCollecting;

    [SerializeField] private LayerMask m_SourceTriggerLayer;

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (GameManager.Instance.CheckCollision(m_SourceTriggerLayer, other.gameObject))
        {
            if (m_OnCollisionEnterActions != null)
            {
                m_OnCollisionEnterActions.Invoke();
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other) 
    {
        if (GameManager.Instance.CheckCollision(m_SourceTriggerLayer, other.gameObject))
        {
            if (m_OnTriggerEnterActions != null)
            {
                m_OnTriggerEnterActions.Invoke();
            }

            // QUEST CODE SECTION
            if (m_OnTriggerArea != null)
            {
                m_OnTriggerArea.Invoke();
            }
            if (m_OnCollecting != null)
            {
                m_OnCollecting.Invoke();
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (GameManager.Instance.CheckCollision(m_SourceTriggerLayer, other.gameObject))
        {
            if (m_OnTriggerExitActions != null)
            {
                m_OnTriggerExitActions.Invoke();
            }
        }
    }
}
