using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;

public class DamageBossState : IBossState
{
    private ReactiveProperty<bool> _damageTrigger { get; } = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> DamageTrigger => _damageTrigger;
    private BossHead _bossHead;
    private IBossPositionUpdater _positionUpdater;

    // ノックバック力
    private float _knockbackForce = 10f;

    public DamageBossState(BossHead bossHead)
    {
        _bossHead = bossHead;
        _knockbackForce = bossHead.knockbackForce;
        _positionUpdater = bossHead;
    }

    public void Enter()
    {
        _damageTrigger.Value = true;
        DOTween.To(
            () => _bossHead.Position.Value,
            value => _positionUpdater.UpdatePosition((Vector2)value),
            _bossHead.Position.Value + new Vector2(_bossHead.knockbackDirection, 0) * _knockbackForce,
            _bossHead.damageInterval
        ).SetEase(Ease.InOutQuart);    
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        _damageTrigger.Value = false;

    }
}
