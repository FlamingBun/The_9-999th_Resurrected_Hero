using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상점 UI를 관리하고, 선택된 상점 인벤토리를 표시합니다.
/// </summary>
public class TownShopManager : MonoBehaviour
{
    public Dictionary<ItemType, List<ItemInstance>> ItemListsDict { get => _itemListsDict; }

    private Dictionary<int, ItemInstance> _allItemsDict;
    private Dictionary<ItemType, List<ItemInstance>> _itemListsDict;

    private string _currentNpcId;

    private void Start()
    {
        //UIManager.Instance.GetUI<GoldUI>().Enable();
    }

    public void SetNpcId(string npcId)
    {
        _currentNpcId = npcId;
    }

    public void BuyItem(int key)
    {
        Logger.Log($"[BuyItem] 시도 key: {key}");

        /*if (_allItemsDict[key].isPurchased)
        {
            Logger.Log("구매한 아이템입니다.");
            return;
        }*/

        //var price = _allItemsDict[key].TownShopData.price;
        //Logger.Log($"아이템 가격: {price}, 현재 골드: {_goldData.CurrentGold}");

        /*if (_goldData.TrySpend(price))
        {
            _allItemsDict[key].isPurchased = true;

            // 친밀도 상승 처리
            if (!string.IsNullOrEmpty(_currentNpcId))
            {
                NpcFriendshipManager.Instance.IncreaseFriendship(_currentNpcId, 5); // 상승량은 필요시 조절
                Logger.Log($"[친밀도] {_currentNpcId} → +5");
            }

            GameManager.Instance.DataManager.SaveNow();
        }
        else
        {
            Logger.Log("TrySpend 실패: 조건 이상");
        }*/
    }

    public void EquipItem(int key)
    {
        /*if (!_allItemsDict.TryGetValue(key, out var item))
        {
            Logger.LogError($"EquipItem: 아이템 없음 (key: {key})");
            return;
        }
        if (item.isEquipped)
        {
            Logger.Log("이미 장착 중입니다.");
            return;
        }

        if (item.data.type == ItemType.Skin)
        {
            UnEquipAllOfSameType(ItemType.Skin);
            //GameManager.Instance.PlayerData.Skin = item;
        }
        else
        {
            UnEquipAllExcept(ItemType.Skin);
            //GameManager.Instance.PlayerData.Weapon = item;
        }
        
        item.isEquipped = true;*/
    }

    // 같은 타입 UnEquip
    private void UnEquipAllOfSameType(ItemType type)
    {
        /*if (_itemListsDict.TryGetValue(type, out var list))
        {
            foreach (var i in list)
                i.isEquipped = false;
        }*/
    }

    // 매개변수 타입 제외하고 UnEquip
    private void UnEquipAllExcept(ItemType exceptType)
    {
        /*foreach (var pair in _allItemsDict)
        {
            if (pair.Value.isEquipped && pair.Value.data.type != exceptType)
            {
                pair.Value.isEquipped = false;
            }
        }*/
    }
}
