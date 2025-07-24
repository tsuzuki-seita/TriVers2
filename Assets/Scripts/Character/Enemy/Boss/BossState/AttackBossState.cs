using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class AttackBossState : IBossState
{
    private ReactiveProperty<bool> _attackTrigger { get; } = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> AttackTrigger => _attackTrigger;

    public AttackBossState()
    {

    }

    public void Enter()
    {
        _attackTrigger.Value = true;
    }

    public void Execute()
    {

    }

    public void Exit()
    {
        _attackTrigger.Value = false;
    }
}
