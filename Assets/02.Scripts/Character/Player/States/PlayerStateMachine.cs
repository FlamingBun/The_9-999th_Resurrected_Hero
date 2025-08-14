using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public enum PlayerStates
{
    Idle,
    Walk,
    Sprint,
    Parry,
    ComboAttack,
    DodgeAttack,
    Dodge,
    ChargeIdle,
    ChargeMove,
    ChargeAttack,
}

public enum InputBufferType
{
    None,
    Primary,
    Dodge
}


public class PlayerStateMachine : CharacterStateMachine
{
    public PlayerController Player { get; }
    public float ChargeTime { get; set; }
    
    private readonly Dictionary<PlayerStates, PlayerBaseState> _playerStates = new();
    private readonly List<InputBuffer> _inputBuffers = new();
    private readonly List<StateWindow> _statesWindows = new();

    private readonly float _windowDuration = 0.25f;
    

    private class StateWindow
    {
        public readonly PlayerStates state;
        public float timer;

        public StateWindow(PlayerStates state, float timer)
        {
            this.state = state;
            this.timer = timer;
        }
    }

    private class InputBuffer
    {
        public readonly InputBufferType bufferType;
        public Vector2 bufferDir;
        
        public InputBuffer(InputBufferType bufferType, Vector2 bufferDir)
        {
            this.bufferType = bufferType;
            this.bufferDir = bufferDir;
        }
    }
    

    public override void Update()
    {
        base.Update();
        
        for (int i = 0; i < _statesWindows.Count; i++)
        {
            var window = _statesWindows[i];
            
            window.timer -= Time.deltaTime;
            
            if (window.timer <= 0)
            {
                _statesWindows.Remove(window);
            }
        }
    }



    public void ActiveStateWindow(PlayerStates state)
    {
        var activeWindow = _statesWindows.FirstOrDefault(i => i.state == state);

        if (activeWindow != null)
        {
            activeWindow.timer = _windowDuration;
        }
        else
        {
            _statesWindows.Add(new StateWindow(state, _windowDuration));
        }
    }
    
    public void ConsumeStateWindow(PlayerStates state)
    {
        var activeWindow = _statesWindows.FirstOrDefault(i => i.state == state);

        if (activeWindow != null)
        {
            activeWindow.timer = 0;
        }
    }

    public void ClearWindow()
    {
        _statesWindows.Clear();
    }
    
    public bool HasStateWindow(PlayerStates state)
    {
        return _statesWindows.Any(i => i.state == state);
    }


    public void RecordBuffer(InputBufferType bufferType)
    {
        var recordedBuffer = _inputBuffers.FirstOrDefault(i => i.bufferType == bufferType);

        Vector2 bufferDir = bufferType switch
        {
            InputBufferType.Primary => Player.LookDir,
            InputBufferType.Dodge => Player.MoveDir  == Vector2.zero ? Player.LookDir : Player.MoveDir,
        };
        
        if (recordedBuffer != null)
        {
            recordedBuffer.bufferDir = bufferDir;
            
            _inputBuffers.Remove(recordedBuffer);
        }
        else
        {
            recordedBuffer = new InputBuffer(bufferType, bufferDir);
        }
        
        _inputBuffers.Insert(0, recordedBuffer);
    }

    public bool TryConsumeBuffer(InputBufferType bufferType, out Vector2 bufferDir)
    {
        var recordedBuffer = _inputBuffers.FirstOrDefault(i => i.bufferType == bufferType);

        bufferDir = Vector2.zero;
        
        if (recordedBuffer != null)
        {
            bufferDir = recordedBuffer.bufferDir;
            _inputBuffers.Remove(recordedBuffer);
            
            return true;
        }
        
        return false;
    }

    public InputBufferType GetLastBufferType()
    {
        if (_inputBuffers.Count > 0)
        {
            return _inputBuffers[0].bufferType;
        }

        return InputBufferType.None;
    }
    
    public bool HasBuffer(InputBufferType bufferType)
    {
        return _inputBuffers.Any(i => i.bufferType == bufferType);
    }
    
    public void ClearBuffer()
    {
        _inputBuffers.Clear();
    }
    
    
    public PlayerStateMachine(PlayerController player) : base(player)
    {
        Player = player;
    }


    public void ChangeIdleState() => ChangePlayerState(PlayerStates.Idle);
    public void ChangeWalkState() =>  ChangePlayerState(PlayerStates.Walk);
    public void ChangeSprintState() => ChangePlayerState(PlayerStates.Sprint);
    public void ChangeComboAttackState() => ChangePlayerState(PlayerStates.ComboAttack);
    public void ChangeDodgeAttackState() => ChangePlayerState(PlayerStates.DodgeAttack);

    public void ChangeDodgeState() =>   ChangePlayerState(PlayerStates.Dodge);
    
    public void RecordDodgeBuffer()
    {
        RecordBuffer(InputBufferType.Dodge);
    }
    
    public void ChangeChargeIdleState() => ChangePlayerState(PlayerStates.ChargeIdle);
    public void ChangeChargeMoveState() => ChangePlayerState(PlayerStates.ChargeMove);
    public void ChangeChargeAttackState() => ChangePlayerState(PlayerStates.ChargeAttack);


    private void ChangePlayerState(PlayerStates state)
    {
        if (!_playerStates.TryGetValue(state, out PlayerBaseState targetState))
        {
            targetState = CreateState(state);
            _playerStates[state] = targetState;
        }
        
        ChangeState(targetState);
    }
    
    private PlayerBaseState CreateState(PlayerStates state)
    {
        return state switch
        {
            PlayerStates.Idle => new PlayerIdleState(this),
            PlayerStates.Walk => new PlayerWalkState(this),
            PlayerStates.Sprint => new PlayerSprintState(this),
            PlayerStates.Parry => new PlayerParryState(this),
            PlayerStates.ComboAttack => new PlayerComboAttackState(this),
            PlayerStates.DodgeAttack => new PlayerDodgeAttackState(this),
            PlayerStates.Dodge => new PlayerDodgeState(this),
            PlayerStates.ChargeIdle => new PlayerChargeIdleState(this),
            PlayerStates.ChargeMove => new PlayerChargeMoveState(this),
            PlayerStates.ChargeAttack => new PlayerChargeAttackState(this),
            _ => throw new ArgumentException($"Unknown state: {state}")
        };
    }

 
}
