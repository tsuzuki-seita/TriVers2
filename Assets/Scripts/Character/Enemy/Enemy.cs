using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character
{
    protected EnemyState state;

    protected Enemy(int initialHp, Vector2 initialPosition, float initialRotation, AttributeType initialAttribute)
        : base(initialHp, initialPosition, initialRotation, initialAttribute, Team.Enemy)
    {
    }

    public virtual void ChangeState(EnemyState newState)
    {
        state = newState;
    }

    public void Act()
    {
        state?.Execute(this);
    }

    public abstract override void Attack();
    public abstract override void Die();
}
