using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class IdleBossState : IBossState
{
    private ReactiveProperty<bool> _idleBool { get; } = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> IdleBool => _idleBool;

    public IdleBossState()
    {

    }

    public void Enter()
    {
        _idleBool.Value = true;
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        _idleBool.Value = false;
    }
}
