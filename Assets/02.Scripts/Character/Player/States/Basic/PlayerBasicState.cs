using System.Collections;
using DG.Tweening;
using UnityEngine;

public abstract class PlayerBasicState : PlayerBaseState
{
    public PlayerBasicState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        InputController.OnLook += Player.UpdateLookDir;
        InputController.OnMove += Player.UpdateMoveDir;
        InputController.OnInteract += Player.InteractionHandler.Interact;
        InputController.OnPrimaryCancel += OnPrimaryCancel;
        
        ToggleAnimState(PlayerConstant.BasicHash, true);
    }
    
    public override void Exit()
    {
        base.Exit();
        
        InputController.OnLook -= Player.UpdateLookDir;
        InputController.OnMove -= Player.UpdateMoveDir;
        InputController.OnInteract -= Player.InteractionHandler.Interact;
        InputController.OnPrimaryCancel -= OnPrimaryCancel;

        ToggleAnimState(PlayerConstant.BasicHash, false);
    }

    protected abstract void OnPrimaryCancel(float holdTime);

}
