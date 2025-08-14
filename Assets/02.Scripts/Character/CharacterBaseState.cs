public class CharacterBaseState : IState
{
    public virtual void Enter() {}
    public virtual void Exit() {}
    public virtual void Update() {}
    public virtual void FixedUpdate() {}
    public virtual void AnimationEvent(string eventName) {}
}
