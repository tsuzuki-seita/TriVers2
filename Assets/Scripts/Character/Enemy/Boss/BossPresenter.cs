using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;

public class BossPresenter : MonoBehaviour
{
    private Boss _boss;
    private BossView _bossView;

    private void Start()
    {
        _boss = new Boss();
        _bossView = GetComponent<BossView>();

        //以下はViewからの入力をPlayerに渡すための購読
        _bossView.MoveDirection
            .Subscribe(delta => _boss.Move(delta))
            .AddTo(this);

        _bossView.ChangeAttribute
            .Subscribe(_ => _boss.ChangeAttribute())
            .AddTo(this);

        _bossView.AttackSword
            .Subscribe(_ => _boss.Attack())
            .AddTo(this);

        _bossView.MagicChargeStart
            .Subscribe(_ => _boss.StartMagicCharge())
            .AddTo(this);

        _bossView.MagicRelease
            .Subscribe(_ => _boss.ReleaseMagic())
            .AddTo(this);

        //以下はPlayerの状態をViewに反映するための購読
        _boss.Position
            .Subscribe(pos => _bossView.UpdatePosition(pos))
            .AddTo(this);

        _boss.Rotation
            .Subscribe(rot => _bossView.UpdateRotation(rot))
            .AddTo(this);

        _boss.State
            .Subscribe(state => _bossView.UpdateAnimation(state))
            .AddTo(this);

        _boss.HP
            .Subscribe(hp => _bossView.UpdateHP(hp))
            .AddTo(this);

        _boss.Attribute   
            .Subscribe(attribute => _bossView.UpdateAttribute(attribute))
            .AddTo(this);
    }
}
