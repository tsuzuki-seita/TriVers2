using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BossView : MonoBehaviour
{

    public Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update() 
    {
        
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
        
    }
}
