using System.Collections;
using UnityEngine;


public class PlayerWalkState : PlayerMoveState
{

    private Coroutine _walkSfxCoroutine;

    public PlayerWalkState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }


    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Player.SetMove();
    }
    
    public override void Update()
    {
        base.Update();

        if (Player.InputController.IsSprintHold && !Player.IsLockedSprint)
        {
            StateMachine.ChangeSprintState();
        }
    }
    
    public override void Enter()
    {
        base.Enter();

        _walkSfxCoroutine = Player.StartCoroutine(PlayWalkSfxCoroutine());
    }

    public override void Exit()
    {
        base.Exit();

        if (_walkSfxCoroutine != null)
        {
            Player.StopCoroutine(_walkSfxCoroutine);
            _walkSfxCoroutine = null;
        }
    }

    protected override void OnPrimaryCancel(float holdTime)
    {
        if (holdTime < 0.2f)
        {
            StateMachine.ChangeComboAttackState();
        }
    }

    private IEnumerator PlayWalkSfxCoroutine()
    {
        while (true)
        {
            if (Player.MoveDir != Vector2.zero)
            {
                AudioManager.Instance.Play("PlayerWalkClip");
            }

            // 발소리 간격 (속도 반영도 가능)
            yield return new WaitForSeconds(0.4f);
        }
    }
}