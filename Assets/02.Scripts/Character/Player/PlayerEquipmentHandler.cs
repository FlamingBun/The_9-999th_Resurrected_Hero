using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEquipmentHandler : MonoBehaviour
{
    public event UnityAction<ItemInstance> OnItemEquipped;
    public event UnityAction<ItemInstance> OnItemUnequipped;
        
    public Dictionary<ItemType, ItemInstance> EquippedItems => _equippedItems; 
    
    private Dictionary<ItemType, ItemInstance> _equippedItems = new();

    private PlayerController _player;

    public void Init(PlayerController player)
    {
        _player = player;
        _equippedItems = _player.PlayerInstance.equippedItems;
        
        foreach (var itemKvp in _equippedItems)
        {
            foreach (var mod in itemKvp.Value.StatModifiers)
            {
                _player.StatHandler.AddModifier(mod);
            }
        }
    }

    public void Equip(ItemInstance itemInstance)
    {
        var itemData = itemInstance.Data;
        
        ItemType itemType = itemData.itemType;
        
        Unequip(itemType);

        _equippedItems[itemType] = itemInstance;
      
        foreach (var mod in itemInstance.StatModifiers)
        {
            _player.StatHandler.AddModifier(mod);
        }
    
        OnItemEquipped?.Invoke(itemInstance);
        
        GameManager.Instance.DataManager.SavePlayer(_player.PlayerInstance);
    }

    private void Unequip(ItemType itemType)
    {
        if (_equippedItems.TryGetValue(itemType, out var equippedItemInstance))
        {
            foreach (var mod in equippedItemInstance.StatModifiers)
            {
                _player.StatHandler.RemoveModifier(mod);
            }
            
            OnItemUnequipped?.Invoke(equippedItemInstance);
            
            _equippedItems[itemType] = null;
        }
    }
}
