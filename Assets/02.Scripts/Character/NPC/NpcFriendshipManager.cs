using System.Collections.Generic;
using UnityEngine;

public class NpcFriendshipManager : MonoBehaviour
{
    public static NpcFriendshipManager Instance { get; private set; }

    [SerializeField] private List<NpcFriendship> friendshipList = new();

    private Dictionary<string, NpcFriendship> _friendshipDict = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var entry in friendshipList)
        {
            _friendshipDict[entry.npcId] = entry;
        }
    }

    public int GetFriendship(string npcId)
    {
        return _friendshipDict.TryGetValue(npcId, out var data) ? data.Friendship : 0;
    }

    public void IncreaseFriendship(string npcId, int amount)
    {
        if (_friendshipDict.TryGetValue(npcId, out var data))
        {
            data.IncreaseFriendship(amount);
        }
        else
        {
            Logger.LogError($"해당 NPC({npcId})의 친밀도 데이터를 찾을 수 없습니다.");
        }
    }
}
