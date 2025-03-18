using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    protected EnemyState state;
    public void ChangeState(EnemyState newState) 
    {
        state = newState;
    }
    public void Act() {
        state.Execute(this);
    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }
}
