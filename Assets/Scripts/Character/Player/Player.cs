using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public override void Attack() {
        Console.WriteLine("Player attacks!");
    }
    public void Move() {
        Console.WriteLine("Player moves.");
    }
    public void ChangeAttribute(AttributeType newAttribute) {
        Attribute = newAttribute;
    }
}
