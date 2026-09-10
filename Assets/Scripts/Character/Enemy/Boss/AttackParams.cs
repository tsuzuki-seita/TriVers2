using UnityEngine;

public readonly struct SwordAttackParams
{
    public readonly float Damage;
    public readonly Vector3 Rotation;
    public readonly AttributeType Attribute;

    public SwordAttackParams(float damage, Vector3 rotation, AttributeType attribute)
    {
        Damage = damage;
        Rotation = rotation;
        Attribute = attribute;
    }
}

public readonly struct MagicAttackParams
{
    public readonly float Damage;
    public readonly float Velocity;
    public readonly bool IsFlip;
    public readonly string ColorCode;
    public readonly AttributeType Attribute;

    public MagicAttackParams(float damage, float velocity, bool isFlip, string colorCode, AttributeType attribute)
    {
        Damage = damage;
        Velocity = velocity;
        IsFlip = isFlip;
        ColorCode = colorCode;
        Attribute = attribute;
    }
}

public readonly struct ScreenFlashParams
{
    public readonly Color Color;
    public readonly float Duration;

    public ScreenFlashParams(Color color, float duration)
    {
        Color = color;
        Duration = duration;
    }
}
