using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pathfinding;
using UnityEngine.Events;

#region require Components
[RequireComponent(typeof(EnemyMoveHandler))]
[RequireComponent(typeof(EnemySuperArmorHandler))]
#endregion require Components
public abstract class EnemyController : CharacterBaseController
{
    public EnemyDataSO Data { get => _data; }
    public EnemyStateMachine EnemyStateMachine { get => enemyStateMachine; }


    public EnemyMoveHandler MoveHandler { get => _moveHandler; }
    public EnemySuperArmorHandler SuperArmorHandler { get => superArmorHandler; }


    public EnemyHealthBar HealthBar { get => healthBar; }


    public PlayerController Player { get => _player; }

    public SpriteRenderer SpriteRenderer { get => _spriteRenderer; }
    public CircleCollider2D Collider { get; private set; }


    public bool IsBoss { get => _isBoss; }
    public bool IsEvadable { get => _isEvadable; }

    public GameObject shadow;

    public GameObject minimapIcon;

    public Transform attackEffectPosition;

    public InitializeImpact initializeImpact;
    
    public event Action OnAttackStart;
    public event Action OnAttackHit;
    public event Action OnAttackEnd;
    public event Action OnHitEnd;


    protected EnemyStateMachine enemyStateMachine;
    protected EnemyHealthBar healthBar;
    protected EnemySuperArmorHandler superArmorHandler;


    private EnemyDataSO _data;
    private EnemyMoveHandler _moveHandler;
    private SpriteRenderer _spriteRenderer;
    private PlayerController _player;
    
    private bool _isEvadable;
    private bool _isBoss;


    protected override void Awake()
    {
        base.Awake();

        CanDamageable = true;

        Collider = GetComponent<CircleCollider2D>();
        superArmorHandler = GetComponent<EnemySuperArmorHandler>();

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _moveHandler = GetComponent<EnemyMoveHandler>();

        healthBar = GetComponentInChildren<EnemyHealthBar>();

        isEnemy = true;
    }

    protected override void Update()
    {
        if (StateMachine == null || _player == null) return;

        base.Update();
        LookDir = _moveHandler.GetDirection();
    }


    public virtual void Init(EnemyDataSO enemyDataSO, FloorManager floorManager)
    {
        UIManager.Instance.GetUI<ScreenIndicatorUI>().AddTarget(transform);

        _data = enemyDataSO;

        ReadyForInitialize();
        
        List<StatData> statDatas = CreateModifiedStats(floorManager);

        StatHandler.Init(statDatas);
        StatusHandler.Init(StatHandler);

        _isBoss = _data.isBoss;
        _isEvadable = StatHandler.GetStat(StatType.EvadeRange).Value != 0f;

        _player = floorManager.Player;

        superArmorHandler.Init(this);
        _moveHandler.Init(this, _player.transform);

        StatusHandler.OnStatusChanged += UpdateHealthBar;
        StatusHandler.OnStatusChanged += PlayHitSFX;
        StatusHandler.OnStatusChanged += SpawnHitEffect;

        OnDeath += ChangeDeadState;
        OnDeath += SpawnDropObject;

        ObjectPoolManager.Instance.CreatePool(_data.hitEffect);

        StartInitializeEffect();
    }

    public override void TakeDamage(DamageInfo damageInfo)
    {
        base.TakeDamage(damageInfo);

        PlayHitEffect(damageInfo);
    }

    protected virtual void PlayHitEffect(DamageInfo damageInfo)
    {
        ParticleHandler hitEffect = ObjectPoolManager.Instance.Spawn<ParticleHandler>(_data.hitEffect.name);
        
        hitEffect.transform.position = damageInfo.HitPoint;

        float angle = Mathf.Atan2(damageInfo.HitDirection.y, damageInfo.HitDirection.x) * Mathf.Rad2Deg;

        hitEffect.transform.localRotation = Quaternion.Euler(0, 0, angle);

        hitEffect.Init(_data.hitEffect.name);

    }


    protected override void OnDestroy()
    {
        base.OnDestroy();

        StatusHandler.OnStatusChanged -= UpdateHealthBar;
        StatusHandler.OnStatusChanged -= PlayHitSFX;
        StatusHandler.OnStatusChanged -= SpawnHitEffect;

        OnDeath -= ChangeDeadState;
        OnDeath -= SpawnDropObject;

    }

    public override void MoveBack(CharacterBaseController otherCharacter, UnityAction onComplete = null)
    {
        if (StatHandler.GetStat(StatType.CollisionMovePower).Value == 0f) return;

        AILerp aipath = MoveHandler.AILerp;

        if (aipath.canMove == false)
        {
            // 다른 이동을 처리중일 경우 ex) 몬스터 대쉬 공격
            return;
        }

        aipath.canMove = false;

        base.MoveBack(otherCharacter, () => aipath.canMove = true);
    }

    public void SetStateMachineNull()
    {
        StateMachine = null;
        enemyStateMachine = null;
    }

    public virtual void AnimationEvent(string key)
    {
        switch (key)
        {
            case EnemyAnimationEventKey.AttackStartKey:
                StartCoroutine(AnimationEndCheckRoutine(CallAttackEndEvent));

                superArmorHandler.TrySuperArmorOnAttack();

                CallAttackStartEvent();
                break;

            case EnemyAnimationEventKey.AttackHitKey:
                CallAttackHitEvent();
                break;

            case EnemyAnimationEventKey.HitEndKey:
                StartCoroutine(AnimationEndCheckRoutine(CallHitEndEvent));
                break;
        }
    }


    protected abstract void SetStateMachine();



    protected IEnumerator AnimationEndCheckRoutine(Action callback)
    {
        AnimatorStateInfo _stateInfo = Anim.GetCurrentAnimatorStateInfo(0);

        while (_stateInfo.normalizedTime < 1.0f)
        {
            _stateInfo = Anim.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        callback?.Invoke();
    }


    protected void CallAttackStartEvent()
    {
        OnAttackStart?.Invoke();
    }

    protected void CallAttackHitEvent()
    {
        OnAttackHit?.Invoke();
    }

    protected void CallAttackEndEvent()
    {
        OnAttackEnd?.Invoke();
    }

    protected void CallHitEndEvent()
    {
        OnHitEnd?.Invoke();
    }

    private void ReadyForInitialize()
    {
        initializeImpact.gameObject.SetActive(false);
        
        CanDamageable = false;

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        shadow.SetActive(false);
        
        Material material = SpriteRenderer.material;

        Color color = SpriteRenderer.color;
        color.a = 0f;
        SpriteRenderer.color = color;
        
        material.SetFloat(EnemyMaterialKey.PixelateSizeKey, EnemyConstant.pixelateStartValue);
    }

    private void StartInitializeEffect()
    {
        Material material = SpriteRenderer.material;

        Sequence sequence = DOTween.Sequence();

        bool isSpawnInitilizeImpact = false;
        
        sequence.Append(material.DOFloat(EnemyConstant.pixelateEndValue, EnemyMaterialKey.PixelateSizeKey, EnemyConstant.initializeEffectTime)).SetEase(Ease.InCubic).OnUpdate(() => {
            if (material.GetFloat(EnemyMaterialKey.PixelateSizeKey) >= (EnemyConstant.pixelateEndValue * 0.8f))
            {
                if (isSpawnInitilizeImpact) return;
                
                SpawnInitializeImpact();
                isSpawnInitilizeImpact = true;
            }
        });
        sequence.Join(SpriteRenderer.DOFade(1f, EnemyConstant.initializeEffectTime)).SetEase(Ease.InCubic).OnComplete(() =>
        {
            material.SetFloat(EnemyMaterialKey.PixelateSizeKey, EnemyConstant.pixelateDefaultValue);
            EndInitializeEffect();
        });
    }
    
    private void SpawnInitializeImpact()
    {
        initializeImpact.gameObject.SetActive(true);
        initializeImpact.Init();
    }

    protected virtual void EndInitializeEffect()
    {
        CanDamageable = true;

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
        }

        shadow.SetActive(true);

        SetStateMachine();
    }

    private List<StatData> CreateModifiedStats(FloorManager floorManager)
    {
        List<StatData> statDatas = new();

        foreach (var statData in _data.statDatas)
        {
            StatData newStat = new()
            {
                statType = statData.statType,
                value = statData.value,
            };

            statDatas.Add(newStat);
        }

        foreach (var stat in statDatas)
        {
            switch (stat.statType)
            {
                case StatType.AttackDamage:
                    stat.value *= floorManager.GetFeatureMultiplier(TowerCurseType.EnemyAttackDamage);
                    break;

                case StatType.Health:
                    stat.value *= floorManager.GetFeatureMultiplier(TowerCurseType.EnemyHealth);
                    break;

                case StatType.MoveSpeed:
                    stat.value *= floorManager.GetFeatureMultiplier(TowerCurseType.EnemyMoveSpeed);
                    break;
            }
        }

        return statDatas;
    }

    public float GetFinalDamage(float attackMultiplier)
    {
        return StatHandler.GetStat(StatType.AttackDamage).Value * attackMultiplier;
    }

    private void SpawnHitEffect(StatusEventData eventData)
    {
        if (eventData.StatType == StatType.Health)
        {
            if (eventData.CurValue < eventData.PreValue)
            {
                Vector2 directionFromPlayer = transform.position - _player.transform.position;
                // TODO : 피 터지는 효과 추가
            }
        }
    }

    private void PlayHitSFX(StatusEventData eventData)
    {
        if (eventData.StatType == StatType.Health)
        {
            if (eventData.CurValue < eventData.PreValue)
            {
                AudioManager.Instance.Play("MonsterHitClip");
            }
        }
    }

    protected virtual void UpdateHealthBar(StatusEventData eventData)
    {
        if (!Data.isBoss && eventData.StatType == StatType.Health)
        {
            healthBar.SetHealthBarValue(eventData.CurValue / eventData.MaxValue);
        }
    }

    private void ChangeDeadState()
    {
        UIManager.Instance.GetUI<ScreenIndicatorUI>().RemoveTarget(transform);
        enemyStateMachine.ChangeEnemyState(EnemyStates.Dead);
    }


    private void SpawnDropObject()
    {
        if (TryGetComponent(out TowerDropObjectHandler objectHandler))
        {
            objectHandler.SpawnGold();
            objectHandler.SpawnSoul();
        }
    }
}
