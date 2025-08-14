using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "itemDataSO", menuName = "Scriptable Objects/Item/ItemDataSO", order = 0)]
public class ItemDataSO : ScriptableObject
{
    public ItemType itemType;
    
    [Space(10f)]
    public string itemName;
    public string description;
    
    [Space(10f)]
    public int price;

    [Space(10f)]
    public List<StatModifierData> statModifierDatas;
    public List<StatModifierData> statusModifierDatas;
        
    [Space(10f)]
    public Sprite icon;
    public GameObject prefab;
}
