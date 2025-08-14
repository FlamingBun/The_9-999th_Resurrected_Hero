using UnityEngine;

[System.Serializable]
public class QuestEntry
{
    [Header("퀘스트 정보")]
    public string questId;
    public string title;
    [TextArea(2, 5)] public string description;

    public bool isCleared;

    [Header("진행도")]
    public int currentCount;
    public int targetCount;

    public string GetProgressText()
    {
        return $"{description} ({currentCount}/{targetCount})";
    }
}
