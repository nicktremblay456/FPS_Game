using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float m_Sensitivity = 1.5f;
    [SerializeField] private float m_Smoothing = 10f;

    private float m_MouseX;
    private float m_SmoothMousePos;

    private float m_CurrentLookingPos;

    private PlayerInput m_Input;

    private void Awake()
    {
        m_Input = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        GetInput();
        ModifyInput();
        MovePlayer();
    }

    private void GetInput()
    {
        m_MouseX = m_Input.CameraInput.x;
    }

    private void ModifyInput()
    {
        m_MouseX *= m_Sensitivity * m_Smoothing;
        m_SmoothMousePos = Mathf.Lerp(m_SmoothMousePos, m_MouseX, 1f / m_Smoothing);
    }

    private void MovePlayer()
    {
        m_CurrentLookingPos += m_SmoothMousePos;
        transform.localRotation = Quaternion.AngleAxis(m_CurrentLookingPos, transform.up);
    }
}