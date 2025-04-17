using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;

public class PlayerPresenter : MonoBehaviour
{
    private Player _player;
    private PlayerView _playerView;

    private void Start()
    {
        _player = new Player();
        _playerView = GetComponent<PlayerView>();

        //以下はViewからの入力をPlayerに渡すための購読
        _playerView.MoveDirection
            .Subscribe(delta => _player.Move(delta))
            .AddTo(this);

        _playerView.ChangeAttribute
            .Subscribe(_ => _player.ChangeAttribute())
            .AddTo(this);

        _playerView.AttackSword
            .Subscribe(_ => _player.Attack())
            .AddTo(this);

        _playerView.MagicChargeStart
            .Subscribe(_ => _player.StartMagicCharge())
            .AddTo(this);

        _playerView.MagicRelease
            .Subscribe(_ => _player.ReleaseMagic())
            .AddTo(this);

        //以下はPlayerの状態をViewに反映するための購読
        _player.Position
            .Subscribe(pos => _playerView.UpdatePosition(pos))
            .AddTo(this);

        _player.State
            .Subscribe(state => _playerView.UpdateAnimation(state))
            .AddTo(this);

        _player.HP
            .Subscribe(hp => _playerView.UpdateHP(hp))
            .AddTo(this);

        _player.Attribute   
            .Subscribe(attribute => _playerView.UpdateAttribute(attribute))
            .AddTo(this);
    }
}
