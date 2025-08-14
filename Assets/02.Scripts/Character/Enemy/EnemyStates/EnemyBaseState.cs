using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemyBaseState:CharacterBaseState
{
    protected EnemyStateMachine stateMachine;
    protected EnemyController controller;
    protected EnemyMoveHandler moveHandler;

    // TODO : 보스가 늘어나면 상위 클래스 추가
    protected GoblinBossController bossController;
    
    protected List<Coroutine> currentRoutines;

    protected int rigidbodyInstanceID;
    
    protected bool isBoss;

    private bool _isFlip;
    
    public EnemyBaseState(EnemyController enemyController, EnemyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        controller = enemyController;
        
        bossController = controller as GoblinBossController;
        
        moveHandler = controller.MoveHandler;
        
        currentRoutines = new ();
        rigidbodyInstanceID = controller.Rigid.GetInstanceID();
        
        isBoss = enemyController.Data.isBoss;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        foreach (Coroutine routine in currentRoutines)
        {
            if (routine != null) controller.StopCoroutine(routine);
        }

        currentRoutines.Clear();
        
        if (DOTween.IsTweening(rigidbodyInstanceID))
        {
            DOTween.Kill(rigidbodyInstanceID);
        }
    }

    public override void Update()
    {
        base.Update();
        FlipSprite();
    }

    protected void FlipSprite()
    {
        float lookDirX = controller.LookDir.x;

        if (lookDirX == 0) return;

        _isFlip = lookDirX < 0;

        controller.SpriteRenderer.flipX = _isFlip;
        
        if (isBoss)
        {
            bossController.animationEffect.flipX = _isFlip;
        }
    }

    protected void ResetAnimationBool()
    {
        controller.Anim.SetBool(EnemyAnimationHashes.Idle, false);
        controller.Anim.SetBool(EnemyAnimationHashes.Move, false);
        controller.Anim.SetBool(EnemyAnimationHashes.Attack,false);
        controller.Anim.SetBool(EnemyAnimationHashes.Hit,false);
        controller.Anim.SetBool(EnemyAnimationHashes.Dead,false);
    }
    
    protected void StartAndTrackCoroutine(IEnumerator routine)
    {
        Coroutine r = controller.StartCoroutine(routine);
        currentRoutines.Add(r);
    }
}
