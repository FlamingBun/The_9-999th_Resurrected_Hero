using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Random = UnityEngine.Random;

public class GoblinBossAttackStompAttackState:EnemyAttackState
{
    private EnemyAreaAttackDataSO[] _areaAttackDatas;

    private List<SpriteRenderer> _indicators;
    
    private List<Stone> _stones;
    
    private int _maxAttackCount;
    private int _currentAttackIndex;
    
    private List<Vector2> _defaultStoneSpawnPositions;

    private List<Vector2> _currentStoneSpawnPositions;

    
    public GoblinBossAttackStompAttackState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine)
    {
        attackDataList = controller.Data.attackPatternList[2].attackDatas;
        
        _maxAttackCount = attackDataList.Count;

        _areaAttackDatas = new EnemyAreaAttackDataSO[_maxAttackCount];
        
        
        int maxStoneCount = 0;
        
        
        for (int i = 0; i < _maxAttackCount; i++)
        {
            _areaAttackDatas[i] = attackDataList[i] as EnemyAreaAttackDataSO;
            
            if (_areaAttackDatas[i].stompAttackData.stoneCount > maxStoneCount)
            {
                maxStoneCount = _areaAttackDatas[i].stompAttackData.stoneCount;
            }
        }
        
        InitStonesAndIndicator(maxStoneCount);

        
        ObjectPoolManager.Instance.CreatePool(_areaAttackDatas[0].impact.GetComponent<VFXHandler>());
        
        waitForAfterAttackDelay = new WaitForSeconds(_areaAttackDatas[0].afterAttackDelay);
    }
    
    public override void Enter()
    {
        base.Enter();

        if (_currentAttackIndex == _maxAttackCount)
        {
            _currentAttackIndex = 0;
        }
        
        float randomAttackRateAdjustment = Random.Range(-_areaAttackDatas[_currentAttackIndex].attackRateAdjustment, _areaAttackDatas[_currentAttackIndex].attackRateAdjustment);
        
        StartAndTrackCoroutine(ReadyForAttack(_areaAttackDatas[_currentAttackIndex].attackDelay, _areaAttackDatas[_currentAttackIndex].attackRate + randomAttackRateAdjustment,  Attack));
    }

    public override void Update()
    {
    }
    
    public override void Exit()
    {
        base.Exit();
        
        controller.Anim.SetBool(EnemyAnimationHashes.StompAttack, false);
        
        if (_currentAttackIndex >= _maxAttackCount)
        {
            controller.SuperArmorHandler.RemoveSuperArmor();   
        }
    }
    
    protected override void Attack()
    {
        isAttack = true;
        stateMachine.isAttack = true;
        
        controller.Anim.speed = attackRate;
        
        GenerateStonePositions(bossController.roomBounds, _areaAttackDatas[_currentAttackIndex].stompAttackData);

        controller.Anim.SetBool(EnemyAnimationHashes.StompAttack, true);
    }

    protected override void OnStart()
    {
        if (_currentAttackIndex == 0)
        {
            SetIndicatorsActive(true);
            IndicatorFadeInRoutine(attackDelay);
            
            StartAndTrackCoroutine(GlowOnAttack(attackDelay));    
        }
        else
        {
            SetIndicatorsActive(true);
            IndicatorFadeInRoutine(attackDelay);
        }
    }

   
    protected override void OnHit()
    {
        FlipSprite();
        bossController.SpawnStompSmoke();
        SetIndicatorsActive(false);
        Stomp();
    }

    protected override void OnEnd()
    {
        StartAndTrackCoroutine(EndAttackRoutine());
    }
    


    protected override IEnumerator EndAttackRoutine()
    {
        while (isAttack)
        {
            yield return null;
        }
        
        yield return waitForAfterAttackDelay;
        
        ChangeState();
    }
    

    private void ChangeState()
    {
        _currentAttackIndex++;
        stateMachine.isAttack = false;
        
        if (_currentAttackIndex < _maxAttackCount)
        {
            stateMachine.ChangeEnemyState(EnemyStates.ThirdAttack);
            return;
        }
        
        if (!stateMachine.CheckTargetInAttackRange())
        {
            stateMachine.ChangeEnemyState(EnemyStates.Trace);
        }
        else
        {
            stateMachine.ChangeAttackState();
        }
    }

    private void SetIndicatorsActive(bool isActive)
    {
        int index = 0;

        foreach (Vector2 SpawnPosition in _currentStoneSpawnPositions)
        {
            _indicators[index].gameObject.SetActive(isActive);
            _indicators[index++].transform.position = SpawnPosition;
        }
    }

    private void Stomp()
    {
        int index = 0;
        
        foreach (Vector2 spawnPosition in _currentStoneSpawnPositions)
        {
            _stones[index++].Spawn(spawnPosition);
        }

        // 사운드 재생 추가
        AudioManager.Instance.Play("BossStompAttackClip");

        isAttack = false;
    }

    private void InitStonesAndIndicator(int maxStoneCount)
    {
        _stones =  new ();
        _indicators = new();
        
        for (int i = 0; i < maxStoneCount; i++)
        {
            Stone newStone = bossController.InstantiateStone(_areaAttackDatas[0].stompAttackData.stone);
            newStone.Init(_areaAttackDatas[0], controller);
            _stones.Add(newStone);
            
            SpriteRenderer newIndicator = bossController.InstantiateIndicator(_areaAttackDatas[0].indicator);
            newIndicator.gameObject.SetActive(false);
            _indicators.Add(newIndicator);
        }
    }

    private void IndicatorFadeInRoutine(float duration)
    {
        foreach (SpriteRenderer indicator in _indicators)
        {
            Color color = indicator.color;
            color.a = 0f;
            indicator.color = color;
            
            indicator.DOFade(1f, duration).SetEase(Ease.OutQuart);
        }
    }


    private void GenerateStonePositions(Bounds bounds, StompAttackData stompAttackData, int maxAttempts = 100)
    {
        List<Vector2> positions = new List<Vector2>();

        int count = stompAttackData.stoneCount;
        Vector2 stoneSize = stompAttackData.stoneSize;
        float minOffset = stompAttackData.minOffset;

        bool hasXOffset = stoneSize.x % 2f == 1; // X 크기가 홀수면 0.5 위치 조정
        
        for (int i = 0; i < count; i++)
        {
            bool found = false;
            int attempts = 0;

            while (!found && attempts < maxAttempts)
            {
                attempts++;

                Vector2 candidatePosition = new Vector2((int)Random.Range(bounds.min.x, bounds.max.x), (int)Random.Range(bounds.min.y, bounds.max.y));

                if (hasXOffset)
                {
                    candidatePosition.x -= 0.5f;
                }

                
                bool isOverlap = false;

                Collider2D[] results = new Collider2D[1];

                int hitCount = Physics2D.OverlapBoxNonAlloc(candidatePosition, stoneSize, 0f, results, LayerMask.GetMask("Obstacle"));
                
                if (hitCount > 0)
                {
                    continue;
                }

                foreach (Vector2 selectedPosition in positions)
                {
                    if (Vector2.Distance(candidatePosition, selectedPosition) < minOffset)
                    {
                        isOverlap = true;
                        break;
                    }
                }

                if (!isOverlap)
                {
                    positions.Add(candidatePosition);
                    found = true;
                } 
            }

            if (!found)
            {
                if (_defaultStoneSpawnPositions != null)
                {
                    _currentStoneSpawnPositions = _defaultStoneSpawnPositions;
                    return;
                }
            }
        }
        
        if (_defaultStoneSpawnPositions == null)
        {
            _defaultStoneSpawnPositions = positions;
        }

        _currentStoneSpawnPositions = positions;
    }

    
}
