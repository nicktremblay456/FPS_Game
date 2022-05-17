using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : BaseEnemy
{
    [SerializeField] private int m_Damage;

    private bool m_IsAttackReady = true;

    private readonly int m_HashRunning = Animator.StringToHash("IsRunning");
    private readonly int m_HashAttack = Animator.StringToHash("Attack");
    private readonly int m_HashDeath = Animator.StringToHash("Death");

    protected override void Awake()
    {
        base.Awake();
        m_DamageableData.amount = m_Damage;
    }

    protected override void OnIdleEnter()
    {
        StopMovement();
        m_Animator.SetBool(m_HashRunning, false);
        if (m_Target.IsDead)
            this.enabled = false;
    }

    protected override void OnIdleUpdate()
    {
        if (Vector3.Distance(transform.position, m_Target.transform.position) <= m_ChaseThreshold)
            ChangeState(State.Chase);
    }

    protected override void OnChaseEnter()
    {
        GainMovement();
        m_Animator.SetBool(m_HashRunning, true);
    }

    protected override void OnChaseUpdate()
    {
        if (m_Target.IsDead)
            ChangeState(State.Idle);

        m_Agent.SetDestination(m_Target.transform.position);
        if (Vector3.Distance(transform.position, m_Target.transform.position) <= m_AttackThreshold)
            ChangeState(State.Attack);
    }

    protected override void OnAttackEnter()
    {
        StopMovement();
        m_Animator.SetBool(m_HashRunning, false);
    }

    protected override void OnAttackUpdate()
    {
        if (m_Target.IsDead)
            ChangeState(State.Idle);

        if (Vector3.Distance(transform.position, m_Target.transform.position) >= m_AttackThreshold)
            ChangeState(State.Chase);

        if (m_IsAttackReady)
            m_Animator.SetTrigger(m_HashAttack);
    }

    protected override void OnAttackExit()
    {
        m_IsAttackReady = true;
    }

    protected override void OnDeathEnter()
    {
        StopMovement();
        Destroy(m_Rigidbody);
        m_Agent.enabled = false;
        m_IsDead = true;
        m_Animator.SetTrigger(m_HashDeath);
    }

    protected override void OnDeathUpdate()
    {
        
    }

    #region Animation Events
    public void OnAttackStart()
    {
        m_IsAttackReady = false;
        RaycastHit hit;
        if (Physics.BoxCast(transform.position, transform.localScale, transform.forward * m_AttackThreshold, out hit, Quaternion.identity, LayerMask.GetMask("Player")))
        {
            Damageable damageable = hit.transform.GetComponent<Damageable>();
            if (damageable != null)
                damageable.ApplyDamage(m_DamageableData);
        }
    }

    public void OnAttackEnd()
    {
        m_IsAttackReady = true;
    }

    public void OnDeath()
    {
        ChangeState(State.Death);
    }

    public void OnDeathEnd()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, LayerMask.GetMask("Default")))
        {
            transform.position = hit.point + new Vector3(0, 0.26f, 0);
        }
    }
    #endregion
}