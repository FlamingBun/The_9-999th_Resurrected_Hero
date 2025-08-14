using UnityEngine;

public class PlayerChargeIdleState : PlayerChargeState
{
    
    public PlayerChargeIdleState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        InputController.OnDodge +=  ExitToBasicDodge;
    }

    public override void Exit()
    {
        base.Exit();
        InputController.OnDodge -=  ExitToBasicDodge;
    }

    public override void Update()
    {
        base.Update();

        UpdateChargeGage();
        
        Player.VisualHandler.UpdateModelToLookDir();
        Player.WeaponHandler.HandleWeapon();
        
        if (Player.MoveDir != Vector2.zero)
        {
            StateMachine.ChangeChargeMoveState();
            return;
        }
    }
}