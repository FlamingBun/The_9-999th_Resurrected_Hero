using System;
using UnityEngine.Events;

public class Status
{
    public Status(Stat stat)
    {
        _stat = stat;
    }

    public float CurValue
    {
        get
        {
            _curValue = Math.Clamp(_curValue, 0, _stat.Value);
            return _curValue;
        }
        set
        {
            value =  Math.Clamp(value, 0, _stat.Value);
            _curValue = value;
        }
    }
    public float MaxValue => _stat.Value;
    
    
    private readonly Stat _stat;
    private float _curValue;

    public void SetValue(float value) => _curValue = value;

    public void ModifyValue(float amount) => _curValue += amount;   
}

