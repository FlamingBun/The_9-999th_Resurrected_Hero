using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class StatHandler : MonoBehaviour
{
    public event UnityAction<StatEventData> OnStatChanged;
    
    private Dictionary<StatType, Stat> _stats = new();


    public void Init(List<StatData> statDatas)
    {
        foreach (var data in statDatas)
        {
            _stats[data.statType] = new Stat(data);
        }
    }
    

    public Stat GetStat(StatType statType)
    {
        if (_stats.TryGetValue(statType, out var stat))
        {
            return stat;
        }
        return null;
    }

    public void AddModifier(StatModifier modifier)
    {
        var targetStat = GetStat(modifier.statType);

        if (targetStat != null)
        {
            float preValue = targetStat.Value;
            
            targetStat.AddModifier(modifier);
            
            var eventData = new StatEventData(modifier.statType, preValue, targetStat.Value, modifier.value);
            
            OnStatChanged?.Invoke(eventData);
        }
    }
    
    public void RemoveModifier(StatModifier modifier)
    {
        var targetStat = GetStat(modifier.statType);
        
        if (targetStat != null)
        {
            float preValue = targetStat.Value;
            
            targetStat.RemoveModifier(modifier);
            
            var eventData = new StatEventData(modifier.statType, preValue, targetStat.Value, modifier.value * -1);
            
            OnStatChanged?.Invoke(eventData);
        }
    }

    public void RemoveModifiersFromSource(object source)
    {
        foreach (var statKvp in _stats)
        {
            float preValue = statKvp.Value.Value;
            
            statKvp.Value.RemoveAllModifiersFromSource(source);  
            
            float curValue = statKvp.Value.Value;

            float amount = curValue - preValue;
            
            var eventData = new StatEventData(statKvp.Key, preValue, curValue, amount);

            OnStatChanged?.Invoke(eventData);
        }
    }

}
