using UnityEngine;
using System;

[Serializable]
public class NpcFriendship
{
    public string npcId; // 예: "TutorialNPC"
    [SerializeField] private int friendship;

    public int Friendship => friendship;

    public void IncreaseFriendship(int amount)
    {
        friendship += amount;
        Logger.Log($"[{npcId}] 친밀도 증가: +{amount} → 현재 {friendship}");
    }
}
