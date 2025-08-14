using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Stat
{
    public StatType StatType => _statType;
    public float Value => GetFinalValue();
    
    private readonly List<StatModifier> _statModifiers = new();
    private StatType _statType;
    private float _baseValue;
    private float _limitValue;
    private float _value;

    public Stat(StatData statData)
    {
        _statType = statData.statType;
        _baseValue = statData.value;
        _limitValue = statData.limitValue;

        /*if (_limitValue > 0)
        {
            _baseValue = Mathf.Min(_limitValue, _baseValue);
        }*/
    }
    
    public void AddModifier(StatModifier modifier) => _statModifiers.Add(modifier);
    
    public bool RemoveModifier(StatModifier modifier)
    {
        if (_statModifiers.Remove(modifier))
        {
            return true;
        }
        return false;
    }

    public void RemoveAllModifiersFromSource(object source)
    {
        _statModifiers.RemoveAll(mod => mod.source == source);
    }
    
    private float GetFinalValue()
    {
        float finalValue = _baseValue;
        float sumPercentAdd = 0;

        // 적용 순서에 따라 정렬
        var modifiers = _statModifiers.OrderBy(_ => _.order).ToList();

        for (int i = 0; i < modifiers.Count; i++)
        {
            StatModifier mod = modifiers[i];

            if (mod.modType == StatModType.Flat)
            {
                finalValue += mod.value;
            }
            else if (mod.modType == StatModType.PercentAdd)
            {
                
                sumPercentAdd += mod.value;

                // 다음 Modifier가 PercentAdd가 아니라면 누적 적용
                if (i + 1 >= modifiers.Count || modifiers[i + 1].modType != StatModType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }
            else if (mod.modType == StatModType.PercentMult)
            {
                finalValue *= 1 + mod.value;
            }
        }

        // 부동소수점 오차 보정
        finalValue = (float)Math.Round(finalValue, 3);
            
        finalValue = _limitValue > 0 ? 
            Mathf.Min(_limitValue, finalValue) :
            finalValue;

        return finalValue;
    }
}

