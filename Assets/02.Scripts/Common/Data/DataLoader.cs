using System;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader<T> where T : class
{
    public List<T> DataList { get; private set; }
    public Dictionary<int, T> DataDict { get; private set; }

 
    public DataLoader(string path = "JSON/ItemData")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        DataList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        DataDict = new Dictionary<int, T>();
        foreach (var data in DataList)
        {
            var keyProp = data.GetType().GetField("key");
            if (keyProp != null)
            {
                int key = (int)keyProp.GetValue(data);
                DataDict.Add(key, data);
            }
            else
            {
                Logger.LogError($"DataLoader: {typeof(T)}에는 key가 없다.");
            }
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<T> Items;
    }

    public T GetByKey(int key)
    {
        if (DataDict.ContainsKey(key))
        {
            return DataDict[key];
        }
        return null;
    }
    public T GetByIndex(int index)
    {
        if (index >= 0 && index < DataList.Count)
        {
            return DataList[index];
        }
        return null;
    }
}