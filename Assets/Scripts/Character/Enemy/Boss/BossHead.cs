using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using UniRx;
using System.Runtime.CompilerServices;
using System.Threading;

public enum BossState
{
    Idle = 0,
    Walk = 1,
    Attack = 2,
    MagicCharge = 3,
    MagicRelease = 4,
    Laugh = 5,
    Damage = 6,
    Dead = 7,
}

public interface IBossPositionUpdater
{
    void UpdatePosition(Vector2 position);
}
public interface IBossRotationUpdater
{
    void UpdateRotation(float angle);
}

public class BossHead : MonoBehaviour, IBossPositionUpdater, IBossRotationUpdater
{
    public const int initialHP = 100;
    private bool isDamageAnimating = false;
    private const float MaxMagicChargeTime = 2.0f;
    private const float MaxLaughTime = 2.0f;
    private ReactiveProperty<int> _hp { get; } = new ReactiveProperty<int>(initialHP);
    public IReadOnlyReactiveProperty<int> HP => _hp;
    private ReactiveProperty<AttributeType> _bossAttribute { get; } = new ReactiveProperty<AttributeType>(AttributeType.Red);
    public IReadOnlyReactiveProperty<AttributeType> BossAttribute => _bossAttribute;
    private ReactiveProperty<Vector2> _position { get; } = new ReactiveProperty<Vector2>(new Vector2(5f, 0f));
    public IReadOnlyReactiveProperty<Vector2> Position => _position;
    private ReactiveProperty<float> _angle { get; } = new ReactiveProperty<float>(0f);
    public IReadOnlyReactiveProperty<float> Angle => _angle;
    private ReactiveProperty<bool> _isDead { get; } = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> IsDead => _isDead;

    public AttributeType playerAttribute;
    public PlayerPresenter playerPresenter;
    public GameObject playerObject;
    public BossStateController stateController;
    private Transform playerTarget;

    public float moveSpeed = 1.0f; // ボスの移動速度
    public float swordDamage = 10; // 剣攻撃のダメージ
    public Vector3 swordRotation = new Vector3(0, 0, 45); // 剣の回転角度
    public float magicDamage = 20; // 魔法攻撃のダメージ
    public float magicVelocity = 5.0f; // 魔法速度
    public bool magicFlip;
    public string magicColorCode = "Red"; // 魔法の色コード

    private float animationTime = 0f; // アニメーション時間
    private float attackDistance = 2.0f;
    public float damageInterval = 0.5f; // ダメージ間隔
    public float knockbackDirection = 1;
    public float knockbackForce = 10f; // ノックバック力

    private CancellationTokenSource cts = new CancellationTokenSource();

    private readonly CompositeDisposable disposables = new CompositeDisposable(); // 購読を管理するためのCompositeDisposable

    private void Awake()
    {
        // 初期化
        stateController = new BossStateController(this);
        stateController.stateChanged += HandleStateChanged;
        stateController.Initialize(stateController.idleState);
        playerTarget = GameObject.FindWithTag("Player").transform;

        playerPresenter = playerObject.GetComponent<PlayerPresenter>();

        // 購読の初期化
        disposables.Clear();
    }

    private void Update()
    {
        stateController.Execute();
    }

    public void UpdateAnimationTime(float time)
    {
        animationTime = time;
    }

    public void ChangeAttribute(AttributeType newAttribute)
    {
        _bossAttribute.Value = newAttribute;
    }

    public void HandleStateChanged()
    {
        if (IsDead.Value) return; // 既に死んでいる場合は何もしない

        playerAttribute = playerPresenter.GetAttribute();

        animationTime = 0f; // アニメーション時間をリセット

        //enum型の要素数を取得
        int maxCount = Enum.GetNames(typeof(AttributeType)).Length;

        //ランダムな整数を取得
        int number = UnityEngine.Random.Range(0, maxCount);

        //int型からenum型へ変換
        var nextAttribute = (AttributeType)Enum.ToObject(typeof(AttributeType), number);
        ChangeAttribute(nextAttribute);

        if (stateController.CurrentState == stateController.idleState)
        {
            // Idle ステートから次に行うランダムなメソッドを決定
            int randomChoice = UnityEngine.Random.Range(0, 3);
            switch (randomChoice)
            {
                case 0:
                    AttackSword();
                    break;
                case 1:
                    AttackMagic();
                    break;
                case 2:
                    Laugh();
                    break;
            }
        }

        if (transform.position.x < playerPresenter.GetPosition().x)
        {
            _angle.Value = 180f; // プレイヤーがボスの右側にいる
        }
        else
        {
            _angle.Value = 0f; // プレイヤーがボスの左側にいる
        }
    }

    public async UniTask AttackSword()
    {
        // Step 1: Walkステートへ
        stateController.TransitionTo(stateController.walkState);

        // Step 2: プレイヤーに近づくまで待機
        if (playerTarget != null)
        {
            await UniTask.WaitUntil(() => Vector3.Distance(transform.position, playerTarget.position) <= attackDistance, cancellationToken: cts.Token);
        }

        // Step 3: Attackステートに切り替え
        stateController.TransitionTo(stateController.attackState);

        swordRotation = _angle.Value == 0f ? new Vector3(0, 180, -45) : new Vector3(0, 0, -45);

        // Step 4: Idleに戻す（次の行動判断のため）
        stateController.TransitionTo(stateController.idleState);
    }

    public async UniTask AttackMagic()
    {
        // Step 1: Walkステートへ
        stateController.TransitionTo(stateController.magicChargeState);

        // Step 2: チャージ時間分待機
        await UniTask.Delay(TimeSpan.FromSeconds(MaxMagicChargeTime), cancellationToken: cts.Token);

        var tmp = magicVelocity;
        if (transform.rotation.y == 0f)
        {
            magicVelocity *= -1f; // 左向きなら速度を反転
            magicFlip = false;
        }

        switch (_bossAttribute.Value)
        {
            case AttributeType.Red:
                magicColorCode = "#FF0061";
                break;
            case AttributeType.Green:
                magicColorCode = "#03B46B";
                break;
            case AttributeType.Blue:
                magicColorCode = "#6E4EF5";
                break;
        }

        // Step 3: MagicReleaseステートに切り替え
        stateController.TransitionTo(stateController.magicReleaseState);

        // Step 4: Idleに戻す（次の行動判断のため）
        stateController.TransitionTo(stateController.idleState);
        magicVelocity = tmp; // 元の速度に戻す
        magicFlip = true; // 魔法の向きをリセット

    }

    public async UniTask Laugh()
    {
        // Step 1: Walkステートへ
        stateController.TransitionTo(stateController.laughState);

        // Attackのアニメーションが終わるのを待つ（仮に2秒）
        await UniTask.Delay(TimeSpan.FromSeconds(MaxLaughTime), cancellationToken: cts.Token);

        // Step 4: Idleに戻す（次の行動判断のため）
        stateController.TransitionTo(stateController.idleState);
    }

    public void Die()
    {
        // Step 1: Dieステートへ
        stateController.TransitionTo(stateController.dieState);
        _isDead.Value = true; // 死亡フラグを立てる
        cts.Cancel();
    }

    public async void OnDamaged(CollisionInfo collisionInfo)
    {
        if (isDamageAnimating) return;
        if (stateController.CurrentState == stateController.dieState) return; // 既に死んでいる場合は無視
        Debug.Log("当たった");

        isDamageAnimating = true;
        var attack = collisionInfo.attackParam;
        if (attack == null)
        {
            Debug.LogWarning("Attack parameter is null in OnDamaged.");
            isDamageAnimating = false;
            return;
        }
        if (attack.team == Team.Enemy)
        {
            Debug.LogWarning("Attack from enemy team in OnDamaged.");
            isDamageAnimating = false;
            return; // 敵からの攻撃は無視
        }

        knockbackDirection = (transform.position.x - collisionInfo.hitTransform.position.x < 0f) ? -1f : 1f;

        // Step 1: Damageステートへ
        stateController.TransitionTo(stateController.damageState);

        TakeDamage(attack.damage, attack.attackerAttribute);
        if (_hp.Value <= 0)
        {
            Die();
            return; // HPが0以下なら即座に死亡処理
        }
        Debug.Log("BossHead OnDamaged called");

        await UniTask.Delay(TimeSpan.FromSeconds(damageInterval), cancellationToken: cts.Token);

        isDamageAnimating = false;

        // Step 4: Idleに戻す（次の行動判断のため）
        stateController.TransitionTo(stateController.idleState);
    }

    public void TakeDamage(float damage, AttributeType attackerAttribute)
    {
        float multiplier = CalculateDamage(attackerAttribute, this._bossAttribute.Value);
        int finalDamage = (int)(damage * multiplier);
        _hp.Value -= finalDamage;
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

    public void UpdatePosition(Vector2 position)
    {
        _position.Value = position;
    }

    public void UpdateRotation(float angle)
    {
        _angle.Value = angle;
    }
}
