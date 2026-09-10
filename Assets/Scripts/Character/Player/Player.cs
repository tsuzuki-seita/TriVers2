using System;
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

public interface IPlayerReadModel
{
    AttributeType GetAttribute();
    Vector2 GetPosition();
}

public interface IPlayerDamageable
{
    void TakeAreaDamage(float damage, AttributeType attackerAttribute);
}

public class Player : Character, IPlayerReadModel, IPlayerDamageable
{
    private const int InitialHP = 100;
    private const float MinPosition = -2f;
    private const float MaxPosition = 7.5f;

    private readonly ReactiveProperty<BossState> _state = new ReactiveProperty<BossState>(BossState.Idle);
    public IReadOnlyReactiveProperty<BossState> State => _state;

    private readonly ReactiveProperty<bool> _magicAttackTrigger = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> MagicAttackTrigger => _magicAttackTrigger;
    private readonly ReactiveProperty<bool> _swordAttackTrigger = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> SwordAttackTrigger => _swordAttackTrigger;
    private readonly ReactiveProperty<float> _currentChargeTime = new ReactiveProperty<float>(0f);
    public IReadOnlyReactiveProperty<float> CurrentChargeTime => _currentChargeTime;

    private bool isDamageAnimating;
    private bool isChargingMagic;
    private bool isAttacking;
    private float idleTimer;
    private float previousDeltaX;

    private readonly float walkwaitTime = 0.1f;
    private readonly float attackInterval = 0.5f;
    private readonly float castingInterval = 0.5f;
    private readonly float damageInterval = 0.5f;
    private float knockbackDirection = 1f;

    public float maxMagicChargeTime = 0.5f;
    public float swordDamage = 10f;
    public Vector3 swordRotation = new Vector3(0, 0, 45);
    public float magicDamage = 20f;
    public float magicVelocity = 10f;
    public bool magicFlip = true;
    public string magicColorCode = "Red";

    private IDisposable magicChargeDisposable;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    public Player() : base(InitialHP, new Vector2(-10f, 0f), 180f, AttributeType.Red, Team.Player)
    {
    }

    public void Move(Vector2 delta)
    {
        if (isChargingMagic || isDamageAnimating || isAttacking) return;

        if (delta == Vector2.zero)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= walkwaitTime)
            {
                _state.Value = BossState.Idle;
            }

            return;
        }

        idleTimer = 0f;
        Vector2 normalizedDelta = delta.normalized * 0.05f;
        SetPosition(new Vector2(
            Position.Value.x + normalizedDelta.x,
            Mathf.Clamp(Position.Value.y + normalizedDelta.y, MinPosition, MaxPosition)
        ));
        _state.Value = BossState.Walk;

        if (previousDeltaX <= 0f && normalizedDelta.x > 0f)
        {
            SetRotation(180f);
        }
        else if (previousDeltaX >= 0f && normalizedDelta.x < 0f)
        {
            SetRotation(0f);
        }

        previousDeltaX = normalizedDelta.x;
    }

    public void ChangeAttribute()
    {
        CycleAttribute();
    }

    public override void Attack()
    {
        AttackSword();
    }

    public void AttackSword()
    {
        if (isChargingMagic || isDamageAnimating || isAttacking) return;

        _state.Value = BossState.Attack;
        isAttacking = true;
        swordRotation = Rotation.Value == 0f ? new Vector3(0, 180, -45) : new Vector3(0, 0, -45);

        _swordAttackTrigger.Value = true;
        Observable.Timer(TimeSpan.FromSeconds(0.1f))
            .Subscribe(_ => _state.Value = BossState.Idle)
            .AddTo(disposables);
        Observable.Timer(TimeSpan.FromSeconds(attackInterval))
            .Subscribe(_ => isAttacking = false)
            .AddTo(disposables);
        _swordAttackTrigger.Value = false;
    }

    public void StartMagicCharge()
    {
        if (isChargingMagic || isDamageAnimating || isAttacking) return;

        _state.Value = BossState.MagicCharge;
        isChargingMagic = true;
        _currentChargeTime.Value = 0f;

        magicChargeDisposable = Observable.EveryUpdate()
            .TakeWhile(_ => isChargingMagic)
            .Subscribe(_ => _currentChargeTime.Value += Time.deltaTime)
            .AddTo(disposables);
    }

    public void CheckMagicCharge()
    {
        if (!isChargingMagic) return;

        isChargingMagic = false;
        magicChargeDisposable?.Dispose();

        if (CurrentChargeTime.Value >= maxMagicChargeTime)
        {
            _state.Value = BossState.MagicRelease;
            ReleaseMagic();
        }
        else
        {
            _state.Value = BossState.MagicRelease;
            Debug.Log("Charge not enough, no magic released.");
        }

        _currentChargeTime.Value = 0f;
    }

    public void ReleaseMagic()
    {
        _state.Value = BossState.MagicRelease;

        float originalVelocity = magicVelocity;
        if (Rotation.Value == 0f)
        {
            magicVelocity *= -1f;
            magicFlip = false;
        }

        switch (Attribute.Value)
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

        _magicAttackTrigger.Value = true;
        Observable.Timer(TimeSpan.FromSeconds(castingInterval))
            .Subscribe(_ => isChargingMagic = false)
            .AddTo(disposables);

        _magicAttackTrigger.Value = false;
        magicVelocity = originalVelocity;
        magicFlip = true;
    }

    public override void Die()
    {
        _state.Value = BossState.Dead;
        MarkAsDead();
        disposables.Clear();
    }

    public void OnDamaged(CollisionInfo info)
    {
        if (isDamageAnimating) return;

        AttackParamator attack = info.attackParam;
        if (!CanReceiveAttackFrom(attack)) return;

        isDamageAnimating = true;
        TakeDamage(attack.damage, attack.attackerAttribute);
        if (HP.Value <= 0)
        {
            Die();
            return;
        }

        PlayDamageAnimation();
        knockbackDirection = attack.knockbackDirection;
    }

    public void TakeAreaDamage(float damage, AttributeType attackerAttribute)
    {
        if (isDamageAnimating || IsDead.Value) return;

        isDamageAnimating = true;
        TakeDamage(damage, attackerAttribute);
        if (HP.Value <= 0)
        {
            Die();
            return;
        }

        PlayDamageAnimation();
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
        return Position.Value;
    }

    public AttributeType GetAttribute()
    {
        return Attribute.Value;
    }
}
