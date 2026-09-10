using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public enum AttributeType 
{ 
    Red = 0, 
    Blue = 1, 
    Green = 2 
}

public abstract class Character
{
    private readonly Team _team;
    private readonly ReactiveProperty<Vector2> _position;
    private readonly ReactiveProperty<float> _rotation;
    private readonly ReactiveProperty<AttributeType> _attribute;
    private readonly ReactiveProperty<int> _hp;
    private readonly ReactiveProperty<bool> _isDead = new ReactiveProperty<bool>(false);

    protected Character(int initialHp, Vector2 initialPosition, float initialRotation, AttributeType initialAttribute, Team team)
    {
        _team = team;
        _position = new ReactiveProperty<Vector2>(initialPosition);
        _rotation = new ReactiveProperty<float>(initialRotation);
        _attribute = new ReactiveProperty<AttributeType>(initialAttribute);
        _hp = new ReactiveProperty<int>(initialHp);
    }

    public Team Team => _team;
    public IReadOnlyReactiveProperty<Vector2> Position => _position;
    public IReadOnlyReactiveProperty<float> Rotation => _rotation;
    public IReadOnlyReactiveProperty<AttributeType> Attribute => _attribute;
    public IReadOnlyReactiveProperty<int> HP => _hp;
    public IReadOnlyReactiveProperty<bool> IsDead => _isDead;

    public abstract void Attack();
    public abstract void Die();

    public virtual void TakeDamage(float damage, AttributeType attackerAttribute)
    {
        float multiplier = CalculateDamage(attackerAttribute, _attribute.Value);
        int finalDamage = Mathf.RoundToInt(damage * multiplier);
        _hp.Value = Mathf.Max(0, _hp.Value - finalDamage);
    }

    public bool CanReceiveAttackFrom(AttackParamator attackParam)
    {
        return attackParam != null && attackParam.team != Team && !_isDead.Value;
    }

    protected void SetPosition(Vector2 position)
    {
        _position.Value = position;
    }

    protected void MovePosition(Vector2 delta)
    {
        _position.Value += delta;
    }

    protected void SetRotation(float rotation)
    {
        _rotation.Value = rotation;
    }

    protected void SetAttribute(AttributeType attribute)
    {
        _attribute.Value = attribute;
    }

    protected void CycleAttribute()
    {
        _attribute.Value = NextAttribute(_attribute.Value);
    }

    protected void MarkAsDead()
    {
        _isDead.Value = true;
    }

    protected static AttributeType NextAttribute(AttributeType current)
    {
        return (AttributeType)(((int)current + 1) % Enum.GetValues(typeof(AttributeType)).Length);
    }

    public static float CalculateDamage(AttributeType attacker, AttributeType defender)
    {
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
