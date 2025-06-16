using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class LaughBossState : IBossState,IDisposable
{
    private ReactiveProperty<bool> _laughTrigger { get; } = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> LaughTrigger => _laughTrigger;
    public LaughBossState()
    {

    }

    public void Enter()
    {
        _laughTrigger.Value = true;
    }

    public void Execute()
    {

    }

    public void Exit()
    {
        _laughTrigger.Value = false;
    }
    
    public void Dispose()
    {
        _laughTrigger.Dispose();
    }
}
