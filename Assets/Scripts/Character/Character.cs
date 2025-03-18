using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character
{
    public AttributeType Attribute { get; protected set; }
    public abstract void Attack();
    public virtual void TakeDamage(int damage, AttributeType attackerAttribute) {
        float multiplier = DamageCalculator.CalculateDamage(attackerAttribute, this.Attribute);
        int finalDamage = (int)(damage * multiplier);
        Console.WriteLine($"Took {finalDamage} damage!");
    }
}
