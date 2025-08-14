using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownItemShopHandler : MonoBehaviour
{
    public bool IsOpen => _shopUI.IsEnabled;
    public List<ItemDataSO> HandleItemDatas => _handleItemDatas;

    [SerializeField] private ItemType handleItemType;
    
    private List<ItemDataSO> _handleItemDatas = new();
    private PlayerInstance _playerInstance;
    private TownItemShopUI _shopUI;
    

    private void Start()
    {
        _shopUI = UIManager.Instance.GetUI<TownItemShopUI>();
        
        var gameManager = GameManager.Instance;

        _playerInstance = gameManager.Player.PlayerInstance;

        foreach (var itemData in gameManager.DataManager.itemDatas)
        {
            if (itemData.itemType == handleItemType)
            {
                _handleItemDatas.Add(itemData);
            }
        }
    }

    public void Enable()
    {
        _shopUI.Enable();
        _shopUI.InitShop(this);
    }

    
    public bool TryPurchase(ItemDataSO itemData)
    {
        if (_playerInstance.Gold >= itemData.price)
        {
            ItemInstance item = new ItemInstance(itemData, null);

            _playerInstance.ownedItems.Add(item);
            _playerInstance.ModifyGold(-item.price);
            
            GameManager.Instance.DataManager.SavePlayer(_playerInstance);
            return true;
        }
        AudioManager.Instance.Play(key: "ErrorClip");
        return false;
    }
}
