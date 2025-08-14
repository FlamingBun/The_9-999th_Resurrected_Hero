using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemySuperArmorHandler : MonoBehaviour
{

    private EnemyController _controller;

    private StatHandler _statHandler;
    private StatusHandler _statusHandler;
    
    private List<StatModifier> _superArmorChanceModifiers = new();
    
    private bool _canHaveSuperArmor;
    private bool _isInterruptible;
    
    private bool _haveSuperArmor;

    private Material _material;
    
    public void Init(EnemyController controller)
    {
        _controller = controller;
        
        _material = _controller.SpriteRenderer.material;
        
        _statHandler = controller.StatHandler;
        _statusHandler = controller.StatusHandler;
        
        _statusHandler.OnStatusChanged += Interrupt;
        
        _isInterruptible = _statHandler.GetStat(StatType.Interruptible).Value > 0f;
        _canHaveSuperArmor = _statHandler.GetStat(StatType.SuperArmorChance).Value > 0f;
    }

    private void OnDestroy()
    {
        if (_statusHandler != null)
        {
            _statusHandler.OnStatusChanged -= Interrupt;
        }
    }


    private void Interrupt(StatusEventData eventData)
    {
        if (_controller.IsBoss) return;

        if (eventData.StatType != StatType.Health) return;
        
        if (eventData.CurValue < eventData.PreValue)
        {
            if (!_haveSuperArmor)
            {
                SetModifier(0.1f);
            
                if (!_controller.EnemyStateMachine.isAttack||_isInterruptible)
                {
                    _controller.EnemyStateMachine.ChangeEnemyState(EnemyStates.Hit);
                    _controller.MoveHandler.MakeMove(_controller.Rigid,-_controller.LookDir);   
                }
            }
        }
    }

    public void TrySuperArmorOnAttack()
    {
        if (!_canHaveSuperArmor) return;
        
        float randomValue = Random.Range(0f, 1f);
        
        if (randomValue < _statusHandler.GetStatus(StatType.SuperArmorChance).CurValue)
        {
            SetSuperArmor();
        }
    }

    public void RemoveSuperArmor()
    {
        if (!_haveSuperArmor) return;
            
        _haveSuperArmor = false;
        _material.SetFloat(EnemyMaterialKey.OutLineAlphaKey, 0f);
        
        foreach (var modifier in _superArmorChanceModifiers)
        {
            _statHandler.RemoveModifier(modifier);
        }
        _superArmorChanceModifiers.Clear();
    }

    public void SetSuperArmor()
    {
        _haveSuperArmor = true;
        _material.SetFloat(EnemyMaterialKey.OutLineAlphaKey, 0f);
        _material.DOFloat(1f, EnemyMaterialKey.OutLineAlphaKey, 0.1f);
    }

    private void SetModifier(float adjustment)
    {
        StatModifier modifier = new()
        {
            statType = StatType.SuperArmorChance,
            modType = StatModType.Flat,
            value = adjustment,
        };
        
        _superArmorChanceModifiers.Add(modifier);
        _statHandler.AddModifier(modifier);
        _statusHandler.GetStatus(StatType.SuperArmorChance).SetValue(1f);
    }
}
