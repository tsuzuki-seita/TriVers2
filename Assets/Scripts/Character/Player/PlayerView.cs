using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

// プレイヤーの入力インターフェース
public interface IPlayerInput {
    IObservable<Vector2> MoveDirection { get; }
    IObservable<Unit> ChangeAttribute { get; }
    IObservable<Unit> AttackSword { get; }
    IObservable<Unit> MagicChargeStart { get; }
    IObservable<Unit> MagicRelease { get; }
}

public class PlayerView : MonoBehaviour, IPlayerInput
{
    private Subject<Vector2> moveDirectionSubject = new Subject<Vector2>();
    private Subject<Unit> changeAttributeSubject = new Subject<Unit>();
    private Subject<Unit> attackSwordSubject = new Subject<Unit>();
    private Subject<Unit> magicChargeStartSubject = new Subject<Unit>();
    private Subject<Unit> magicReleaseSubject = new Subject<Unit>();

    public IObservable<Vector2> MoveDirection => moveDirectionSubject;
    public IObservable<Unit> ChangeAttribute => changeAttributeSubject;
    public IObservable<Unit> AttackSword => attackSwordSubject;
    public IObservable<Unit> MagicChargeStart => magicChargeStartSubject;
    public IObservable<Unit> MagicRelease => magicReleaseSubject;

    public Animator _animator;

    private AttributeType currentAttribute = AttributeType.Red;
    [SerializeField]
    private Slider hpSlider;
    [SerializeField]
    private Slider magicChargeSlider;

    [SerializeField]
    private GameObject PlayerAttackPoint;
    [SerializeField]
    private GameObject swordAttackPrefab;
    [SerializeField]
    private GameObject magicAttackPrefab;

    [SerializeField]
    private GameObject BlueAura;
    [SerializeField]
    private GameObject GreenAura;
    [SerializeField]
    private GameObject RedAura;

    [SerializeField]
    private GameObject ResultPanel;
    [SerializeField]
    private Text ResultText;

    public int maxHp = 100;
    public float magicChargeTime = 0.5f; // 魔法チャージの最大時間

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update() 
    {
        moveDirectionSubject.OnNext(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            changeAttributeSubject.OnNext(Unit.Default);
        }

        if (Input.GetMouseButtonDown(0))
        {
            attackSwordSubject.OnNext(Unit.Default);
        }

        if (Input.GetMouseButtonDown(1))
        {
            magicChargeStartSubject.OnNext(Unit.Default);
        }

        if (Input.GetMouseButtonUp(1))
        {
            magicReleaseSubject.OnNext(Unit.Default);
        }
    }

    public void UpdatePosition(Vector2 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }

    public void UpdateRotation(float angle)
    {
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    public void UpdateAnimation(BossState state)
    {
        // 全boolを一度falseにリセット
        _animator.SetBool("idle", false);

        switch (state)
        {
            case BossState.Idle:
                _animator.SetBool("walk",false);
                _animator.SetBool("casting", false);
                break;
            case BossState.Walk:
                _animator.SetBool("walk",true);
                break;
            case BossState.Attack:
                _animator.Play("attack");
                break;
            case BossState.MagicCharge:
                _animator.SetBool("casting", true);
                break;
            case BossState.MagicRelease:
                _animator.SetBool("walk",false);
                _animator.SetBool("casting", false);
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
        hpSlider.value = hp / (float)maxHp;
    }

    public void UpdateMagicCharge(float chargeAmount)
    {
        magicChargeSlider.value = chargeAmount / magicChargeTime;
    }

    public void UpdateAttribute(AttributeType attribute)
    {
        currentAttribute = attribute;
        RedAura.SetActive(currentAttribute == AttributeType.Red);
        GreenAura.SetActive(currentAttribute == AttributeType.Green);
        BlueAura.SetActive(currentAttribute == AttributeType.Blue);
    }

    public void SwordAttack(float swordDamage,Vector3 swordRotation = default)
    {
        GameObject attack = Instantiate(swordAttackPrefab, PlayerAttackPoint.transform.position, Quaternion.Euler(swordRotation));
        SetAttackAttribute(attack, swordDamage);
    }

    public void MagicAttack(float magicDamage,float magicVelocity = 5f,bool isFlip = false, string colorCode = "00FF98")
    {
        GameObject attack = Instantiate(magicAttackPrefab, PlayerAttackPoint.transform.position, Quaternion.identity);
        SetAttackAttribute(attack, magicDamage);
        var magicattack = attack.GetComponent<MagicAttack>();
        magicattack.velosity = magicVelocity;
        magicattack.casterName = this.gameObject.tag;

        var renderer = attack.GetComponent<SpriteRenderer>();
        renderer.flipX = isFlip;
        if (ColorUtility.TryParseHtmlString(colorCode, out Color color))
        {
            renderer.color = color;
        }
    }

    private void SetAttackAttribute(GameObject attack, float damage)
    {
        AttackParamator proj = attack.GetComponent<AttackParamator>();
        if (proj != null)
        {
            proj.damage = damage;
            proj.attackerAttribute = currentAttribute;
            proj.team = Team.Player; // プレイヤーの攻撃であることを設定
        }
    }

    private void OnDestroy()
    {
        Dispose(); // Ensure proper cleanup when the object is destroyed
    }

    private void OnDead()
    {
        moveDirectionSubject.OnCompleted();
        changeAttributeSubject.OnCompleted();
        attackSwordSubject.OnCompleted();
        magicChargeStartSubject.OnCompleted();
        magicReleaseSubject.OnCompleted();

        BlueAura.SetActive(false);
        GreenAura.SetActive(false);
        RedAura.SetActive(false);
        hpSlider.gameObject.SetActive(false);

        ResultPanel.SetActive(true);
        ResultText.text = "You Died!";
        ResultText.color = Color.red;
    }

    private void Dispose()
    {
        // Dispose of all subjects and subscriptions
        moveDirectionSubject.Dispose();
        changeAttributeSubject.Dispose();
        attackSwordSubject.Dispose();
        magicChargeStartSubject.Dispose();
        magicReleaseSubject.Dispose();
    }
}
