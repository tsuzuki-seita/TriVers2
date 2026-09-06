using UniRx;
using UnityEngine;
using VContainer;

public class PlayerPresenter : MonoBehaviour
{
    private Player _player;
    private IPlayerView _playerView;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    [Inject]
    public void Construct(Player player, IPlayerInput playerInput, IPlayerView playerView, IPlayerCollisionSource playerCollisionSource)
    {
        _player = player;
        _playerView = playerView;

        _playerView.Initialize(_player.HP.Value, _player.maxMagicChargeTime);

        playerInput.MoveDirection
            .Subscribe(delta => _player.Move(delta))
            .AddTo(_disposables);

        playerInput.ChangeAttribute
            .Subscribe(_ => _player.ChangeAttribute())
            .AddTo(_disposables);

        playerInput.AttackSword
            .Subscribe(_ => _player.Attack())
            .AddTo(_disposables);

        playerInput.MagicChargeStart
            .Subscribe(_ => _player.StartMagicCharge())
            .AddTo(_disposables);

        playerInput.MagicRelease
            .Subscribe(_ => _player.CheckMagicCharge())
            .AddTo(_disposables);

        playerCollisionSource.CollisionInfo
            .Subscribe(collider => _player.OnDamaged(collider))
            .AddTo(_disposables);

        _player.Position
            .Subscribe(pos => _playerView.UpdatePosition(pos))
            .AddTo(_disposables);

        _player.Rotation
            .Subscribe(rot => _playerView.UpdateRotation(rot))
            .AddTo(_disposables);

        _player.State
            .Subscribe(state => _playerView.UpdateAnimation(state))
            .AddTo(_disposables);

        _player.HP
            .Subscribe(hp => _playerView.UpdateHP(hp))
            .AddTo(_disposables);

        _player.CurrentChargeTime
            .Subscribe(chargeTime => _playerView.UpdateMagicCharge(chargeTime))
            .AddTo(_disposables);

        _player.Attribute
            .Subscribe(attribute => _playerView.UpdateAttribute(attribute))
            .AddTo(_disposables);

        _player.SwordAttackTrigger
            .Where(trigger => trigger)
            .Subscribe(_ => _playerView.SwordAttack(_player.swordDamage, _player.swordRotation))
            .AddTo(_disposables);

        _player.MagicAttackTrigger
            .Where(trigger => trigger)
            .Subscribe(_ => _playerView.MagicAttack(_player.magicDamage, _player.magicVelocity, _player.magicFlip, _player.magicColorCode))
            .AddTo(_disposables);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
