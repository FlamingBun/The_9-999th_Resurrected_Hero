using DG.Tweening;
using UnityEngine;
public class EnemyDeadState: EnemyBaseState
{
    private AnimatorStateInfo _stateInfo;
    public EnemyDeadState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine) { }
    
    public override void Enter()
    {
        base.Enter();
        controller.Anim.SetBool(EnemyAnimationHashes.Dead, true);

        DestroyEnemy();
    }

    public override void Update()
    {
    }
    
    public override void Exit()
    {
    }
    
    public void DestroyEnemy()
    {
        if (controller.HealthBar != null)
        {
            controller.HealthBar.gameObject.SetActive(false);
        }

        DOTween.Kill(controller.Rigid.GetInstanceID());
        DOTween.Kill(controller.transform);
        
        moveHandler.AILerp.canMove = false;
        moveHandler.StopMove();
        moveHandler.enabled = false;

        controller.SetStateMachineNull();
        
        controller.Collider.enabled = false;
        
        controller.SpriteRenderer.material.SetFloat(EnemyMaterialKey.FadeAmountKey, -0.01f);
        controller.SpriteRenderer.material.SetFloat(EnemyMaterialKey.GreyScaleBlendKey, 0.8f);
        
        Sequence seq = DOTween.Sequence();
        
        controller.Anim.speed = 0f;
            
        Vector3 movePosition;

        if (controller.Player == null) return;
        
        if (controller.Player.transform.position.x < controller.transform.position.x)
        {
            movePosition = Vector3.right * EnemyConstant.deathMoveHorizontalPower;
        }
        else
        {
            movePosition = Vector3.left * EnemyConstant.deathMoveHorizontalPower;
        }
            
        movePosition += Vector3.up * EnemyConstant.deathMoveVerticalPower;
            
        seq.Append(controller.transform.DOJump(controller.transform.position + movePosition , EnemyConstant.deathJumpPower, EnemyConstant.deathJumpCount, EnemyConstant.deathJumpDuration, false)).SetEase(Ease.OutQuad);
        seq.Join(DOTween.Sequence().AppendInterval(EnemyConstant.deathJumpInterval).AppendCallback(() => controller.Anim.speed = 1f));

        seq.AppendInterval(EnemyConstant.deathJumpInterval).Append(controller.SpriteRenderer.material.DOFloat(1f, EnemyMaterialKey.FadeAmountKey, EnemyConstant.deathFadeTime).OnStart(()=> 
        { 
            controller.shadow.SetActive(false);
            controller.minimapIcon.SetActive(false);
        })).OnComplete(() =>
        {
            if (!controller) return;
            DOTween.Kill(controller.transform);
            Object.Destroy(controller.gameObject);
        });
    }
}
