using System.Collections;
using UnityEngine;

public class PlayerSprintState : PlayerMoveState
{
    private float _sprintSpeedMultiplier = PlayerConstant.SprintSpeedMultiplier;
    private Coroutine _runSfxCoroutine;

    public PlayerSprintState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        Anim.speed = _sprintSpeedMultiplier;

        _runSfxCoroutine = Player.StartCoroutine(PlayRunSfxCoroutine());
    }

    public override void Exit()
    {
        base.Exit();

        Anim.speed = 1;

        if (_runSfxCoroutine != null)
        {
            Player.StopCoroutine(_runSfxCoroutine);
            _runSfxCoroutine = null;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Player.SetMove(_sprintSpeedMultiplier);
    }

    public override void Update()
    {
        base.Update();

        if (!Player.InputController.IsSprintHold || Player.IsLockedSprint)
        {
            StateMachine.ChangeIdleState();
        }
    }
    
    protected override void OnPrimaryCancel(float holdTime)
    {
        if (holdTime < 0.2f)
        {
            StateMachine.ChangeComboAttackState();
        }
    }

    private IEnumerator PlayRunSfxCoroutine()
    {
        while (true)
        {
            if (Player.MoveDir != Vector2.zero)
            {
                AudioManager.Instance.Play("PlayerRunClip");
            }

            // 더 빠르게 반복 (걷기보다 간격 짧게)
            yield return new WaitForSeconds(0.25f);
        }
    }
}