using System;
using System.Collections.Generic;
using UnityEngine;

public class GoblinBossController : EnemyController
{
    [HideInInspector]public Bounds roomBounds;

    [HideInInspector]public GameObject parentOfStonesAndIndicators;
    
    [HideInInspector]public JumpAttackCollider jumpAttackDamageCollider;
    
    public SpriteRenderer animationEffect;
    
    [Space(10)]
    [Header("Fire")]

    public Transform firePointsAnchor;
    
    public List<Transform> firePoints;

    [Space(10)]
    [Header("Smoke")]
    
    [SerializeField] private Transform _smokePosition;
    
    [SerializeField] private SmokeVFX _jumpStartSmoke;
    [SerializeField] private SmokeVFX _jumpEndSmoke;

    [Space(5)]
    [SerializeField] private SmokeVFX _stompSmoke;
    [SerializeField] private Vector3 _stompSmokeOffset;
    
    
    private BossHealthBar _bossHealthbar;
    
    public event Action OnMoveStart;
    public event Action OnMoveEnd;
    public event Action OnHitFloor;

    protected override void Awake()
    {
        base.Awake();

        jumpAttackDamageCollider = GetComponentInChildren<JumpAttackCollider>();
        
        parentOfStonesAndIndicators = new GameObject("StoneAndIndicators");
        
        OnDeath += DestroyStonesAndIndicators;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_jumpStartSmoke != null)
        {
            Destroy(_jumpStartSmoke);
        }

        if (_jumpEndSmoke != null)
        {
            Destroy(_jumpEndSmoke);
        }

        OnDeath -= DestroyStonesAndIndicators;
    }

    public override void Init(EnemyDataSO enemyDataSO, FloorManager floorManager)
    {
        base.Init(enemyDataSO, floorManager);
        
        parentOfStonesAndIndicators.transform.SetParent(floorManager.transform);

        _bossHealthbar = UIManager.Instance.GetUI<TowerHUDUI>().bossHealthBar;
        
        _bossHealthbar.gameObject.SetActive(true);

        _bossHealthbar.SetHealthBarValue(StatusHandler.GetStatus(StatType.Health).CurValue, StatusHandler.GetStatus(StatType.Health).MaxValue);
        
        OnDeath += () => { _bossHealthbar.gameObject.SetActive(false); };
    }
    
    protected override void SetStateMachine()
    {        
        StateMachine = new GoblinBossStateMachine(this);
        enemyStateMachine = StateMachine as GoblinBossStateMachine;
        enemyStateMachine.ChangeEnemyState(EnemyStates.Trace);
    }
    
    public virtual void AnimationEvent(string key)
    {
        switch (key)
        {
            case EnemyAnimationEventKey.AttackStartKey :
                superArmorHandler.TrySuperArmorOnAttack();
                
                CallAttackStartEvent();
                break;
            
            case EnemyAnimationEventKey.AttackHitKey :
                CallAttackHitEvent();
                break;
            
            case EnemyAnimationEventKey.AttackEndKey :
                StartCoroutine(AnimationEndCheckRoutine(CallAttackEndEvent));
                break;
            
            case EnemyAnimationEventKey.HitFloorKey :
                CallHitFloorEvent();
                break;
            
            case EnemyAnimationEventKey.MoveStartKey :
                CallMoveStartEvent();
                break;
            
            case EnemyAnimationEventKey.MoveEndKey :
                StartCoroutine(AnimationEndCheckRoutine(CallMoveEndEvent));
                break;
            
            case EnemyAnimationEventKey.HitEndKey :
                StartCoroutine(AnimationEndCheckRoutine(CallHitEndEvent));
                break;
        }
    }

    public Stone InstantiateStone(Stone stone)
    {
        return Instantiate(stone, parentOfStonesAndIndicators.transform);
    }

    public SpriteRenderer InstantiateIndicator(SpriteRenderer indicator)
    {
        return Instantiate(indicator, parentOfStonesAndIndicators.transform);
    }

    public void SpawnJumpStartSmoke()
    {
        _jumpStartSmoke.transform.position = _smokePosition.position;
        _jumpStartSmoke.Spawn();
    }

    public void SpawnJumpEndSmoke()
    {
        _jumpEndSmoke.transform.position = _smokePosition.position;
        _jumpEndSmoke.Spawn();
    }

    public void SpawnStompSmoke()
    {
        if (attackEffectPosition.localPosition.x < 0f)
        {
            _stompSmoke.transform.position = _smokePosition.position - _stompSmokeOffset;
        }
        else
        {
            _stompSmoke.transform.position = _smokePosition.position + _stompSmokeOffset;
        }

        _stompSmoke.Spawn();
    }

    protected override void UpdateHealthBar(StatusEventData eventData)
    {
        if (eventData.StatType == StatType.Health)
        {
            _bossHealthbar.SetHealthBarValue(eventData.CurValue, eventData.MaxValue);
        }
    }
    
    private void DestroyStonesAndIndicators()
    {
        Destroy(parentOfStonesAndIndicators);
    }

    private void CallMoveStartEvent()
    {
        OnMoveStart?.Invoke();
    }

    private void CallMoveEndEvent()
    {
        OnMoveEnd?.Invoke();
    }

    private void CallHitFloorEvent()
    {
        OnHitFloor?.Invoke();
    }

    //
    // public JumpAttackData jumpAttackData;
    // public Vector3 offset;
    // private void OnDrawGizmos()
    // {
    //     Vector3 boxCenter = transform.position + offset;
    //
    //     Gizmos.color = Color.green;
    //
    //     Gizmos.DrawWireCube(boxCenter, jumpAttackData.areaRange);
    // }
}
