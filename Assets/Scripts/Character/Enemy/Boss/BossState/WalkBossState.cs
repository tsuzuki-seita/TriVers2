using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class WalkBossState : IBossState, IDisposable
{
    private ReactiveProperty<bool> _walkBool { get; } = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> WalkBool => _walkBool;
    private IBossPositionUpdater _positionUpdater;
    private IBossRotationUpdater _rotationUpdater;
    private BossHead _bossHead;
    private PlayerPresenter _player;
    private float _moveSpeed;

    public WalkBossState(BossHead bossHead)
    {
        _bossHead = bossHead;
        _positionUpdater = bossHead;
        _rotationUpdater = bossHead;
        _player = bossHead.playerPresenter;
        _moveSpeed = bossHead.moveSpeed;
    }

    public void Enter()
    {
        _walkBool.Value = true;
    }

    public void Execute()
    {
        // プレイヤーへの方向ベクトルを計算
        Vector2 direction = _player.GetPosition() - _bossHead.Position.Value;

        // 向きの計算（右側なら0、左側なら180）
        if (direction.x > 0)
        {
            _rotationUpdater.UpdateRotation(180f);  // プレイヤーがボスの右側にいる
        }
        else
        {
            _rotationUpdater.UpdateRotation(0f);    // プレイヤーがボスの左側にいる
        }

        // 単位ベクトルを求めて移動方向を正規化
        Vector2 moveDirection = direction.normalized;

        // ボスの位置を更新
        _positionUpdater.UpdatePosition(_bossHead.Position.Value + moveDirection * _moveSpeed * Time.deltaTime);
    }

    public void Exit()
    {
        _walkBool.Value = false;
    }
    
    public void Dispose()
    {
        _walkBool.Dispose();
    }
}
