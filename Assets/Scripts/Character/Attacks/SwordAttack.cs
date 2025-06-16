using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [SerializeField] float deleteTime = 0.3f; // 剣の攻撃が当たった後に削除されるまでの時間
    void Start()
    {
        Invoke("DestroyObject", deleteTime);
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
