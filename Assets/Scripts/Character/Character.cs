using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public abstract class Character
{
    public ReactiveProperty<AttributeType> Attribute { get; protected set; }
    public ReactiveProperty<int> HP { get; protected set; }

    public abstract void Attack();
    public abstract void Die();
    
    public virtual void TakeDamage(int damage, AttributeType attackerAttribute) 
    {
        float multiplier = DamageCalculator.CalculateDamage(attackerAttribute, this.Attribute.Value);
        int finalDamage = (int)(damage * multiplier);
        Console.WriteLine($"Took {finalDamage} damage!");
    }
}
