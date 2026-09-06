using System;
using UnityEngine;
using UnityEngine.UI;

public interface IBossView
{
    void Initialize(int maxHp);
    void UpdatePosition(Vector2 pos);
    void UpdateRotation(float angle);
    void UpdateAnimation(BossState state);
    void UpdateHP(int hp);
    void UpdateAttribute(AttributeType attribute);
    void SpawnSwordAttack(SwordAttackParams p);
    void SpawnMagicAttack(MagicAttackParams p);
}

public class BossView : MonoBehaviour, IBossView
{
    private AttributeType _currentAttribute = AttributeType.Red;

    [SerializeField] private Animator _animator;
    [SerializeField] private Slider hpSlider;

    [SerializeField] private GameObject BossAttackPoint;
    [SerializeField] private GameObject swordAttackPrefab;
    [SerializeField] private GameObject magicAttackPrefab;

    [SerializeField] private GameObject BlueAura;
    [SerializeField] private GameObject GreenAura;
    [SerializeField] private GameObject RedAura;

    [SerializeField] private GameObject ResultPanel;
    [SerializeField] private Text ResultText;

    private int _maxHp = 100;

    private void Awake()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    public void Initialize(int maxHp)
    {
        _maxHp = maxHp;
    }

    public void UpdatePosition(Vector2 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }

    public void UpdateRotation(float angle)
    {
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    public void UpdateAnimation(BossState state)
    {
        _animator.SetBool("idle", false);
        _animator.SetBool("walk", false);
        _animator.SetBool("casting", false);
        _animator.SetBool("laugh", false);

        switch (state)
        {
            case BossState.Idle:
                break;
            case BossState.Walk:
                _animator.SetBool("walk", true);
                break;
            case BossState.Attack:
                _animator.Play("attack");
                break;
            case BossState.MagicCharge:
                _animator.SetBool("casting", true);
                break;
            case BossState.MagicRelease:
                break;
            case BossState.Laugh:
                _animator.SetBool("laugh", true);
                break;
            case BossState.Damage:
                _animator.SetTrigger("hurt");
                break;
            case BossState.Dead:
                _animator.SetTrigger("die");
                OnDead();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void UpdateHP(int hp)
    {
        hpSlider.value = hp / (float)_maxHp;
    }

    public void UpdateAttribute(AttributeType attribute)
    {
        _currentAttribute = attribute;
        RedAura.SetActive(_currentAttribute == AttributeType.Red);
        GreenAura.SetActive(_currentAttribute == AttributeType.Green);
        BlueAura.SetActive(_currentAttribute == AttributeType.Blue);
    }

    public void SpawnSwordAttack(SwordAttackParams p)
    {
        GameObject attack = Instantiate(swordAttackPrefab, BossAttackPoint.transform.position, Quaternion.Euler(p.Rotation));
        SetAttackParams(attack, p.Damage, p.Attribute);
    }

    public void SpawnMagicAttack(MagicAttackParams p)
    {
        GameObject attack = Instantiate(magicAttackPrefab, BossAttackPoint.transform.position, Quaternion.identity);
        SetAttackParams(attack, p.Damage, p.Attribute);
        var magicAttack = attack.GetComponent<MagicAttack>();
        magicAttack.velosity = p.Velocity;
        magicAttack.casterName = gameObject.tag;
        var renderer = attack.GetComponent<SpriteRenderer>();
        renderer.flipX = p.IsFlip;
        if (ColorUtility.TryParseHtmlString(p.ColorCode, out Color color))
            renderer.color = color;
    }

    private void SetAttackParams(GameObject attack, float damage, AttributeType attribute)
    {
        AttackParamator proj = attack.GetComponent<AttackParamator>();
        if (proj == null) return;
        proj.damage = damage;
        proj.attackerAttribute = attribute;
        proj.team = Team.Enemy;
        proj.attackerTransform = transform;
        proj.knockbackDirection = transform.eulerAngles.y >= 90f ? 1f : -1f;
    }

    private void OnDead()
    {
        RedAura.SetActive(false);
        GreenAura.SetActive(false);
        BlueAura.SetActive(false);
        hpSlider.gameObject.SetActive(false);
        ResultPanel.SetActive(true);
        ResultText.text = "You Win!";
        ResultText.color = Color.green;
    }

    private void OnDestroy()
    {
        RedAura.SetActive(false);
        GreenAura.SetActive(false);
        BlueAura.SetActive(false);
        hpSlider.gameObject.SetActive(false);
    }
}
