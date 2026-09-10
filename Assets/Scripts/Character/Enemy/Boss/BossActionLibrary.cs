using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using System.Threading;

public class BossActionLibrary
{
    private readonly IBossStateContext _context;
    private readonly IPlayerDamageable _playerDamage;

    public BossActionLibrary(IBossStateContext context, IPlayerDamageable playerDamage)
    {
        _context = context;
        _playerDamage = playerDamage;
    }

    // ── Atoms (private building blocks) ────────────────────────────────────

    private async UniTask WalkAtom(CancellationToken ct)
    {
        _context.SetAnimation(BossState.Walk);
        while (Vector2.Distance(_context.Position.Value, _context.Player.GetPosition()) > _context.AttackDistance)
        {
            ct.ThrowIfCancellationRequested();
            Vector2 direction = _context.Player.GetPosition() - _context.Position.Value;
            _context.UpdateRotation(direction.x > 0f ? 180f : 0f);
            _context.UpdatePosition(_context.Position.Value + direction.normalized * _context.MoveSpeed * Time.deltaTime);
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: ct);
        }
    }

    private async UniTask SwordAttackAtom(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        Vector3 swordRot = _context.Rotation.Value == 0f
            ? new Vector3(0f, 180f, -45f)
            : new Vector3(0f, 0f, -45f);
        _context.SetAnimation(BossState.Attack);
        _context.TriggerSwordAttack(new SwordAttackParams(_context.SwordDamage, swordRot, _context.Attribute.Value));
        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: ct);
    }

    private async UniTask MagicChargeAtom(CancellationToken ct)
    {
        _context.SetAnimation(BossState.MagicCharge);
        await UniTask.Delay(TimeSpan.FromSeconds(_context.MagicChargeTime), cancellationToken: ct);
    }

    private static string GetAttributeColorCode(AttributeType attribute) => attribute switch
    {
        AttributeType.Red => "#FF0061",
        AttributeType.Blue => "#6E4EF5",
        _ => "#03B46B",
    };

    private async UniTask MagicReleaseAtom(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        bool isFacingRight = _context.Rotation.Value == 180f;
        float velocity = isFacingRight ? _context.MagicVelocity : -_context.MagicVelocity;
        bool isFlip = isFacingRight;
        string colorCode = GetAttributeColorCode(_context.Attribute.Value);
        _context.SetAnimation(BossState.MagicRelease);
        _context.TriggerMagicAttack(new MagicAttackParams(_context.MagicDamage, velocity, isFlip, colorCode, _context.Attribute.Value));
        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: ct);
    }

    private async UniTask LaughAtom(float duration, CancellationToken ct)
    {
        _context.SetAnimation(BossState.Laugh);
        await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: ct);
    }

    private async UniTask AoeAttackAtom(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        Color flashColor = ColorUtility.TryParseHtmlString(GetAttributeColorCode(_context.Attribute.Value), out Color c)
            ? c
            : Color.white;
        _context.TriggerScreenFlash(flashColor, _context.AoeFlashDuration);
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_context.AoeFlashDuration), cancellationToken: ct);
        }
        catch (OperationCanceledException)
        {
            _context.CancelScreenFlash();
            throw;
        }
        _playerDamage.TakeAreaDamage(_context.AoeDamage, _context.Attribute.Value);
    }

    private async UniTask DamageAtom(float knockbackDir, CancellationToken ct)
    {
        _context.SetAnimation(BossState.Damage);
        Vector2 startPos = _context.Position.Value;
        Vector2 endPos = startPos + new Vector2(knockbackDir, 0f) * _context.KnockbackForce;
        DOTween.To(
            () => _context.Position.Value,
            v => _context.UpdatePosition(v),
            endPos,
            _context.DamageInterval
        ).SetEase(Ease.InOutQuart);
        await UniTask.Delay(TimeSpan.FromSeconds(_context.DamageInterval), cancellationToken: ct);
    }

    // ── Behaviors (public composed sequences) ──────────────────────────────

    public async UniTask AttackSword(CancellationToken ct)
    {
        await WalkAtom(ct);
        await SwordAttackAtom(ct);
    }

    public async UniTask AttackMagic(CancellationToken ct)
    {
        await MagicChargeAtom(ct);
        await MagicReleaseAtom(ct);
    }

    public async UniTask Laugh(CancellationToken ct)
    {
        await LaughAtom(_context.LaughTime, ct);
    }

    public async UniTask AttackMagicWithLaugh(CancellationToken ct)
    {
        await LaughAtom(_context.AoeTelegraphTime, ct);
        await AoeAttackAtom(ct);
    }

    public async UniTask RunDamageSequence(float knockbackDir, CancellationToken ct)
    {
        await DamageAtom(knockbackDir, ct);
    }
}
