using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCondition : MonoBehaviour
{
    [SerializeField] private Transform m_Door;
    [SerializeField] private Transform m_StartPosition;
    [SerializeField] private Transform m_EndPosition;
    [SerializeField] private float m_Duration;

    [SerializeField] private int m_NumberOfSwitchRequire;
    private int m_CurrentSwitchActivated;

    private bool m_IsDoorOpen = false;

    public void DoorSwitchActivated()
    {
        m_CurrentSwitchActivated++;
        if (m_CurrentSwitchActivated == m_NumberOfSwitchRequire)
        {
            StartCoroutine(DoorCoroutine(true));
        }
    }

    public void DoorSwitchDeactivated()
    {
        m_CurrentSwitchActivated--;
        if (m_CurrentSwitchActivated != m_NumberOfSwitchRequire && m_IsDoorOpen)
        {
            StartCoroutine(DoorCoroutine(false));
        }
    }

    public void TriggerDoorClose()
    {
        StartCoroutine(DoorCoroutine(true));
    }

    public void TriggerDoorOpen()
    {
        StartCoroutine(DoorCoroutine(false));
    }

    public IEnumerator DoorCoroutine(bool open)
    {
        Vector3 target = open ? m_EndPosition.position : m_StartPosition.position;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / m_Duration;
            m_Door.position = Vector3.Lerp(m_Door.position, target, t);
            yield return null;
        }
        m_Door.position = target;
        m_IsDoorOpen = open ? true : false;
    }
}
