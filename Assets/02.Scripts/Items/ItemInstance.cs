using System.Collections.Generic;
using UnityEngine;


public class ItemInstance
{
    public ItemDataSO Data => _data;
    public List<StatModifier> StatModifiers => _statModifiers;
    public List<StatModifier> StatusModifiers => _statusModifiers;

    public int price;
    
    private ItemDataSO _data;
    private List<StatModifier> _statModifiers = new();
    private List<StatModifier> _statusModifiers = new();
    

    
    //public List<ItemOption> options;

    public ItemInstance(ItemDataSO data, object soruce = null)
    {
        _data = data;

        price = data.price;

        foreach (var modData in data.statModifierDatas)
        {
            _statModifiers.Add(new StatModifier()
                {
                    statType = modData.statType,
                    modType = modData.modType,
                    value = modData.value,
                    source = soruce
                });
        }
        
        foreach (var modData in data.statusModifierDatas)
        {
            _statusModifiers.Add(new StatModifier()
            {
                statType = modData.statType,
                modType = modData.modType,
                value = modData.value,
                source = soruce
            });
        }
    }

}
