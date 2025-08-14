using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class StatusHandler : MonoBehaviour
{
    public event UnityAction<StatusEventData> OnStatusChanged;
    
    private readonly Dictionary<StatType, Status> _statusDict = new();


    public void Init(StatHandler statHandler)
    {
        CreateStatus(statHandler, StatType.Health);
        CreateStatus(statHandler, StatType.SuperArmorChance);

        statHandler.OnStatChanged += ModifyStatus;
    }

    public Status GetStatus(StatType statType)
    {
        return _statusDict.GetValueOrDefault(statType);
    }

    public void ModifyStatus(StatType statType, float amount)
    {
        Status status = GetStatus(statType);

        if (status == null) return;

        float preValue = status.CurValue;
        
        _statusDict[statType].ModifyValue(amount);
        
        var eventData = new StatusEventData(statType, preValue, status.CurValue, amount, status.MaxValue);
        OnStatusChanged?.Invoke(eventData);
    }

    public void SetStatus(StatType statType, float value)
    {
        Status status = GetStatus(statType);

        if (status == null) return;

        float preValue = status.CurValue;
        
        _statusDict[statType].SetValue(value);
        
        var eventData = new StatusEventData(statType, preValue,  status.CurValue, value, status.MaxValue);
        OnStatusChanged?.Invoke(eventData);
    }


    private void CreateStatus(StatHandler statHandler, StatType statType)
    {
        Stat stat = statHandler.GetStat(statType);
        
        if (stat != null)
        {
            Status status = new Status(stat);
            status.SetValue(stat.Value);

            _statusDict.Add(statType, status);
        }
    }

    private void ModifyStatus(StatEventData eventData)
    {
        if (eventData.StatType == StatType.Health)
        {
            var health = GetStatus(StatType.Health);
            
            if (health.CurValue + eventData.EventValue <= 0)
            {
                health.ModifyValue(+1);
            }
        }
        
        ModifyStatus(eventData.StatType, eventData.EventValue);
    }
}
