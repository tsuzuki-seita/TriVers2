using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueMobBody : Enemy
{
    public BlueMobBody() : base(100, Vector2.zero, 0f, AttributeType.Blue)
    {
    }

    public override void Attack()
    {
        CastMagic();
    }

    public override void Die()
    {
        Debug.Log("BlueMob died.");
    }

    public void CastMagic() 
    {
        Debug.Log("BlueMob casts magic!");
    }
}
