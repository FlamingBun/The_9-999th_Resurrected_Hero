using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectHandler : MonoBehaviour
{
    private Dictionary<StatusEffectType, StatusBaseEffect>_activatedEffects = new ();
    
    private CharacterBaseController _characterBaseController;
    
    private void Awake()
    {
        _characterBaseController = GetComponent<CharacterBaseController>();
    }

    private void Update()
    {
        List<StatusEffectType> removeEffectKeys = new ();

        float deltaTime = Time.deltaTime;
        
        foreach (var effectKvp in _activatedEffects)
        {
            var effect = effectKvp.Value;

            if (effect.CurStack == 0)
            {
                removeEffectKeys.Add(effect.EffectType);
                continue;
            }
            
            if (effect.CurStack > 0)
            {
                effect.UpdateEffect(_characterBaseController, deltaTime);

                effect.CurDuration -= deltaTime;
            
                if (effect.CurDuration <= 0)
                {
                    effect.CurStack--;
                
                    if (effect.CurStack > 0)
                    {
                        effect.CurDuration = effect.MaxDuration;
                    }
                }
            }
        }

        foreach (var key in removeEffectKeys)
        {
            _activatedEffects.Remove(key);
        }
        
    }

    public void ApplyEffect<T>(T applyEffect) where T : StatusBaseEffect
    {
        if (_activatedEffects.TryGetValue(applyEffect.EffectType, out StatusBaseEffect activetedEffect))
        {
            activetedEffect.Apply(applyEffect);
        }
        else
        {
            _activatedEffects[applyEffect.EffectType] = applyEffect;
            applyEffect.Apply(applyEffect); 
        }
    }
}
