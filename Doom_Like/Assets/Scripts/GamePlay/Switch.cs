using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] private LayerMask m_InteractableLayer;
    [SerializeField] private Sprite m_ActivatedSprite;
    [SerializeField] private UnityEngine.Events.UnityEvent m_OnSwitchActivation;
    private SpriteRenderer m_SpriteRenderer;
    private bool m_IsActivated = false;
    private bool m_IsInRange = false;

    private void Awake() 
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() 
    {
        if (m_IsInRange && !m_IsActivated && Input.GetKeyDown(KeyCode.Space))
        {
            m_IsActivated = true;
        }

        if (m_IsActivated)
        {
            m_SpriteRenderer.sprite = m_ActivatedSprite;
            if (m_OnSwitchActivation != null)
            {
                m_OnSwitchActivation.Invoke();
            }
            this.enabled = false;
        }
    }

    public void TriggerSwitchDoublon()
    {
        m_IsActivated = true;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (GameManager.Instance.CheckCollision(m_InteractableLayer, other.gameObject))
        {
            m_IsInRange = true;
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (GameManager.Instance.CheckCollision(m_InteractableLayer, other.gameObject))
        {
            m_IsInRange = false;
        }
    }
}
