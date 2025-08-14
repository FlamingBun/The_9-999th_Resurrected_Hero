using UnityEngine;

public class PlayerChargeAttackState : PlayerChargeState
{
    private Vector2 _attackDir;
    private BaseWeapon _equippedWeapon;
    
    public PlayerChargeAttackState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        _equippedWeapon = Player.WeaponHandler.UseWeapon;
        
        ToggleAnimState(PlayerConstant.ComboAttackHash, true);
        RegisterWeaponEvent();
    }

    public override void Exit()
    {
        base.Exit();
        
        ToggleAnimState(PlayerConstant.ComboAttackHash, false);
        UnregisterWeaponEvent();
    }
    
    private void RegisterWeaponEvent()
    {
        /*_equippedWeapon.OnStart += SetAttackState;
        _equippedWeapon.OnDash += DashToAttackDir;
        _equippedWeapon.OnHit += DrawComboHitBox;
        _equippedWeapon.OnFinish += OnFinishAttack;*/
    }

    private void UnregisterWeaponEvent()
    {
        /*_equippedWeapon.OnStart -= SetAttackState;
        _equippedWeapon.OnDash -= DashToAttackDir;
        _equippedWeapon.OnHit -= DrawComboHitBox;
        _equippedWeapon.OnFinish -= OnFinishAttack;*/
    }
    
    private void SetAttackState()
    {
        _attackDir = Player.LookDir;
                
        Player.VisualHandler.UpdateModelToLookDir();
        Player.WeaponHandler.HandleWeapon();
                
        Anim.speed = 1 + Player.StatHandler.GetStat(StatType.AttackSpeed).Value;
    }
    
    private void DashToAttackDir()
    {
        //Dash(_attackDir);
    }
    
    private void DrawComboHitBox()
    {
        //DrawHitBox(new Vector2(3.5f, 3.5f)); // 임시
    }

    private void OnFinishAttack()
    {
        StateMachine.ChangeIdleState();
        InputController.DisablePrimaryCharge();
    }

}