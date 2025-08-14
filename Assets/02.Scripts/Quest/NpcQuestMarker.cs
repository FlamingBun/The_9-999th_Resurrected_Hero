using UnityEngine;

/// <summary>
/// NPC 머리 위에 퀘스트 마커(느낌표)를 표시/숨김 처리하는 컴포넌트입니다.
/// </summary>
public class NpcQuestMarker : MonoBehaviour
{
    [SerializeField] private GameObject questMark;
    [SerializeField] private QuestDataSO questDB;

    private void Start()
    {
        UpdateQuestMark();
    }

    public void UpdateQuestMark()
    {
        /*bool hasQuest = questDB.quests.Exists(q => !q.isCleared);
        questMark.SetActive(hasQuest);*/
    }
}
