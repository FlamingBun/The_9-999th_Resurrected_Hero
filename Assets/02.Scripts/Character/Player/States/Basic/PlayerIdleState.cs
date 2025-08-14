using DG.Tweening;
using UnityEngine;

public class PlayerIdleState : PlayerBasicState
{
    public PlayerIdleState(PlayerStateMachine playerStateMachine) : base(playerStateMachine){}

    public override void Enter()
    {
        base.Enter();
        
        Rigid.DOKill();

        InputController.OnDodge += StateMachine.ChangeDodgeState;
    }

    public override void Exit()
    {
        base.Exit();

        InputController.OnDodge -= StateMachine.ChangeDodgeState;
    }

    protected override void OnPrimaryCancel(float holdTime)
    {
        if (holdTime < 0.2f)
        {
            StateMachine.ChangeComboAttackState();
        }
    }

    public override void Update()
    {
        base.Update();
        
        if (Player.MoveDir != Vector2.zero)
        {
            StateMachine.ChangeWalkState();
            return;
        }
        
        Player.VisualHandler.UpdateModelToLookDir();
        Player.WeaponHandler.HandleWeapon();
    }
 
}
