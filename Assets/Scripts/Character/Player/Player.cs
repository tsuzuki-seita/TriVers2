using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public enum PlayerState
{
    Idle = 0,
    Walk = 1,
    Attack = 2,
    MagicCharge = 3,
    MagicRelease = 4,
    Damage = 5,
    Dead = 6,
}

public class Player : Character
{
    private const int InitialHP = 100;
    private const float MinPosition = -2f;
    private const float MaxPosition = 7.5f;

    private ReactiveProperty<Vector2> _position { get; } = new ReactiveProperty<Vector2>(Vector2.zero);
    private ReactiveProperty<PlayerState> _state = new ReactiveProperty<PlayerState>(PlayerState.Idle);
    private ReactiveProperty<AttributeType> _attribute { get; } = new ReactiveProperty<AttributeType>(AttributeType.Red);
    private ReactiveProperty<int> _hp { get; } = new ReactiveProperty<int>(InitialHP);

    public IReadOnlyReactiveProperty<Vector2> Position => _position;
    public IReadOnlyReactiveProperty<PlayerState> State => _state;
    public IReadOnlyReactiveProperty<AttributeType> Attribute => _attribute;
    public IReadOnlyReactiveProperty<int> HP => _hp;

    private bool isDamageAnimating = false;
    private bool isChargingMagic = false;
    private bool isAttacking = false;
    private float idleTimer = 0f;
    private float walkwaitTime = 0.1f; // 待機時間
    private float attackInterval = 0.5f; // 攻撃間隔
    private float castingInterval = 0.5f; // チャージ間隔
    private float damageInterval = 0.5f; // ダメージ間隔
    private const float MaxMagicChargeTime = 2.0f;
    private float currentChargeTime = 0f;

    private readonly CompositeDisposable disposables = new CompositeDisposable(); // 購読を管理するためのCompositeDisposable

    public void Move(Vector2 delta)
    {
        if (isChargingMagic) return;
        if (isDamageAnimating) return;
        if (isAttacking) return;
        if (delta == Vector2.zero)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= walkwaitTime)
            {
                _state.Value = PlayerState.Idle;
            }
        }
        else
        {
            idleTimer = 0f;
            delta = delta.normalized * 0.05f; 
            _position.Value = new Vector2(
                (_position.Value.x + delta.x),
                Mathf.Clamp(_position.Value.y + delta.y, MinPosition, MaxPosition)
            );
            _state.Value = PlayerState.Walk;
        }
    }

    public void ChangeAttribute()
    {
        _attribute.Value = (AttributeType)(((int)_attribute.Value + 1) % Enum.GetValues(typeof(AttributeType)).Length);
    }

    public override void Attack()
    {
        AttackSword();
    }

    public void AttackSword()
    {
        if (isChargingMagic) return;
        if (isDamageAnimating) return;
        if (isAttacking) return;
        _state.Value = PlayerState.Attack;
        isAttacking = true;
        Observable.Timer(TimeSpan.FromSeconds(0.1f))
                  .Subscribe(_ => _state.Value = PlayerState.Idle)
                  .AddTo(disposables);
        Observable.Timer(TimeSpan.FromSeconds(attackInterval))
                  .Subscribe(_ => isAttacking = false)
                  .AddTo(disposables);
    }

    public void StartMagicCharge()
    {
        if (isChargingMagic) return;
        if (isDamageAnimating) return;
        if (isAttacking) return;
        _state.Value = PlayerState.MagicCharge;
        isChargingMagic = true;
        currentChargeTime = 0f;
        Debug.Log("Magic charge started!");

        // チャージ中は毎フレーム時間を加算（例：0.1秒ごとに0.1加算）
        Observable.EveryUpdate()
            .TakeWhile(_ => isChargingMagic)
            .Subscribe(_ => {
                currentChargeTime += Time.deltaTime;
                if (currentChargeTime >= MaxMagicChargeTime) {
                    currentChargeTime = MaxMagicChargeTime;
                }
            });
    }

    public void ReleaseMagic() 
    {
        _state.Value = PlayerState.MagicRelease;
        Observable.Timer(TimeSpan.FromSeconds(castingInterval))
                  .Subscribe(_ => isChargingMagic = false)
                  .AddTo(disposables);
        Debug.Log("Magic released!");
    }

    public override void Die()
    {
        _state.Value = PlayerState.Dead;
        disposables.Clear();
    }

    public void OnDamaged(int damage, AttributeType attackerAttribute) 
    {
        if (!isDamageAnimating) {
            isDamageAnimating = true;
            TakeDamage(damage, attackerAttribute);
            PlayDamageAnimation();
        }

        if (HP.Value <= 0)
        {
            Die();
        }
    }
    private void PlayDamageAnimation() 
    {
        _state.Value = PlayerState.Damage;
        Observable.Timer(TimeSpan.FromSeconds(damageInterval))
                  .Subscribe(_ => isDamageAnimating = false)
                  .AddTo(disposables);
    }
}
