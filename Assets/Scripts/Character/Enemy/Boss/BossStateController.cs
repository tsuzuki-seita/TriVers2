using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class BossStateController
{
    public IBossState CurrentState { get; private set; }

    // reference to the state objects
    public IdleBossState idleState;
    public WalkBossState walkState;
    public AttackBossState attackState;
    public MagicChargeBossState magicChargeState;
    public MagicReleaseBossState magicReleaseState;
    public DamageBossState damageState;
    public LaughBossState laughState;
    public DieBossState dieState;

    // event to notify other objects of the state change
    public event Action<IBossState> stateChanged;

    // pass in necessary parameters into constructor 
    public BossStateController(BossHead bossHead)
    {
        // create an instance for each state and pass in PlayerController
        this.walkState = new WalkBossState(bossHead);
        this.attackState = new AttackBossState(bossHead);
        this.idleState = new IdleBossState(bossHead);
        this.magicChargeState = new MagicChargeBossState(bossHead);
        this.magicReleaseState = new MagicReleaseBossState(bossHead);
        this.damageState = new DamageBossState(bossHead);
        this.laughState = new LaughBossState(bossHead);
        this.dieState = new DieBossState(bossHead);
    }

    // set the starting state
    public void Initialize(IBossState state)
    {
        CurrentState = state;
        state.Enter();

        // notify other objects that state has changed
        stateChanged?.Invoke(state);
    }

    // exit this state and enter another
    public void TransitionTo(IBossState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();

        // notify other objects that state has changed
        stateChanged?.Invoke(nextState);
    }

    // allow the StateMachine to update this state
    public void Execute()
    {
        if (CurrentState != null)
        {
            CurrentState.Execute();
        }
    }
}