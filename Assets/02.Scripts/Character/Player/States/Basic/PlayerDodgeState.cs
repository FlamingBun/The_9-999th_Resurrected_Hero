using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerDodgeState : PlayerBasicState
{
    private Coroutine _dodgeCoroutine;

    public PlayerDodgeState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

    public override void Enter()
    {
        base.Enter();
        
        Player.ToggleLockMoveBack(true);
     
        _dodgeCoroutine = Player.StartCoroutine(DodgeCoroutine());
    }

    public override void Exit()
    {
        base.Exit();
        
        Player.ToggleLockMoveBack(false);
        
        if (_dodgeCoroutine != null)
        {
            Player.StopCoroutine(_dodgeCoroutine);
        }
    }

    protected override void OnPrimaryCancel(float holdTime)
    {
        if (holdTime < 0.2f)
        {
            StateMachine.RecordBuffer(InputBufferType.Primary);
        }
    }
    

    public override void Update()
    {
        base.Update();
        
        Player.VisualHandler.UpdateModelToLookDir();
        Player.WeaponHandler.HandleWeapon();
    }


    private IEnumerator DodgeCoroutine()
    {
        float dodgeTimer = 0.25f;
        float ghostOffTime = 0.1f;
        float dodgeDist = 3.5f;

        bool hasComboWindow = StateMachine.HasStateWindow(PlayerStates.ComboAttack);
        bool hasDodgeWindow = StateMachine.HasStateWindow(PlayerStates.DodgeAttack);

        Vector2 dodgeDir = StateMachine.TryConsumeBuffer(InputBufferType.Dodge, out Vector2 bufferDir) ? 
            bufferDir :
            Player.MoveDir == Vector2.zero ? Player.LookDir : Player.MoveDir;
     
        Vector2 targetPos = Rigid.position + dodgeDir * dodgeDist;

        AudioManager.Instance.Play("PlayerDashClip");

        Rigid.DOKill();
        Rigid.DOMove(targetPos, dodgeTimer);

        Player.VisualHandler.ActiveAfterImageEffect(dodgeTimer);
        Player.VisualHandler.ActiveDodgeGhostEffect(dodgeTimer + 0.1f);
        Player.CanDamageable = false;

        while (dodgeTimer > 0)
        {
            dodgeTimer -= Time.deltaTime;

            if (ghostOffTime >= dodgeTimer)
            {
                if (!Player.CanDamageable)
                {
                    Player.CanDamageable = true;
                }
            }

            if (!hasComboWindow && !hasDodgeWindow)
            {
                if (StateMachine.GetLastBufferType() == InputBufferType.Primary)
                {
                    StateMachine.ChangeDodgeAttackState();
                    yield break;
                }
            }
            
            yield return null;
        }
 
        if (StateMachine.GetLastBufferType() == InputBufferType.Primary)
        {
            if (hasComboWindow)
            {
                StateMachine.ActiveStateWindow(PlayerStates.ComboAttack);
            }
            StateMachine.ChangeComboAttackState();
            yield break;
        }
        
        StateMachine.ChangeIdleState();
    }
  
}