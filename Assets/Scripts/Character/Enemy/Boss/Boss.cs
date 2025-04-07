using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy {
    public void SwitchAttribute(AttributeType newAttribute) 
    {
        Attribute.Value = newAttribute;
    }
}
