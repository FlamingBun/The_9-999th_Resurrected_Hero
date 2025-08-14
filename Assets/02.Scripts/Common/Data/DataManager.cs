using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class DataManager : MonoBehaviour
{
    public PlayerDataSO playerDataSO;
    public List<ItemDataSO> itemDatas;

    private PlayerSaveData _playerSaveData;
    private string _savePath;

    private void Awake()
    {
        _savePath = Path.Combine(Application.persistentDataPath, "save.json");
        //ClearSave();
    }
    public void Init()
    {
        _playerSaveData = new PlayerSaveData();
    }

    public PlayerInstance LoadPlayer()
    {
        if (File.Exists(_savePath))
        {
            string json = File.ReadAllText(_savePath);
            _playerSaveData = JsonUtility.FromJson<PlayerSaveData>(json);
            return ConvertSaveDataToInstance(_playerSaveData);
        }
        else
        {
            // 초기 데이터 생성
            PlayerInstance playerInstance = new PlayerInstance();

            ItemInstance defaultItem = new(playerDataSO.defaultWeaponData, null);
            playerInstance.ownedItems.Add(defaultItem);
            playerInstance.equippedItems[defaultItem.Data.itemType] = defaultItem;
            playerInstance.defaultStatDatas = playerDataSO.statDatas;

            return playerInstance;
        }
    }


    public void SavePlayer(PlayerInstance playerInstance)
    {
        _playerSaveData = ConvertInstanceToSaveData(playerInstance);

        string json = JsonUtility.ToJson(_playerSaveData, true); // pretty print
        File.WriteAllText(_savePath, json);

        Debug.Log("Data Saved: " + _savePath);
    }
    
    public void ClearSave()
    {
        if (File.Exists(_savePath))
        {
            File.Delete(_savePath);
            Debug.Log("Save data cleared.");
        }
        else
        {
            Debug.Log("No save file to delete.");
        }
    }

    private PlayerSaveData ConvertInstanceToSaveData(PlayerInstance instance)
    {
        PlayerSaveData saveData = new PlayerSaveData();
        saveData.gold = instance.Gold;

        saveData.ownedItemsName = new List<string>();
        foreach (var item in instance.ownedItems)
        {
            saveData.ownedItemsName.Add(item.Data.itemName);
        }

        saveData.equippedItemsName = new List<string>();
        foreach (var kvp in instance.equippedItems)
        {
            Debug.Log(kvp.Value.Data.itemName);
            saveData.equippedItemsName.Add(kvp.Value.Data.itemName);
        }

        return saveData;
    }

    private PlayerInstance ConvertSaveDataToInstance(PlayerSaveData saveData)
    {
        PlayerInstance instance = new PlayerInstance();
        instance.SetGold(saveData.gold);

        foreach (string itemName in saveData.ownedItemsName)
        {
            ItemDataSO dataSO = itemDatas.Find(x => x.itemName == itemName);
            if (dataSO != null)
            {
                instance.ownedItems.Add(new ItemInstance(dataSO, null));
            }
        }

        foreach (string itemName in saveData.equippedItemsName)
        {
            ItemDataSO dataSO = itemDatas.Find(x => x.itemName == itemName);
            if (dataSO != null)
            {
                var itemInstance = new ItemInstance(dataSO, null);
                instance.equippedItems[dataSO.itemType] = itemInstance;
            }
        }

        instance.defaultStatDatas = playerDataSO.statDatas;

        return instance;
    }

}
