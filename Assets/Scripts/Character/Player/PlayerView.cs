using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

// プレイヤーの入力インターフェース
public interface IPlayerInput {
    IObservable<Vector2> MoveDirection { get; }
    IObservable<Unit> ChangeAttribute { get; }
    IObservable<Unit> AttackSword { get; }
    IObservable<Unit> MagicChargeStart { get; }
    IObservable<Unit> MagicRelease { get; }
}

public class PlayerView : MonoBehaviour, IPlayerInput
{
    private Subject<Vector2> moveDirectionSubject = new Subject<Vector2>();
    private Subject<Unit> changeAttributeSubject = new Subject<Unit>();
    private Subject<Unit> attackSwordSubject = new Subject<Unit>();
    private Subject<Unit> magicChargeStartSubject = new Subject<Unit>();
    private Subject<Unit> magicReleaseSubject = new Subject<Unit>();

    public IObservable<Vector2> MoveDirection => moveDirectionSubject;
    public IObservable<Unit> ChangeAttribute => changeAttributeSubject;
    public IObservable<Unit> AttackSword => attackSwordSubject;
    public IObservable<Unit> MagicChargeStart => magicChargeStartSubject;
    public IObservable<Unit> MagicRelease => magicReleaseSubject;

    public Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update() 
    {
        moveDirectionSubject.OnNext(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            changeAttributeSubject.OnNext(Unit.Default);
        }

        if (Input.GetMouseButtonDown(0))
        {
            attackSwordSubject.OnNext(Unit.Default);
        }

        if (Input.GetMouseButtonDown(1))
        {
            magicChargeStartSubject.OnNext(Unit.Default);
        }

        if (Input.GetMouseButtonUp(1))
        {
            magicReleaseSubject.OnNext(Unit.Default);
        }
    }

    public void UpdatePosition(Vector2 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }

    public void UpdateRotation(float angle)
    {
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    public void UpdateAnimation(PlayerState state)
    {
        // 全boolを一度falseにリセット
        _animator.SetBool("idle", false);

        switch (state)
        {
            case PlayerState.Idle:
                _animator.SetBool("walk",false);
                break;
            case PlayerState.Walk:
                _animator.SetBool("walk",true);
                break;
            case PlayerState.Attack:
                _animator.Play("attack");
                break;
            case PlayerState.MagicCharge:
                _animator.SetBool("casting", true);
                break;
            case PlayerState.MagicRelease:
                _animator.SetBool("walk",false);
                _animator.SetBool("casting", false);
                break;
            case PlayerState.Damage:
                _animator.SetTrigger("hurt");
                break;
            case PlayerState.Dead:
                _animator.SetTrigger("die");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void UpdateHP(int hp)
    {
        throw new NotImplementedException();
    }

    public void UpdateAttribute(AttributeType attribute)
    {
        throw new NotImplementedException();
    }

    private void OnDestroy()
    {
        Dispose(); // Ensure proper cleanup when the object is destroyed
    }

    private void Dispose()
    {
        // Dispose of all subjects and subscriptions
        moveDirectionSubject.Dispose();
        changeAttributeSubject.Dispose();
        attackSwordSubject.Dispose();
        magicChargeStartSubject.Dispose();
        magicReleaseSubject.Dispose();
    }
}
