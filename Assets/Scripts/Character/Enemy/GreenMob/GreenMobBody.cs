using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenMobBody : Enemy
{
    public GreenMobBody() : base(100, Vector2.zero, 0f, AttributeType.Green)
    {
    }

    public override void Attack()
    {
        ShootArrow();
    }

    public override void Die()
    {
        Debug.Log("GreenMob died.");
    }

    public void ShootArrow() 
    {
        Debug.Log("GreenMob shoots an arrow!");
    }
}
