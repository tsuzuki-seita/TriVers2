using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttributeType 
{ 
    Red = 0, 
    Blue = 1, 
    Green = 2 
}

public class DamageCalculator
{
    public static float CalculateDamage(AttributeType attacker, AttributeType defender) {
        if ((attacker == AttributeType.Red && defender == AttributeType.Green) ||
            (attacker == AttributeType.Blue && defender == AttributeType.Red) ||
            (attacker == AttributeType.Green && defender == AttributeType.Blue)) 
        {
            return 2.0f;
        } 
        else if ((attacker == AttributeType.Red && defender == AttributeType.Blue) ||
                (attacker == AttributeType.Blue && defender == AttributeType.Green) ||
                (attacker == AttributeType.Green && defender == AttributeType.Red)) 
        {
            return 0.5f;
        }
        return 1.0f;
    }
}
