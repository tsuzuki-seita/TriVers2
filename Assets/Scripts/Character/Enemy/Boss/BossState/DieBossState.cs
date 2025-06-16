using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class DieBossState : IBossState,IDisposable
{
    private readonly ReactiveProperty<bool> _deadTrigger = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> DeadTrigger => _deadTrigger;
    public DieBossState()
    {

    }

    public void Enter()
    {
        _deadTrigger.Value = true;

        Debug.Log("BossHead is dying");
    }

    public void Execute()
    {

    }

    public void Exit()
    {

    }

    public void Dispose()
    {
        _deadTrigger.Dispose();
    }
}
