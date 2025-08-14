using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 퀘스트 UI 생성 및 업데이트를 담당하는 클래스입니다.
/// </summary>
public class QuestUIRenderer : MonoBehaviour
{
    [SerializeField] private GameObject questTMPPrefab;
    [SerializeField] private Transform questListParent;

    private Dictionary<string, TextMeshProUGUI> questTextMap = new();

    /// <summary>
    /// 퀘스트 UI 추가
    /// </summary>
    public void ShowQuest(string questId, string text)
    {
        var go = Instantiate(questTMPPrefab, questListParent);
        go.name = questId;

        var tmpText = go.GetComponentInChildren<TextMeshProUGUI>();
        tmpText.text = text;
        questTextMap[questId] = tmpText;
    }

    /// <summary>
    /// 퀘스트 UI 갱신
    /// </summary>
    public void UpdateQuestText(string questId, string text)
    {
        if (questTextMap.TryGetValue(questId, out var tmp))
        {
            tmp.text = text;
        }
    }

    /// <summary>
    /// 퀘스트 UI 제거
    /// </summary>
    public void RemoveQuest(string questId)
    {
        if (questTextMap.TryGetValue(questId, out var tmp))
        {
            Destroy(tmp.transform.parent.gameObject);
            questTextMap.Remove(questId);
        }
    }
}
