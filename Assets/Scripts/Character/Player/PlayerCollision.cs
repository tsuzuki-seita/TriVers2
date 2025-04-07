using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public Player playerModel;

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        var attack = collision.GetComponent<EnemyAttack>();
        if (attack != null) 
        {
            playerModel.OnDamaged(attack.Damage, attack.Attribute);
        }
    }
}
