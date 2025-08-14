using DG.Tweening;
using UnityEngine;

public abstract class PlayerMoveState : PlayerBasicState
{
    public PlayerMoveState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) {}

    public override void Enter()
    {
        base.Enter();
     
        Rigid.DOKill();
        
        Player.VisualHandler.EnableMoveVFX();
        
        InputController.OnDodge += StateMachine.ChangeDodgeState;
        
        ToggleAnimState(PlayerConstant.MoveHash, true);
    }

    public override void Exit()
    {
        base.Exit();
        
        Player.VisualHandler.DisableMoveVFX();
        
        InputController.OnDodge -= StateMachine.ChangeDodgeState;
        
        ToggleAnimState(PlayerConstant.MoveHash, false);
    }
 

    public override void Update()
    {
        base.Update();
        
        if (Player.MoveDir == Vector2.zero)
        {
            StateMachine.ChangeIdleState();
            return;
        }
        
        Player.VisualHandler.UpdateModelToLookDir();
        Player.WeaponHandler.HandleWeapon();
    }

}
