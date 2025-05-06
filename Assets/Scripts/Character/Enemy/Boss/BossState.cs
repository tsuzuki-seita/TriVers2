using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public enum BossState
{
    Idle = 0,
    Walk = 1,
    Attack = 2,
    MagicCharge = 3,
    MagicRelease = 4,
    Laughing = 5,
    Damage = 6,
    Dead = 7,
}

public interface IBossState
{
    BossState GetCurrentState { get; }
    bool ChangeState(IBossState nextState);

    void OnStateChanged();
    void OnStateBegin();
    void OnStateEnd();

    void Update(float deltaTime);
    void SetNextState(IBossState nextState);
    IBossState GetNextState();
}

public class IdleBossState : IBossState
{
    private IBossState m_nextState =null;
    public bool IsEndState { get; protected set; } = false;
    #region ===== IBossState =====

    BossState IBossState.GetCurrentState { get; } = BossState.Idle;

    bool IBossState.ChangeState(IBossState nextState)
    {
        IBossState state = this;
        state.OnStateEnd();
        if (nextState == null) return false;
        
        nextState.OnStateChanged();
        nextState.OnStateBegin();
        return true;
    }

    void IBossState.OnStateChanged()
    {
        // Initialize
    }

    void IBossState.OnStateBegin()
    {
        IsEndState = false;
    }

    void IBossState.OnStateEnd()
    {
        IsEndState = true;
    }

    void IBossState.Update(float deltaTime)
    {
        if (IsEndState) return;
        if (m_nextState != null)
        {
            (this as IBossState).ChangeState(m_nextState);
            return;
        }
    }

    void IBossState.SetNextState(IBossState nextState) { m_nextState = nextState;}
    IBossState IBossState.GetNextState() { return m_nextState; }

    #endregion //) ===== IState =====

}
public class WalkBossState : IBossState
{
    protected Transform m_moveTarget = null;
    protected Vector3 m_moveDestination = Vector3.zero;
    protected float m_moveSpeed = 0.0f;
    private IBossState m_nextState =null;
    public bool IsEndState { get; protected set; } = false;
    #region ===== IBossState =====

    BossState IBossState.GetCurrentState { get; } = BossState.Walk;

    bool IBossState.ChangeState(IBossState nextState)
    {
        IBossState state = this;
        state.OnStateEnd();
        if (nextState == null) return false;
        
        nextState.OnStateChanged();
        nextState.OnStateBegin();
        return true;
    }

    void IBossState.OnStateChanged()
    {
        // Initialize
    }

    void IBossState.OnStateBegin()
    {
        IsEndState = false;
    }

    void IBossState.OnStateEnd()
    {
        IsEndState = true;
    }

    void IBossState.Update(float deltaTime)
    {
        if (IsEndState) return;
        if (m_nextState != null)
        {
            (this as IBossState).ChangeState(m_nextState);
            return;
        }

        if (m_moveTarget == null) return;
        Vector3 currentPosition = m_moveTarget.position;
        Vector3 moveVec = (m_moveDestination - currentPosition).normalized;
        m_moveTarget.position = currentPosition + moveVec * (m_moveSpeed * deltaTime);
    }

    void IBossState.SetNextState(IBossState nextState) { m_nextState = nextState;}
    IBossState IBossState.GetNextState() { return m_nextState; }

    #endregion //) ===== IState =====

    public void SetTarget(Transform target, Vector3 destination, float speedPerSec)
    {
        m_moveTarget = target;
        m_moveDestination = destination;
        m_moveSpeed = speedPerSec;
    }
}
public class AttackBossState : IBossState
{
    private IBossState m_nextState =null;
    public bool IsEndState { get; protected set; } = false;
    #region ===== IBossState =====

    BossState IBossState.GetCurrentState { get; } = BossState.Attack;

    bool IBossState.ChangeState(IBossState nextState)
    {
        IBossState state = this;
        state.OnStateEnd();
        if (nextState == null) return false;
        
        nextState.OnStateChanged();
        nextState.OnStateBegin();
        return true;
    }

    void IBossState.OnStateChanged()
    {
        // Initialize
    }

    void IBossState.OnStateBegin()
    {
        IsEndState = false;
    }

    void IBossState.OnStateEnd()
    {
        IsEndState = true;
    }

    void IBossState.Update(float deltaTime)
    {
        if (IsEndState) return;
        if (m_nextState != null)
        {
            (this as IBossState).ChangeState(m_nextState);
            return;
        }
    }

    void IBossState.SetNextState(IBossState nextState) { m_nextState = nextState;}
    IBossState IBossState.GetNextState() { return m_nextState; }

    #endregion //) ===== IAttackBoss =====
}
public class MagicChargeBossState : IBossState
{
    private IBossState m_nextState =null;
    public bool IsEndState { get; protected set; } = false;
    #region ===== IBossState =====

    BossState IBossState.GetCurrentState { get; } = BossState.MagicCharge;

    bool IBossState.ChangeState(IBossState nextState)
    {
        IBossState state = this;
        state.OnStateEnd();
        if (nextState == null) return false;
        
        nextState.OnStateChanged();
        nextState.OnStateBegin();
        return true;
    }

    void IBossState.OnStateChanged()
    {
        // Initialize
    }

    void IBossState.OnStateBegin()
    {
        IsEndState = false;
    }

    void IBossState.OnStateEnd()
    {
        IsEndState = true;
    }

    void IBossState.Update(float deltaTime)
    {
        if (IsEndState) return;
        if (m_nextState != null)
        {
            (this as IBossState).ChangeState(m_nextState);
            return;
        }
    }

    void IBossState.SetNextState(IBossState nextState) { m_nextState = nextState;}
    IBossState IBossState.GetNextState() { return m_nextState; }

    #endregion //) ===== IAttackBoss =====
}
public class MagicReleaseBossState : IBossState
{
    private IBossState m_nextState =null;
    public bool IsEndState { get; protected set; } = false;
    #region ===== IBossState =====

    BossState IBossState.GetCurrentState { get; } = BossState.MagicRelease;

    bool IBossState.ChangeState(IBossState nextState)
    {
        IBossState state = this;
        state.OnStateEnd();
        if (nextState == null) return false;
        
        nextState.OnStateChanged();
        nextState.OnStateBegin();
        return true;
    }
    void IBossState.OnStateChanged()
    {
        // Initialize
    }
    void IBossState.OnStateBegin()
    {
        IsEndState = false;
    }
    void IBossState.OnStateEnd()
    {
        IsEndState = true;
    }
    void IBossState.Update(float deltaTime)
    {
        if (IsEndState) return;
        if (m_nextState != null)
        {
            (this as IBossState).ChangeState(m_nextState);
            return;
        }
    }
    void IBossState.SetNextState(IBossState nextState) { m_nextState = nextState;}
    IBossState IBossState.GetNextState() { return m_nextState; }
    #endregion //) ===== IAttackBoss =====
}
public class LaughingBossState : IBossState
{
    private IBossState m_nextState =null;
    public bool IsEndState { get; protected set; } = false;
    #region ===== IBossState =====

    BossState IBossState.GetCurrentState { get; } = BossState.Laughing;

    bool IBossState.ChangeState(IBossState nextState)
    {
        IBossState state = this;
        state.OnStateEnd();
        if (nextState == null) return false;
        
        nextState.OnStateChanged();
        nextState.OnStateBegin();
        return true;
    }
    void IBossState.OnStateChanged()
    {
        // Initialize
    }
    void IBossState.OnStateBegin()
    {
        IsEndState = false;
    }
    void IBossState.OnStateEnd()
    {
        IsEndState = true;
    }   
    void IBossState.Update(float deltaTime)
    {
        if (IsEndState) return;
        if (m_nextState != null)
        {
            (this as IBossState).ChangeState(m_nextState);
            return;
        }
    }
    void IBossState.SetNextState(IBossState nextState) { m_nextState = nextState;}
    IBossState IBossState.GetNextState() { return m_nextState; }
    #endregion //) ===== IAttackBoss =====
}
public class DamageBossState : IBossState
{
    private IBossState m_nextState =null;
    public bool IsEndState { get; protected set; } = false;
    #region ===== IBossState =====

    BossState IBossState.GetCurrentState { get; } = BossState.Damage;

    bool IBossState.ChangeState(IBossState nextState)
    {
        IBossState state = this;
        state.OnStateEnd();
        if (nextState == null) return false;
        
        nextState.OnStateChanged();
        nextState.OnStateBegin();
        return true;
    }
    void IBossState.OnStateChanged()
    {
        // Initialize
    }
    void IBossState.OnStateBegin()
    {
        IsEndState = false;
    }
    void IBossState.OnStateEnd()
    {
        IsEndState = true;
    } 
    void IBossState.Update(float deltaTime)
    {
        if (IsEndState) return;
        if (m_nextState != null)
        {
            (this as IBossState).ChangeState(m_nextState);
            return;
        }
    }
    void IBossState.SetNextState(IBossState nextState) { m_nextState = nextState;}
    IBossState IBossState.GetNextState() { return m_nextState; }
    #endregion //) ===== IAttackBoss =====
}
public class DeadBossState : IBossState
{
    private IBossState m_nextState =null;
    public bool IsEndState { get; protected set; } = false;
    #region ===== IBossState =====

    BossState IBossState.GetCurrentState { get; } = BossState.Dead;

    bool IBossState.ChangeState(IBossState nextState)
    {
        IBossState state = this;
        state.OnStateEnd();
        if (nextState == null) return false;
        
        nextState.OnStateChanged();
        nextState.OnStateBegin();
        return true;
    }
    void IBossState.OnStateChanged()
    {
        // Initialize
    }
    void IBossState.OnStateBegin()
    {
        IsEndState = false;
    }
    void IBossState.OnStateEnd()
    {
        IsEndState = true;
    }
    void IBossState.Update(float deltaTime)
    {
        if (IsEndState) return;
        if (m_nextState != null)
        {
            (this as IBossState).ChangeState(m_nextState);
            return;
        }
    }
    void IBossState.SetNextState(IBossState nextState) { m_nextState = nextState;}
    IBossState IBossState.GetNextState() { return m_nextState; }
    #endregion //) ===== IAttackBoss =====
}

