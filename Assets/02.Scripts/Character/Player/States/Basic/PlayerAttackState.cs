using System.Collections;
using DG.Tweening;
using UnityEngine;

public abstract class PlayerAttackState : PlayerBasicState
{
    protected Vector2 AttackDir { get; set; }
    protected BaseWeapon UseWeapon { get; private set; }

    private bool _isOverAttackFrame;
    
    public PlayerAttackState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }
    
    public override void Enter()
    {
        base.Enter();

        UseWeapon = Player.WeaponHandler.UseWeapon;

        AttackDir = StateMachine.TryConsumeBuffer(InputBufferType.Primary, out Vector2 bufferDir) ?
            bufferDir : 
            Player.LookDir;
        
        Player.UpdateLookDir(AttackDir);
        Player.VisualHandler.UpdateModelToLookDir();
        Player.WeaponHandler.HandleWeapon();
        

        ToggleAnimState(PlayerConstant.AttackHash, true);
        
        float attackAnimSpeed = Player.StatHandler.GetStat(StatType.AttackSpeed).Value;
        Anim.speed = attackAnimSpeed;
        UseWeapon.Anim.speed = attackAnimSpeed;
        
        
        UseWeapon.OnFinishAttack += CheckOverAttackFrame;
        UseWeapon.OnFinish += StateMachine.ChangeIdleState;
        UseWeapon.OnFinish += StateMachine.ClearBuffer;
        
        InputController.OnDodge += StateMachine.RecordDodgeBuffer;
    }

    public override void Exit()
    {
        base.Exit();
        
        
        _isOverAttackFrame = false;
        
        ToggleAnimState(PlayerConstant.AttackHash, false);
        
        UseWeapon.OnFinishAttack -= CheckOverAttackFrame;
        UseWeapon.OnFinish -= StateMachine.ChangeIdleState;
        UseWeapon.OnFinish -= StateMachine.ClearBuffer;
        
        InputController.OnDodge -= StateMachine.RecordDodgeBuffer;
    }
    
    
    public override void Update()
    {
        base.Update();

        if (_isOverAttackFrame)
        {
            CheckInputBuffer();
        }
    }
    
    private void CheckInputBuffer()
    {
        if (StateMachine.HasBuffer(InputBufferType.Dodge))
        {
            StateMachine.ChangeDodgeState();
            return;
        }
                
        if (StateMachine.HasBuffer(InputBufferType.Primary))
        {
            StateMachine.ChangeComboAttackState();
            return;
        }
                
        if (Player.MoveDir != Vector2.zero)
        {
            StateMachine.ChangeWalkState();
            return;
        }
    }

    
    
    protected override void OnPrimaryCancel(float holdTime)
    {
        if (holdTime < 0.2f)
        {
            StateMachine.RecordBuffer(InputBufferType.Primary);
        }
    }

 
    private void CheckOverAttackFrame() =>  _isOverAttackFrame = true;

    private void HandleAttackQuestProgress()
    {
        QuestManager questManager = GameObject.FindObjectOfType<QuestManager>();
        questManager?.IncrementQuestProgress("tutorial_001");
    }
}