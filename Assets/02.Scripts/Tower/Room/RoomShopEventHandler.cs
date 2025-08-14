using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class RoomShopEventHandler : MonoBehaviour, IRoomEvent
{
    public int Order => 0;
    public bool IsCleared => isGeneratedItems;
    
    
    [Header("상점 설정")]
    [SerializeField] private Transform itemSpawnPointsRoot;
    [SerializeField] private List<ItemDataSO> shopItemDatas;

    
    private TowerRoomShopUI _spawnedUI;
    private RoomController _room;
    private RoomShopItem _triggerItem;
    private FloorManager _floorManager;

    private bool isGeneratedItems = false;

    private void Awake()
    {
        _floorManager = FindAnyObjectByType<FloorManager>();
    }
    
    
    public void Init(RoomController room)
    {
        _room = room;
    }
    

    public void StartEvent()
    {
        _spawnedUI = UIManager.Instance.GetUI<TowerRoomShopUI>();
        
        GenerateShop();
    }

 
    
    public bool TryBuyItem(PlayerController playerController)
    {
        int price = _triggerItem.ItemInstance.price;
        
        if (playerController.PlayerInstance.Soul >= price)
        {
            AudioManager.Instance.Play(key: "BuyItemClip");
            var statModifiers = _triggerItem.ItemInstance.StatModifiers;
            var statusModifiers = _triggerItem.ItemInstance.StatusModifiers;

            foreach (var statModifier in statModifiers)
            {
                playerController.StatHandler.AddModifier(statModifier);
            }
            
            foreach (var statusModifier in statusModifiers)
            {
                playerController.StatusHandler.ModifyStatus(statusModifier.statType, statusModifier.value);
            }

            if (_triggerItem.ItemInstance.Data.itemType == ItemType.Gem)
            {
                UIManager.Instance.GetUI<TowerHUDUI>().AddGemEffect(_triggerItem.ItemInstance.Data.icon);
            }

            playerController.PlayerInstance.ModifySoul(price * -1);
            return true;
        }
        return false;
    }

    public void OnItemTriggerEntered(RoomShopItem item)
    {
        _triggerItem = item;
        
        _spawnedUI.Enable();
        _spawnedUI.SetItem(_triggerItem.ItemInstance);
    }

    public void OnItemTriggerExited()
    {
        _triggerItem = null;
        
        _spawnedUI.Disable();
    }
    

    private void GenerateShop()
    {
        List<ItemDataSO> chosenItems = new();

        for (int i = 0; i < itemSpawnPointsRoot.childCount; i++)
        {
            if (i == 0)
            {
                chosenItems.Add(GetTypeItem(ItemType.Consumable));
            }
            else
            {
                int loopCheckCount = 0;
                
                while (loopCheckCount < 1000)
                {
                    loopCheckCount++;

                    var gemItem = GetTypeItem(ItemType.Gem);
                    
                    if (!chosenItems.Contains(gemItem))
                    {
                        chosenItems.Add(gemItem);
                        break;
                    }
                }
            }
        }
        
        for (int i = 0; i < chosenItems.Count && i < itemSpawnPointsRoot.childCount; i++)
        {
            var point = itemSpawnPointsRoot.GetChild(i);

            ItemInstance itemInstance = new(chosenItems[i], Constant.FloorSceneName);
            itemInstance.price = Mathf.RoundToInt(_floorManager.GetFeatureMultiplier(TowerCurseType.ShopPrice) * itemInstance.price);
            
            
            RoomShopItem generateItem = Instantiate(chosenItems[i].prefab, point).GetComponent<RoomShopItem>();
            generateItem.transform.position = point.position;
            generateItem.Init(this, itemInstance);
        }
        

        isGeneratedItems = true;
    }


    private ItemDataSO GetTypeItem(ItemType itemType)
    {
        List<ItemDataSO> consumeItems = shopItemDatas.FindAll(item => item.itemType == itemType).OrderBy(_ => Random.value).ToList();

        return consumeItems[0];
    }
   
}
