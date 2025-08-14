using DG.Tweening;
using UnityEngine;

public class PlayerDodgeAttackState : PlayerAttackState
{
    public PlayerDodgeAttackState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        StateMachine.ConsumeStateWindow(PlayerStates.DodgeAttack);
        
        StateMachine.ClearBuffer();
        
        ToggleAnimState(PlayerConstant.DodgeAttackHash, true);

        UseWeapon.OnHit += Attack;
    }

    public override void Exit()
    {
        base.Exit();
        
        StateMachine.ActiveStateWindow(PlayerStates.ComboAttack);
        StateMachine.ActiveStateWindow(PlayerStates.DodgeAttack);

        UseWeapon.SetComboIndex(-1);

        ToggleAnimState(PlayerConstant.DodgeAttackHash, false);

        UseWeapon.OnHit -= Attack;
    }

    private void Attack()
    {
        float damageMult =  UseWeapon.GetDodgeAttackDamageMult();
        
        Player.WeaponHandler.Attack(damageMult, UseWeapon.GetDodgeAttackHitBoxSize(), AttackDir);
    }
}