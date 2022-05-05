using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController m_Instance;
    public static PlayerController Instance { get => m_Instance; }

    [SerializeField] private float m_MoveSpeed = 20f;
    [SerializeField] private float m_Gravity = -10f;

    private Vector3 m_MoveDirection = new Vector3();
    private Vector3 m_InputVector;

    private PlayerInput m_Input;
    private Animator m_CamAnimator;
    private CharacterController m_CC;

    private readonly int m_HashMoving = Animator.StringToHash("Moving");

    private void Awake()
    {
        if (m_Instance == null) m_Instance = this;

        m_Input = GetComponent<PlayerInput>();
        m_CamAnimator = GetComponentInChildren<Animator>();
        m_CC = GetComponent<CharacterController>();
    }

    private void Update()
    {
        GetInput();
        Move();
        HandleCamAnimation();
    }

    private void GetInput()
    {
        m_InputVector = new Vector3(m_Input.MoveInput.x, 0f, m_Input.MoveInput.y);
        m_InputVector.Normalize();
        m_InputVector = transform.TransformDirection(m_InputVector);

        m_MoveDirection = (m_InputVector * m_MoveSpeed) + (Vector3.up * m_Gravity);
    }

    private void Move()
    {
        m_CC.Move(m_MoveDirection * Time.deltaTime);
    }

    private void HandleCamAnimation()
    {
        m_CamAnimator.SetBool(m_HashMoving, m_CC.velocity.magnitude > 0.1f);
    }
}