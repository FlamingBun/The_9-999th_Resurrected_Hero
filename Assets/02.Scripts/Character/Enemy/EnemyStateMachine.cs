using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public abstract class EnemyStateMachine:CharacterStateMachine
{
    public EnemyController EnemyController { get; private set; }
    public EnemyStates CurrentState { get; private set; }

    public EnemyStates NextAttackState { get => nextAttackState; }

    public AILerp aiPath;
    
    
    public bool isFollowTarget;
    public bool isAttack;

    protected Dictionary<EnemyStates, EnemyBaseState> statesDict = new();
    protected EnemyStates nextAttackState;
    
    protected float attackStartRange;
    
    private float _evadeRange;
    
    private PlayerController _player;
    

    public EnemyStateMachine(EnemyController enemyController) : base(enemyController)
    {
        EnemyController = enemyController;

        _player = enemyController.Player;
        aiPath = enemyController.MoveHandler.AILerp;
        
        _evadeRange = enemyController.StatHandler.GetStat(StatType.EvadeRange).Value;
        
        enemyController.MoveHandler.OnFollowTargetChange += OnChangeTarget;
    }
    

    public void ChangeEnemyState(EnemyStates states)
    {
        if (CurrentState == EnemyStates.Dead)
        {
            return;
        }

        if(statesDict.TryGetValue(states, out EnemyBaseState targetState))
        {
            CurrentState = states;
            ChangeState(targetState);
        }
        else
        {
            Logger.Log($"{states}가 존재하지 않습니다.");
        }
    }

    public void OnChangeTarget(bool isFollowTarget)
    {
        this.isFollowTarget = isFollowTarget;
    }
    
    public bool CheckArriveDestination()
    {
        if (isFollowTarget && aiPath.remainingDistance <= attackStartRange)
        {
            return true;
        }

        return aiPath.reachedDestination;
    }
    
    public bool CheckTargetInAttackRange()
    {
        RaycastHit2D hit = Physics2D.Raycast(EnemyController.transform.position, (_player.transform.position - EnemyController.transform.position).normalized,Mathf.Min(Vector3.Distance(_player.transform.position, EnemyController.transform.position),attackStartRange), LayerMask.GetMask("Obstacle"));
        if (!hit&&Vector3.Distance(_player.transform.position, EnemyController.transform.position) <= attackStartRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckIsNearTarget()
    {
        if (Vector3.Distance(EnemyController.transform.position, _player.transform.position) <= _evadeRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public abstract void ChangeAttackState();
}
