using UnityEngine;

/// <summary>
/// 퀘스트 등록 요청만 처리하는 컴포넌트입니다.
/// UI 생성과 출력은 외부에서 담당합니다.
/// </summary>
public class QuestPresenter : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;

    /// <summary>
    /// 퀘스트를 등록만 처리합니다.
    /// UI 출력은 외부에서 처리합니다.
    /// </summary>
    public void RegisterQuest(QuestEntry questEntry)
    {
        if (questEntry == null)
        {
            Logger.LogError("[QuestPresenter] QuestEntry가 null입니다.");
            return;
        }

        Logger.Log($"[QuestPresenter] 퀘스트 등록: {questEntry.questId}");
        questManager.AddQuest(questEntry.questId, this.transform);
    }

    public void RegisterQuestById(string questId)
    {
        if (questManager.IsQuestCleared(questId))
        {
            Logger.Log($"[QuestPresenter] 이미 완료된 퀘스트입니다: {questId}");
            return;
        }

        var questEntry = questManager.GetQuestById(questId);
        if (questEntry == null)
        {
            Logger.LogError($"[QuestPresenter] Quest ID '{questId}'에 해당하는 퀘스트를 찾을 수 없습니다.");
            return;
        }

        RegisterQuest(questEntry);
    }

    public void RegisterNextQuest()
    {
        /*foreach (var quest in questManager.GetAllQuests())
        {
            if (!questManager.IsQuestAccepted(quest.questId) && !questManager.IsQuestCleared(quest.questId))
            {
                RegisterQuest(quest);
                return;
            }
        }

        Logger.Log("수락할 수 있는 퀘스트가 없습니다.");*/
    }

}
