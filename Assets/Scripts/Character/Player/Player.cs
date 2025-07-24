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

    private ReactiveProperty<Vector2> _position { get; } = new ReactiveProperty<Vector2>(new Vector2(-10f, 0f));
    private ReactiveProperty<float> _rotation { get; } = new ReactiveProperty<float>(180f);
    private ReactiveProperty<BossState> _state = new ReactiveProperty<BossState>(BossState.Idle);
    private ReactiveProperty<AttributeType> _attribute { get; } = new ReactiveProperty<AttributeType>(AttributeType.Red);
    private ReactiveProperty<int> _hp { get; } = new ReactiveProperty<int>(InitialHP);

    public IReadOnlyReactiveProperty<Vector2> Position => _position;
    public IReadOnlyReactiveProperty<float> Rotation => _rotation;
    public IReadOnlyReactiveProperty<BossState> State => _state;
    public IReadOnlyReactiveProperty<AttributeType> Attribute => _attribute;
    public IReadOnlyReactiveProperty<int> HP => _hp;

    private ReactiveProperty<bool> _magicAttackTrigger { get; } = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> MagicAttackTrigger => _magicAttackTrigger;
    private ReactiveProperty<bool> _swordAttackTrigger { get; } = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> SwordAttackTrigger => _swordAttackTrigger;
    private ReactiveProperty<float> _currentChargeTime { get; } = new ReactiveProperty<float>(0f);
    public IReadOnlyReactiveProperty<float> CurrentChargeTime => _currentChargeTime;

    private bool isDamageAnimating = false;
    private bool isChargingMagic = false;
    private bool isAttacking = false;
    private float idleTimer = 0f;
    private float walkwaitTime = 0.1f; // 待機時間
    private float attackInterval = 0.5f; // 攻撃間隔
    private float castingInterval = 0.5f; // チャージ間隔
    private float damageInterval = 0.5f; // ダメージ間隔
    public float maxMagicChargeTime = 0.5f;
    private float knockbackDirection = 1f; // ノックバック方向

    private IDisposable magicChargeDisposable;
    public float swordDamage = 10; // 剣攻撃のダメージ
    public Vector3 swordRotation = new Vector3(0, 0, 45); // 剣の回転角度
    public float magicDamage = 20; // 魔法攻撃のダメージ
    public float magicVelocity = 10f; // 魔法速度
    public bool magicFlip = true;
    public string magicColorCode = "Red"; // 魔法の色コード

    private float previousDeltaX = 0f; // 前回の移動量を保存する変数

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
                _state.Value = BossState.Idle;
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
            _state.Value = BossState.Walk;

            if (previousDeltaX <= 0 && delta.x > 0)
            {
                _rotation.Value = 180f;
            }
            else if (previousDeltaX >= 0 && delta.x < 0)
            {
                _rotation.Value = 0f;
            }

            // 前回の値を更新
            previousDeltaX = delta.x;
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
        _state.Value = BossState.Attack;
        isAttacking = true;

        swordRotation = _rotation.Value == 0f ? new Vector3(0, 180, -45) : new Vector3(0, 0, -45);
        _swordAttackTrigger.Value = true; // 剣攻撃トリガーを発火
        Observable.Timer(TimeSpan.FromSeconds(0.1f))
                  .Subscribe(_ => _state.Value = BossState.Idle)
                  .AddTo(disposables);
        Observable.Timer(TimeSpan.FromSeconds(attackInterval))
                  .Subscribe(_ => isAttacking = false)
                  .AddTo(disposables);
        _swordAttackTrigger.Value = false; // 剣攻撃トリガーをリセット
    }

    public void StartMagicCharge()
    {
        if (isChargingMagic) return;
        if (isDamageAnimating) return;
        if (isAttacking) return;
        _state.Value = BossState.MagicCharge;
        isChargingMagic = true;
        _currentChargeTime.Value = 0f;

        // チャージ中は毎フレーム時間を加算（例：0.1秒ごとに0.1加算）
        magicChargeDisposable = Observable.EveryUpdate()
            .TakeWhile(_ => isChargingMagic)
            .Subscribe(_ =>
            {
                _currentChargeTime.Value += Time.deltaTime;
            })
            .AddTo(disposables);
    }

    public void CheckMagicCharge()
    {
        if (!isChargingMagic) return;

        isChargingMagic = false;
        magicChargeDisposable?.Dispose(); // チャージ更新停止

        if (CurrentChargeTime.Value >= maxMagicChargeTime)
        {
            _state.Value = BossState.MagicRelease;
            ReleaseMagic(); // 発射
        }
        else
        {
            // チャージ未完了なら何も起こさない（アニメーション戻すならここで）
            _state.Value = BossState.MagicRelease;
            Debug.Log("Charge not enough, no magic released.");
        }

        _currentChargeTime.Value = 0f;
    }

    public void ReleaseMagic()
    {
        _state.Value = BossState.MagicRelease;

        var tmp = magicVelocity;
        if (_rotation.Value == 0f)
        {
            magicVelocity *= -1f; // 左向きなら速度を反転
            magicFlip = false;
        }

        switch (_attribute.Value)
        {
            case AttributeType.Red:
                magicColorCode = "#FF0061";
                break;
            case AttributeType.Green:
                magicColorCode = "#00FF98";
                break;
            case AttributeType.Blue:
                magicColorCode = "#FFFFFF";
                break;
        }

        _magicAttackTrigger.Value = true; // 魔法攻撃トリガーを発火
        Observable.Timer(TimeSpan.FromSeconds(castingInterval))
                  .Subscribe(_ => isChargingMagic = false)
                  .AddTo(disposables);

        _magicAttackTrigger.Value = false; // 魔法攻撃トリガーをリセット
        magicVelocity = tmp; // 元の速度に戻す
        magicFlip = true; // 魔法の向きをリセット
    }

    public override void Die()
    {
        _state.Value = BossState.Dead;
        disposables.Clear();
    }

    public void OnDamaged(CollisionInfo info)
    {
        if (isDamageAnimating) return;

        var attack = info.attackParam;
        if (attack.team == Team.Player) return; // プレイヤーからのダメージは無視
        if (_state.Value == BossState.Dead) return; // 既に死んでいる場合は無視

        isDamageAnimating = true;
        TakeDamage(attack.damage, attack.attackerAttribute);
        if (HP.Value <= 0)
        {
            Die();
        }
        
        PlayDamageAnimation();
        if (_position.Value.x - info.hitTransform.position.x < 0f)
        {
            knockbackDirection = -1f; // 左からの攻撃
        }
        else
        {
            knockbackDirection = 1f; // 右からの攻撃
        }
    }

    public override void TakeDamage(float damage, AttributeType attackerAttribute)
    {
        float multiplier = CalculateDamage(attackerAttribute, this._attribute.Value);
        int finalDamage = (int)(damage * multiplier);
        _hp.Value -= finalDamage;
        //Debug.Log($"Took {finalDamage} damage! Remaining HP: {_hp.Value}");
    }

    private void PlayDamageAnimation()
    {
        _state.Value = BossState.Damage;
        Observable.Timer(TimeSpan.FromSeconds(damageInterval))
                  .Subscribe(_ => isDamageAnimating = false)
                  .AddTo(disposables);
    }

    public Vector2 GetPosition()
    {
        return _position.Value;
    }
    
    public AttributeType GetAttribute()
    {
        return _attribute.Value;
    }
}
