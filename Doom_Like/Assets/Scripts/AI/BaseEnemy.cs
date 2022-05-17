using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using TNT.StateMachine;

[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(Animator))]
public abstract class BaseEnemy : MonoBehaviour
{
    [SerializeField] protected int m_ChaseThreshold;
    [SerializeField] protected float m_AttackThreshold;
    [Space]
    [SerializeField] protected float m_DespawnTimer = 2.5f;

    [SerializeField] protected UnityEvent m_OnDeathEvent;

    protected bool m_IsDead = false;

    protected enum State
    {
        Idle, Chase, Attack, Death,
    }

    protected bool m_IsEngaged = false;

    protected Damageable m_Damageable;
    protected Damageable.DamageMessage m_DamageableData;

    protected StateMachine m_SM;
    protected Animator m_Animator;
    protected NavMeshAgent m_Agent;
    protected Rigidbody m_Rigidbody;
    protected PlayerController m_Target;

    public GameObject Target { get => m_Target.gameObject; }
    public bool IsDeath { get => m_IsDead; }


    protected virtual void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Damageable = GetComponent<Damageable>();

        m_Target = FindObjectOfType<PlayerController>();

        InitSM();
    }

    protected virtual void Update()
    {
        m_SM.UpdateSM();
        if (m_IsDead)
        {
            if (!IsTargetInRange(100)) gameObject.SetActive(false);
        }
    }

    protected virtual void InitSM()
    {
        m_SM = new StateMachine();

        m_SM.AddState((int)State.Idle);
        m_SM.AddState((int)State.Chase);
        m_SM.AddState((int)State.Attack);
        m_SM.AddState((int)State.Death);

        m_SM.OnEnter((int)State.Idle, OnIdleEnter);
        m_SM.OnEnter((int)State.Chase, OnChaseEnter);
        m_SM.OnEnter((int)State.Attack, OnAttackEnter);
        m_SM.OnEnter((int)State.Death, OnDeathEnter);

        m_SM.OnUpdate((int)State.Idle, OnIdleUpdate);
        m_SM.OnUpdate((int)State.Chase, OnChaseUpdate);
        m_SM.OnUpdate((int)State.Attack, OnAttackUpdate);
        m_SM.OnUpdate((int)State.Death, OnDeathUpdate);

        m_SM.OnExit((int)State.Attack, OnAttackExit);

        m_SM.Init((int)State.Idle);
    }

    protected void GainMovement() => m_Agent.isStopped = false;

    protected void StopMovement()
    {
        m_Agent.isStopped = true;
        m_Agent.velocity = Vector3.zero;
    }

    protected void ChangeState(State state)
    {
        if (m_SM.CurrentState != (int)state) m_SM.ChangeState((int)state);
    }

    protected bool IsTargetInRange(float threshold)
    {
        return Vector3.Distance(transform.position, m_Target.transform.position) <= threshold;
    }

    #region Abstract Methods
    protected abstract void OnIdleEnter();
    protected abstract void OnIdleUpdate();

    protected abstract void OnChaseEnter();
    protected abstract void OnChaseUpdate();

    protected abstract void OnAttackEnter();
    protected abstract void OnAttackUpdate();
    protected abstract void OnAttackExit();

    protected abstract void OnDeathEnter();
    protected abstract void OnDeathUpdate();
    #endregion
}