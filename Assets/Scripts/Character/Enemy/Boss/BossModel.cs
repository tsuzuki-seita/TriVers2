using System;
using UniRx;
using UnityEngine;

public interface IBossStateContext
{
    IReadOnlyReactiveProperty<Vector2> Position { get; }
    IReadOnlyReactiveProperty<float> Rotation { get; }
    IReadOnlyReactiveProperty<AttributeType> Attribute { get; }
    IReadOnlyReactiveProperty<int> HP { get; }
    IReadOnlyReactiveProperty<bool> IsDead { get; }
    IPlayerReadModel Player { get; }
    float MoveSpeed { get; }
    float SwordDamage { get; }
    float MagicDamage { get; }
    float MagicVelocity { get; }
    float DamageInterval { get; }
    float KnockbackForce { get; }
    float AttackDistance { get; }
    float MagicChargeTime { get; }
    float LaughTime { get; }
    float AoeTelegraphTime { get; }
    float AoeFlashDuration { get; }
    float AoeDamage { get; }
    void UpdatePosition(Vector2 position);
    void UpdateRotation(float rotation);
    void SetAnimation(BossState animation);
    void ChangeAttribute(AttributeType attribute);
    void TriggerSwordAttack(SwordAttackParams p);
    void TriggerMagicAttack(MagicAttackParams p);
    void TriggerScreenFlash(Color color, float duration);
    void CancelScreenFlash();
    void TakeDamage(float damage, AttributeType attackerAttribute);
    void Die();
}

public sealed class BossModelSettings
{
    public Vector2 InitialPosition { get; set; }
    public float InitialAngle { get; set; }
    public float MoveSpeed { get; set; }
    public float SwordDamage { get; set; }
    public float MagicDamage { get; set; }
    public float MagicVelocity { get; set; }
    public float DamageInterval { get; set; }
    public float KnockbackForce { get; set; }
    public float AttackDistance { get; set; }
    public float MagicChargeTime { get; set; }
    public float LaughTime { get; set; }
    public float AoeTelegraphTime { get; set; }
    public float AoeFlashDuration { get; set; }
    public float AoeDamage { get; set; }
}

public class BossModel : Character, IBossStateContext, IDisposable
{
    public const int InitialHP = 100;

    private readonly ReactiveProperty<BossState> _animation = new ReactiveProperty<BossState>(BossState.Idle);
    private readonly Subject<SwordAttackParams> _swordAttackEvent = new Subject<SwordAttackParams>();
    private readonly Subject<MagicAttackParams> _magicAttackEvent = new Subject<MagicAttackParams>();
    private readonly Subject<ScreenFlashParams> _screenFlashEvent = new Subject<ScreenFlashParams>();
    private readonly Subject<Unit> _screenFlashCancelEvent = new Subject<Unit>();

    public BossModel(IPlayerReadModel player, BossModelSettings settings)
        : base(InitialHP, settings.InitialPosition, settings.InitialAngle, AttributeType.Red, Team.Enemy)
    {
        Player = player;
        MoveSpeed = settings.MoveSpeed;
        SwordDamage = settings.SwordDamage;
        MagicDamage = settings.MagicDamage;
        MagicVelocity = settings.MagicVelocity;
        DamageInterval = settings.DamageInterval;
        KnockbackForce = settings.KnockbackForce;
        AttackDistance = settings.AttackDistance;
        MagicChargeTime = settings.MagicChargeTime;
        LaughTime = settings.LaughTime;
        AoeTelegraphTime = settings.AoeTelegraphTime;
        AoeFlashDuration = settings.AoeFlashDuration;
        AoeDamage = settings.AoeDamage;
    }

    public IPlayerReadModel Player { get; }
    public float MoveSpeed { get; }
    public float SwordDamage { get; }
    public float MagicDamage { get; }
    public float MagicVelocity { get; }
    public float DamageInterval { get; }
    public float KnockbackForce { get; }
    public float AttackDistance { get; }
    public float MagicChargeTime { get; }
    public float LaughTime { get; }
    public float AoeTelegraphTime { get; }
    public float AoeFlashDuration { get; }
    public float AoeDamage { get; }

    public IReadOnlyReactiveProperty<BossState> Animation => _animation;
    public IObservable<SwordAttackParams> OnSwordAttack => _swordAttackEvent;
    public IObservable<MagicAttackParams> OnMagicAttack => _magicAttackEvent;
    public IObservable<ScreenFlashParams> OnScreenFlash => _screenFlashEvent;
    public IObservable<Unit> OnScreenFlashCancel => _screenFlashCancelEvent;

    public void UpdatePosition(Vector2 p) => SetPosition(p);
    public void UpdateRotation(float a) => SetRotation(a);
    public void SetAnimation(BossState a) => _animation.Value = a;
    public void ChangeAttribute(AttributeType a) => SetAttribute(a);
    public void TriggerSwordAttack(SwordAttackParams p) => _swordAttackEvent.OnNext(p);
    public void TriggerMagicAttack(MagicAttackParams p) => _magicAttackEvent.OnNext(p);
    public void TriggerScreenFlash(Color color, float duration) => _screenFlashEvent.OnNext(new ScreenFlashParams(color, duration));
    public void CancelScreenFlash() => _screenFlashCancelEvent.OnNext(Unit.Default);

    public override void Attack() { }

    public override void Die()
    {
        _animation.Value = BossState.Dead;
        MarkAsDead();
    }

    public void Dispose()
    {
        _animation?.Dispose();
        _swordAttackEvent?.Dispose();
        _magicAttackEvent?.Dispose();
        _screenFlashEvent?.Dispose();
        _screenFlashCancelEvent?.Dispose();
    }
}
