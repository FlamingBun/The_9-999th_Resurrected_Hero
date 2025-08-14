using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusBaseEffect
{
    public abstract StatusEffectType EffectType { get; }
    
    public int MaxStack { get; }
    public int CurStack { get; set; }
    
    public float MaxDuration { get;}
    public float CurDuration { get; set; }

    public StatusBaseEffect(float duration, int maxStack)
    {
        MaxDuration = duration;
        MaxStack = maxStack;
    }

    public abstract void Apply<T>(T effect) where T : StatusBaseEffect;
    public abstract void UpdateEffect(CharacterBaseController character, float deltaTime);
}
