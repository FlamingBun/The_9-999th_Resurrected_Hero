using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerBaseState : CharacterBaseState
{
    protected PlayerStateMachine StateMachine { get;}
    protected PlayerController Player { get; }
    protected PlayerInputController InputController { get; }
    protected Animator Anim { get; }
    protected Rigidbody2D Rigid { get; }
    protected UIManager UIManager { get; }



    public PlayerBaseState(PlayerStateMachine playerStateMachine)
    {
        UIManager = UIManager.Instance;
        
        StateMachine = playerStateMachine;
        Player = playerStateMachine.Player;
        InputController = playerStateMachine.Player.InputController;
        Anim = playerStateMachine.Player.Anim;
        Rigid = playerStateMachine.Player.Rigid;
    }

    
    protected void ToggleAnimState(int animHash, bool enable)
    {
        Anim.SetBool(animHash, enable);

        var equippedWeapon = Player.WeaponHandler.UseWeapon;

        if (equippedWeapon != null)
        {
            equippedWeapon.Anim.SetBool(animHash, enable);
        }
    }
    

    
  
}
