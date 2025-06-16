using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

// まとめて送る構造体 or クラス
public class CollisionInfo
{
    public Transform hitTransform { get; }
    public AttackParamator attackParam { get; }

    public CollisionInfo(Transform hitTransform, AttackParamator attackParam)
    {
        this.hitTransform = hitTransform;
        this.attackParam = attackParam;
    }
}

public class CharacterCollision : MonoBehaviour
{
    private Subject<CollisionInfo> collisionSubject = new Subject<CollisionInfo>();
    public IObservable<CollisionInfo> CollisionInfo => collisionSubject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var attackParam = collision.GetComponent<AttackParamator>();
        if (attackParam != null)
        {
            var info = new CollisionInfo(collision.transform, attackParam);
            collisionSubject.OnNext(info);
        }
    }
}
