using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class MagicChargeBossState : IBossState, IDisposable
{
    private ReactiveProperty<bool> _magicChargeBool { get; } = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> MagicChargeBool => _magicChargeBool;

    public MagicChargeBossState()
    {

    }

    public void Enter()
    {
        _magicChargeBool.Value = true;
    }

    public void Execute()
    {

    }

    public void Exit()
    {
        _magicChargeBool.Value = false;
    }

    public void Dispose()
    {
        _magicChargeBool.Dispose();
    }
}
