using UnityEngine;

public abstract class PlayerChargeState : PlayerBaseState
{
    
    public PlayerChargeState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        
        ToggleAnimState(PlayerConstant.ChargeHash, true);
        
        InputController.OnPrimaryCancel +=  OnChargeFinish;
    }
    
    public override void Exit()
    {
        base.Exit();
        
        ToggleAnimState(PlayerConstant.ChargeHash, false);

        
        InputController.OnPrimaryCancel -=  OnChargeFinish;
    }


    protected void UpdateChargeGage()
    {
        StateMachine.ChargeTime += Time.deltaTime;
    }
    
    protected void ExitToBasicDodge()
    {
        StateMachine.ChangeDodgeState();
        InputController.DisablePrimaryCharge();
    }

    void OnChargeFinish(float holdTime)
    {
        if ( StateMachine.ChargeTime > 1)
        {
            StateMachine.ChargeTime = 0;
            StateMachine.ChangeChargeAttackState();
            return;
        }
        
        StateMachine.ChangeIdleState();
        InputController.DisablePrimaryCharge();
    }
    
   

}