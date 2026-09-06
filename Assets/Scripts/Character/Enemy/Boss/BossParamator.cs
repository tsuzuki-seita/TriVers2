using UnityEngine;

public class BossParamator : MonoBehaviour
{
    [Header("移動速度")]
    [SerializeField] private float _moveSpeed = 1.0f;

    [Header("剣攻撃ダメージ")]
    [SerializeField] private float _swordDamage = 10f;

    [Header("魔法攻撃ダメージ")]
    [SerializeField] private float _magicDamage = 20f;

    [Header("魔法弾速")]
    [SerializeField] private float _magicVelocity = 5.0f;

    [Header("被弾硬直時間（秒）")]
    [SerializeField] private float _damageInterval = 0.5f;

    [Header("ノックバック距離")]
    [SerializeField] private float _knockbackForce = 10f;

    [Header("剣攻撃の間合い")]
    [SerializeField] private float _attackDistance = 2.0f;

    [Header("魔法チャージ時間（秒）")]
    [SerializeField] private float _magicChargeTime = 2.0f;

    [Header("笑い継続時間（秒）")]
    [SerializeField] private float _laughTime = 2.0f;

    [Header("全体攻撃：発動までの予備動作時間（秒）")]
    [SerializeField] private float _aoeTelegraphTime = 1.5f;

    [Header("全体攻撃：画面フラッシュの継続時間（秒）")]
    [SerializeField] private float _aoeFlashDuration = 0.6f;

    [Header("全体攻撃：ダメージ")]
    [SerializeField] private float _aoeDamage = 30f;

    public BossModelSettings CreateSettings() => new BossModelSettings
    {
        InitialPosition = transform.position,
        InitialAngle = transform.eulerAngles.y,
        MoveSpeed = _moveSpeed,
        SwordDamage = _swordDamage,
        MagicDamage = _magicDamage,
        MagicVelocity = _magicVelocity,
        DamageInterval = _damageInterval,
        KnockbackForce = _knockbackForce,
        AttackDistance = _attackDistance,
        MagicChargeTime = _magicChargeTime,
        LaughTime = _laughTime,
        AoeTelegraphTime = _aoeTelegraphTime,
        AoeFlashDuration = _aoeFlashDuration,
        AoeDamage = _aoeDamage,
    };
}
