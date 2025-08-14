using DG.Tweening;
using UnityEngine;

public class PlayerComboAttackState : PlayerAttackState
{
    public PlayerComboAttackState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

    public override void Enter()
    {
        base.Enter();

        if (!StateMachine.HasStateWindow(PlayerStates.ComboAttack))
        {
            UseWeapon.SetComboIndex(0);
        }
        
        if (StateMachine.HasStateWindow(PlayerStates.ComboAttack) || UseWeapon.CurComboIndex < 0)
        {
            UseWeapon.AddCombo();
        }
        
        
        StateMachine.ConsumeStateWindow(PlayerStates.ComboAttack);
        StateMachine.ClearBuffer();
        
        
        ToggleAnimState(PlayerConstant.ComboAttackHash, true);
        
        
        UseWeapon.Anim.SetInteger(PlayerConstant.BasicAttackComboCountHash, UseWeapon.CurComboIndex);
        Anim.SetInteger(PlayerConstant.BasicAttackComboCountHash, UseWeapon.CurComboIndex);
        
        UseWeapon.OnStartAttack += ComboAttackDash;
        UseWeapon.OnHit += Attack;
    }

    public override void Exit()
    {
        base.Exit();
        
        if(UseWeapon.CurComboIndex == UseWeapon.MaxComboCount - 1)
        {
            StateMachine.ClearWindow();
        }
        else
        {
            StateMachine.ActiveStateWindow(PlayerStates.ComboAttack);
        }

        ToggleAnimState(PlayerConstant.ComboAttackHash, false);

        UseWeapon.OnStartAttack -= ComboAttackDash;
        UseWeapon.OnHit -= Attack;
    }


    private void ComboAttackDash()
    {
        Player.AttackDash(AttackDir, UseWeapon.GetComboAttackDashDist());
    }

    private void Attack()
    {
        float damageMult =  UseWeapon.GetComboAttackDamageMult();
        
        Player.WeaponHandler.Attack(damageMult, UseWeapon.GetComboAttackHitBoxSize(), AttackDir);
    }
}