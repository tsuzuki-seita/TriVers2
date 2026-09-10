using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedMobBody : Enemy
{
    public RedMobBody() : base(100, Vector2.zero, 0f, AttributeType.Red)
    {
    }

    public override void Attack()
    {
        MeleeAttack();
    }

    public override void Die()
    {
        Debug.Log("RedMob died.");
    }

    public void MeleeAttack() 
    {
        Debug.Log("RedMob performs a melee attack!");
    }
}
