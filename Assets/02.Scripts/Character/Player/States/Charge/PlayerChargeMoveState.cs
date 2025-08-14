using UnityEngine;

public class PlayerChargeMoveState : PlayerChargeState
{
    public PlayerChargeMoveState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        
        InputController.OnDodge +=  ExitToBasicDodge;
        
        ToggleAnimState(PlayerConstant.MoveHash, true);
    }

    public override void Exit()
    {
        base.Exit();
        
        InputController.OnDodge -=  ExitToBasicDodge;
        
        ToggleAnimState(PlayerConstant.MoveHash, false);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        //Move(0.66f);
    }

    
    public override void Update()
    {
        base.Update();

        UpdateChargeGage();
        
        Player.VisualHandler.UpdateModelToLookDir();
        Player.WeaponHandler.HandleWeapon();
        
        if (Player.MoveDir == Vector2.zero)
        {
            StateMachine.ChangeChargeIdleState();
            return;
        }
    }
}