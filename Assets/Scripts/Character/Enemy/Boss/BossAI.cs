using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BossAI : IDisposable
{
    private readonly BossActionLibrary _lib;
    private readonly IBossStateContext _context;
    private readonly CancellationTokenSource _lifetimeCts = new CancellationTokenSource();
    private readonly Func<CancellationToken, UniTask>[] _behaviors;
    private CancellationTokenSource _behaviorCts;
    private bool _isRunningDamage;
    private int _lastBehaviorIndex = -1;

    public BossAI(BossActionLibrary lib, IBossStateContext context)
    {
        _lib = lib;
        _context = context;
        _behaviors = new Func<CancellationToken, UniTask>[]
        {
            _lib.AttackSword,
            _lib.AttackMagic,
            _lib.Laugh,
            _lib.AttackMagicWithLaugh,
        };
    }

    public void Start() => BehaviorLoop(_lifetimeCts.Token).Forget();

    private async UniTask BehaviorLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested && !_context.IsDead.Value)
        {
            _behaviorCts?.Dispose();
            _behaviorCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            try
            {
                await RunNextBehavior(_behaviorCts.Token);
            }
            catch (OperationCanceledException)
            {
                await UniTask.WaitUntil(() => !_isRunningDamage, cancellationToken: ct);
            }
        }
    }

    private async UniTask RunNextBehavior(CancellationToken ct)
    {
        RandomizeAttribute();
        _context.SetAnimation(BossState.Idle);
        int index = PickNextBehaviorIndex();
        _lastBehaviorIndex = index;
        await _behaviors[index](ct);
    }

    private int PickNextBehaviorIndex()
    {
        int index;
        do
        {
            index = UnityEngine.Random.Range(0, _behaviors.Length);
        } while (_behaviors.Length > 1 && index == _lastBehaviorIndex);
        return index;
    }

    public void InterruptWithDamage(CollisionInfo info)
    {
        if (_isRunningDamage || _context.IsDead.Value) return;

        var cts = _behaviorCts;
        if (cts != null && !cts.IsCancellationRequested) cts.Cancel();

        _context.TakeDamage(info.attackParam.damage, info.attackParam.attackerAttribute);

        if (_context.HP.Value <= 0)
        {
            _context.Die();
            _lifetimeCts.Cancel();
            return;
        }

        _isRunningDamage = true;
        RunDamageAsync(info.attackParam.knockbackDirection).Forget();
    }

    private async UniTask RunDamageAsync(float knockbackDir)
    {
        try
        {
            await _lib.RunDamageSequence(knockbackDir, _lifetimeCts.Token);
        }
        finally
        {
            _isRunningDamage = false;
        }
    }

    private void RandomizeAttribute()
    {
        int count = Enum.GetValues(typeof(AttributeType)).Length;
        _context.ChangeAttribute((AttributeType)UnityEngine.Random.Range(0, count));
    }

    public void Dispose()
    {
        _lifetimeCts.Cancel();
        _lifetimeCts.Dispose();
        _behaviorCts?.Dispose();
    }
}
