using UniRx;
using UnityEngine;
using VContainer;

public class BossPresenter : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    [Inject]
    public void Construct(BossModel model, IBossView view, IBossCollisionSource collisionSource, BossAI bossAI, IScreenEffectView screenEffectView)
    {
        view.Initialize(BossModel.InitialHP);

        model.HP.Subscribe(hp => view.UpdateHP(hp)).AddTo(_disposables);
        model.Position.Subscribe(pos => view.UpdatePosition(pos)).AddTo(_disposables);
        model.Rotation.Subscribe(rot => view.UpdateRotation(rot)).AddTo(_disposables);
        model.Attribute.Subscribe(attr => view.UpdateAttribute(attr)).AddTo(_disposables);
        model.Animation.Subscribe(anim => view.UpdateAnimation(anim)).AddTo(_disposables);
        model.OnSwordAttack.Subscribe(p => view.SpawnSwordAttack(p)).AddTo(_disposables);
        model.OnMagicAttack.Subscribe(p => view.SpawnMagicAttack(p)).AddTo(_disposables);
        model.OnScreenFlash.Subscribe(p => screenEffectView.Flash(p.Color, p.Duration)).AddTo(_disposables);
        model.OnScreenFlashCancel.Subscribe(_ => screenEffectView.CancelFlash()).AddTo(_disposables);

        collisionSource.CollisionInfo
            .Where(info => model.CanReceiveAttackFrom(info.attackParam))
            .Subscribe(info => bossAI.InterruptWithDamage(info))
            .AddTo(_disposables);

        model.IsDead
            .Where(isDead => isDead)
            .Subscribe(_ => _disposables.Dispose())
            .AddTo(_disposables);

        bossAI.Start();
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
