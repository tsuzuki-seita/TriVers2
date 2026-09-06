using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Player, // プレイヤーのチーム
    Enemy   // 敵のチーム
}
public class AttackParamator : MonoBehaviour
{
    public float damage; // このプロジェクタイルが与えるダメージ
    public AttributeType attackerAttribute; // 攻撃を発射したキャラクターの属性
    public Team team; // この攻撃が属するチーム
    public Transform attackerTransform; // 攻撃したキャラクター本体のTransform
    public float knockbackDirection; // 攻撃時に向いていた方向
}
