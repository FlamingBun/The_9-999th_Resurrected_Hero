using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class StateMachine
{
    public IState CurrentState { get; private set; }

    public virtual void ChangeState(IState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();
    }

    public virtual void Update()
    {
        CurrentState?.Update();
    }
    
    public virtual void FixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }
  
}

