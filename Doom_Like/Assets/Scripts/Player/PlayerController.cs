using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [SerializeField] private float m_MoveSpeed = 8f;
    [SerializeField] private float m_MomentumDamping = 5f;
    [SerializeField] private float m_Gravity = -10f;

    private bool m_IsWalking = false;
    private bool m_IsDead = false;

    private Vector3 m_MoveDirection = new Vector3();
    private Vector3 m_InputVector;

    private PlayerInput m_Input;
    private Animator m_CamAnimator;
    private CharacterController m_CC;

    public bool IsDead { get => m_IsDead; }

    private readonly int m_HashMoving = Animator.StringToHash("Moving");
    private readonly int m_HashDeath = Animator.StringToHash("Death");

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            throw new System.Exception("you can only have one Weapon Manager in the scene");
        }

        Instance = this;

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
        if (Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D))
        {
            m_InputVector = new Vector3(m_Input.MoveInput.x, 0f, m_Input.MoveInput.y);
            m_InputVector.Normalize();
            m_InputVector = transform.TransformDirection(m_InputVector);
            if (!m_IsWalking) m_IsWalking = true;
        }
        else
        {
            m_InputVector = Vector3.Lerp(m_InputVector, Vector3.zero, m_MomentumDamping * Time.deltaTime);
            if (m_IsWalking) m_IsWalking = false;
        }

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

    public void OnDeath()
    {
        m_IsDead = true;
        m_Input.ReleaseControl();
        m_CamAnimator.SetTrigger(m_HashDeath);
    }
}