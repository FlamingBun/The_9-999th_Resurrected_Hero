using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class PlayerInstance
{
    public event UnityAction OnSoulChanged;
    public event UnityAction OnGoldChanged;
    
    public int Soul => _soul;
    public int Gold => _gold;
    
    public List<StatData> defaultStatDatas;
    public HashSet<ItemInstance> ownedItems = new();
    public Dictionary<ItemType, ItemInstance> equippedItems = new();

    private int _soul;
    private int _gold;

    public void ModifyGold(int amount)
    {
        _gold += amount;
        
        OnGoldChanged?.Invoke();
    }

    public void SetGold(int gold)
    {
        _gold = gold;
        
        OnGoldChanged?.Invoke();
    }

    public void ModifySoul(int amount)
    {
        _soul += amount;
        
        OnSoulChanged?.Invoke();
    }

    public void SetSoul(int amount)
    {
        _soul = amount;
        
        OnSoulChanged?.Invoke();
    }
}
