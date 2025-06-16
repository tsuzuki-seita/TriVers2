using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class MagicReleaseBossState : IBossState, IDisposable
{
    private ReactiveProperty<bool> _magicReleaseTrigger { get; } = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> MagicReleaseTrigger => _magicReleaseTrigger;
    public MagicReleaseBossState()
    {

    }

    public void Enter()
    {
        _magicReleaseTrigger.Value = true;
    }

    public void Execute()
    {

    }

    public void Exit()
    {
        _magicReleaseTrigger.Value = false;
    }
    
    public void Dispose()
    {
        _magicReleaseTrigger.Dispose();
    }
}
