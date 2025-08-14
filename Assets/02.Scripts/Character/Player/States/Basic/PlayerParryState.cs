using UnityEngine;

public class PlayerParryState : PlayerBasicState
{
    private Vector2 _parryDir;
    
    public PlayerParryState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        /*_parryDir = StateMachine.BufferType == PlayerBufferType.Attack ?
            StateMachine.ConsumeBuffer() :
            Player.LookDir;*/
        
        Player.ParryTriggerHandler.Enable(_parryDir);
        
        Player.UpdateLookDir(_parryDir);

        Player.VisualHandler.UpdateModelToLookDir();
        Player.WeaponHandler.HandleWeapon();

        Player.WeaponHandler.UseWeapon.OnFinishAttack +=  Player.ParryTriggerHandler.Disable;
        Player.WeaponHandler.UseWeapon.OnFinish += StateMachine.ChangeIdleState;
        
        ToggleAnimState(PlayerConstant.ParryHash, true);
    }

    public override void Exit()
    {
        base.Exit();
        
        Player.WeaponHandler.UseWeapon.OnFinishAttack -=  Player.ParryTriggerHandler.Disable;
        Player.WeaponHandler.UseWeapon.OnFinish -= StateMachine.ChangeIdleState;
        
        ToggleAnimState(PlayerConstant.ParryHash, false);
    }
 
    
    protected override void OnPrimaryCancel(float holdTime) { }
 
}