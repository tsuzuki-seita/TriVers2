using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BossView : MonoBehaviour
{
    private Subject<float> animationTimeSubject = new Subject<float>();
    public IObservable<float> AnimationTime => animationTimeSubject;
    private AttributeType currentAttribute = AttributeType.Red;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Slider hpSlider;

    [SerializeField]
    private GameObject BossAttackPoint;
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

    public int maxHp = 100;

    [SerializeField]
    private GameObject ResultPanel;
    [SerializeField]
    private Text ResultText;

    private void Start()
    {
        _animator = GetComponent<Animator>();
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
                _animator.SetBool("walk", false);
                _animator.SetBool("casting", false);
                _animator.SetBool("laugh", false);
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
                _animator.SetBool("casting", false);
                _animator.SetBool("walk", false);
                break;
            case BossState.Laugh:
                _animator.SetBool("laugh",true);
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
        
        animationTimeSubject.OnNext(_animator.GetCurrentAnimatorStateInfo(0).length);
    }

    public void UpdateHP(int hp)
    {
        hpSlider.value = hp / (float)maxHp;
    }

    public void UpdateAttribute(AttributeType attribute)
    {
        currentAttribute = attribute;
        RedAura.SetActive(currentAttribute == AttributeType.Red);
        GreenAura.SetActive(currentAttribute == AttributeType.Green);
        BlueAura.SetActive(currentAttribute == AttributeType.Blue);
    }

    public void AttackSword(float SwordDamage, Vector3 swordRotation)
    {
        Vector3 SordAttackPointPosition = new Vector3(BossAttackPoint.transform.position.x, BossAttackPoint.transform.position.y - 5, BossAttackPoint.transform.position.z);
        GameObject attack = Instantiate(swordAttackPrefab, BossAttackPoint.transform.position, Quaternion.Euler(swordRotation));
        SetAttackAttribute(attack, SwordDamage);
    }

    public void MagicRelease(float magicDamage,float magicVelocity = 5f,bool isFlip = false, string colorCode = "#FF0061")
    {
        GameObject attack = Instantiate(magicAttackPrefab, BossAttackPoint.transform.position, Quaternion.identity);
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
            proj.team = Team.Enemy; // ボスの攻撃は敵チームとして設定
        }
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
        animationTimeSubject.OnCompleted(); // Ensure proper cleanup when the object is destroyed
        animationTimeSubject.Dispose();
        RedAura.SetActive(false);
        GreenAura.SetActive(false);
        BlueAura.SetActive(false);
        hpSlider.gameObject.SetActive(false);
    }
}
