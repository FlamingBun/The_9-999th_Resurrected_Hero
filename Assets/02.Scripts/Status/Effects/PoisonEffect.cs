using UnityEngine;

public class PoisonEffect : StatusBaseEffect
{
    public override StatusEffectType EffectType => StatusEffectType.Poison;

    private float _tickDamage;

    private float _damageInterval = 1f;
    private float _damageTimer;

    public PoisonEffect(float tickDamage, float duration, int maxStack) : base(duration, maxStack)
    {
        _tickDamage = tickDamage;
    }

    public override void Apply<T>(T effect) 
    {
        if (effect is PoisonEffect poisonEffect)
        {
            CurDuration = MaxDuration;
            
            _tickDamage = poisonEffect._tickDamage;
            
            if (CurStack < MaxStack)
            {
                CurStack++;
            }
        }
    }
    

    public override void UpdateEffect(CharacterBaseController character, float deltaTime)
    {
        _damageTimer += deltaTime;

        if (_damageTimer >= _damageInterval)
        {
            _damageTimer = 0;
                
            character.StatusHandler.ModifyStatus(StatType.Health, _tickDamage * -1f); 
        }
    }
}