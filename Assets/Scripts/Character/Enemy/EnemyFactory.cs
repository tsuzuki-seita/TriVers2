using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory
{
    public static Enemy CreateEnemy(string type) {
        return type switch {
            "Red" => new RedMobBody(),
            "Blue" => new BlueMobBody(),
            "Green" => new GreenMobBody(),
            "Boss" => new Boss(),
            _ => throw new ArgumentException("Unknown enemy type")
        };
    }
}
