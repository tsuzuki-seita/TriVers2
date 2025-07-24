using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;

public class BossPresenter : MonoBehaviour
{
    private BossHead _bossHead;
    private BossView _bossView;
    private CharacterCollision _bossCollision;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    private void Start()
    {
        _bossHead = GetComponent<BossHead>();
        _bossView = GetComponent<BossView>();
        _bossCollision = GetComponent<CharacterCollision>();

        _bossView.maxHp = _bossHead.HP.Value;

        //以下はViewからの入力をBossHeadに渡すための購読
        _bossView.AnimationTime
            .Subscribe(time => _bossHead.UpdateAnimationTime(time))
            .AddTo(_disposables);

        // PlayerCollisionからのダメージ通知をPlayerに渡すための購読
        _bossCollision.CollisionInfo
            .Subscribe(collider => _bossHead.OnDamaged(collider))
            .AddTo(_disposables);

        //以下はPlayerの状態をViewに反映するための購読
        _bossHead.BossAttribute
            .Subscribe(attribute => _bossView.UpdateAttribute(attribute))
            .AddTo(_disposables);

        _bossHead.stateController.idleState.IdleBool
            .Where(trigger => trigger)
            .Subscribe(_ => _bossView.UpdateAnimation(BossState.Idle))
            .AddTo(_disposables);

        _bossHead.stateController.walkState.WalkBool
            .Where(trigger => trigger)
            .Subscribe(_ => _bossView.UpdateAnimation(BossState.Walk))
            .AddTo(_disposables);

        _bossHead.HP
            .Subscribe(hp => _bossView.UpdateHP(hp))
            .AddTo(_disposables);

        _bossHead.Position
            .Subscribe(_ => _bossView.UpdatePosition(_))
            .AddTo(_disposables);

        _bossHead.Angle
            .Subscribe(angle => _bossView.UpdateRotation(angle))
            .AddTo(_disposables);

        _bossHead.stateController.attackState.AttackTrigger
            .Where(trigger => trigger)
            .Subscribe(_ =>
            {
                _bossView.UpdateAnimation(BossState.Attack);
                _bossView.AttackSword(_bossHead.swordDamage, _bossHead.swordRotation);
            })
            .AddTo(_disposables);

        _bossHead.stateController.magicChargeState.MagicChargeBool
            .Where(trigger => trigger)
            .Subscribe(_ => _bossView.UpdateAnimation(BossState.MagicCharge))
            .AddTo(_disposables);

        _bossHead.stateController.magicReleaseState.MagicReleaseTrigger
            .Where(trigger => trigger)
            .Subscribe(_ =>
            {
                _bossView.UpdateAnimation(BossState.Idle);
                _bossView.MagicRelease(_bossHead.magicDamage, _bossHead.magicVelocity, _bossHead.magicFlip, _bossHead.magicColorCode);
            })
            .AddTo(_disposables);

        _bossHead.stateController.laughState.LaughTrigger
            .Where(trigger => trigger)
            .Subscribe(_ => _bossView.UpdateAnimation(BossState.Laugh))
            .AddTo(_disposables);

        _bossHead.stateController.damageState.DamageTrigger
            .Where(trigger => trigger)
            .Subscribe(_ =>
            {
                _bossView.UpdateAnimation(BossState.Damage);
            })
            .AddTo(_disposables);

        _bossHead.stateController.dieState.DeadTrigger
            .Where(trigger => trigger)
            .Subscribe(_ => _bossView.UpdateAnimation(BossState.Dead))
            .AddTo(_disposables);

        _bossHead.IsDead
            .Where(isDead => isDead)
            .Subscribe(_ =>
            {
                _disposables.Dispose();
            })
            .AddTo(_disposables);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
