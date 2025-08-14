using UnityEngine;

public class CharacterStateMachine : StateMachine
{
    public CharacterBaseController CharacterBaseController { get; private set; }
    

    public CharacterStateMachine(CharacterBaseController characterBaseController)
    {
        CharacterBaseController = characterBaseController;
    }

    public void AnimationEvent(string eventName)
    {
        (CurrentState as CharacterBaseState)?.AnimationEvent(eventName);
    }
}
