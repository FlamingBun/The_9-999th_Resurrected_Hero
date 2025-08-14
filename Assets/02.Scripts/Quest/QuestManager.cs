using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class QuestManager : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private QuestCompleteUI questCompleteUIPrefab;
    [SerializeField] private Transform uiCanvas;

    [FormerlySerializedAs("questDatabaseSO")]
    [Header("모든 퀘스트 목록 (직접 할당)")]
    [SerializeField] private QuestDataSO questDataSO;

    private List<string> activeQuests = new();
    private List<string> completedQuests = new();
    private Dictionary<string, QuestEntry> questMap = new(); // questId -> QuestEntry 매핑

  
    // TESTCODE

    // private void Update()
    // {
    //     // 테스트용: C 키를 눌렀을 때 첫 번째 퀘스트 클리어 처리
    //     if (Input.GetKeyDown(KeyCode.C))
    //     {
    //         if (activeQuests.Count > 0)
    //         {
    //             string firstQuestId = activeQuests[0];
    //             Logger.Log($"[Test] C 키 입력: 퀘스트 클리어 시도 → {firstQuestId}");
    //             ClearQuest(firstQuestId);
    //         }
    //         else
    //         {
    //             Logger.Log("[Test] 진행 중인 퀘스트가 없습니다.");
    //         }
    //     }
    //
    //     // 테스트용: V 키 누르면 진행도 +1
    //     if (Input.GetKeyDown(KeyCode.V))
    //     {
    //         if (activeQuests.Count > 0)
    //         {
    //             string firstQuestId = activeQuests[0];
    //             Logger.Log($"[Test] V 키 입력: 퀘스트 진행도 +1 → {firstQuestId}");
    //             IncrementQuestProgress(firstQuestId);
    //         }
    //     }
    // }

    /// <summary>
    /// 퀘스트를 수락하고 UI에 표시합니다.
    /// </summary>
    /// <param name="questId">수락할 퀘스트의 고유 ID</param>
    public void AddQuest(string questId, Transform npcTransform)
    {
        Logger.Log($"[AddQuest] 호출됨! questId: {questId}");

        if (completedQuests.Contains(questId))
            return;

        if (!activeQuests.Contains(questId))
        {
            Logger.Log($"[AddQuest] 수락 시도 ID: '{questId}'");


            activeQuests.Add(questId);

            var questEntry = GetQuestById(questId);
            if (questEntry != null)
            {
                var questUI = UIManager.Instance.CreateUI<QuestUI>(gameObject.scene);
                questUI.Show(questEntry, npcTransform);
            }
            else
            {
                Debug.LogWarning($"퀘스트 ID '{questId}'에 해당하는 데이터가 없습니다!");
            }
        }
    }

    /// <summary>
    /// 퀘스트를 클리어 처리하고 UI 목록에서 제거합니다.
    /// 내부적으로 isCleared 값을 true로 바꾸고, 이후 CompleteQuest에서 보상을 지급합니다.
    /// </summary>
    /// <param name="questId">클리어할 퀘스트의 고유 ID</param>
    public void ClearQuest(string questId)
    {
        activeQuests.Remove(questId);

        var questUI = UIManager.Instance.GetUI<QuestUI>();
        if (questUI != null && questUI.QuestId == questId)
        {
            questUI.RemoveSelf(); // 직접 제거
        }

        var questData = GetQuestById(questId);
        if (questData != null)
            questData.isCleared = true;

        CompleteQuest(questId);
    }



    /// <summary>
    /// 퀘스트를 완료 처리하며, 내부 목록에서 상태를 갱신합니다.
    /// </summary>
    /// <param name="questId">완료할 퀘스트의 고유 ID</param>
    public void CompleteQuest(string questId)
    {
        // 중복 완료 방지
        if (completedQuests.Contains(questId))
            return;

        completedQuests.Add(questId);

        var questEntry = GetQuestById(questId);
        if (questEntry != null)
        {
            Logger.Log($"퀘스트 완료됨: {questEntry.title}");


            // 특정 퀘스트 완료 시 친밀도 증가
            if (questId == "tutorial_003")
            {
                NpcFriendshipManager.Instance.IncreaseFriendship("NPC_Tutorial", 5);
                Logger.Log($"[친밀도] NPC_Tutorial의 친밀도가 5 증가되었습니다.");
            }
        }
        else
        {
            Logger.LogError($"퀘스트 ID '{questId}'에 해당하는 데이터가 없습니다.");
        }

        foreach (var npc in FindObjectsOfType<NpcQuestMarker>())
        {
            npc.UpdateQuestMark();
        }

        ShowQuestCompletePopup(questEntry.title);
    }



    public QuestEntry GetQuestById(string questId)
    {
        questMap.TryGetValue(questId, out var quest);
        return quest;
    }

    public void IncrementQuestProgress(string questId, int amount = 1)
    {
        if (!activeQuests.Contains(questId)) return;

        var questEntry = GetQuestById(questId);
        if (questEntry == null || questEntry.isCleared) return;

        questEntry.currentCount += amount;
        questEntry.currentCount = Mathf.Min(questEntry.currentCount, questEntry.targetCount);

        // UI 갱신
        var questUI = UIManager.Instance.GetUI<QuestUI>();
        if (questUI != null && questUI.QuestId == questId)
        {
            questUI.UpdateText();
        }

        if (questEntry.currentCount >= questEntry.targetCount)
        {
            ClearQuest(questId);
        }
    }

    /// <summary>
    /// 해당 퀘스트가 현재 진행 중인지 확인합니다.
    /// </summary>
    public bool IsQuestAccepted(string questId)
    {
        return activeQuests.Contains(questId);
    }

    public bool IsQuestCleared(string questId)
    {
        return completedQuests.Contains(questId);
    }

   

    private void ShowQuestCompletePopup(string questTitle)
    {
        var popup = Instantiate(questCompleteUIPrefab, uiCanvas);
        popup.SetText($"퀘스트 완료: {questTitle}");
        popup.PlayAnimation();
    }
}
