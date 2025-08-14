using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 퀘스트를 제공하는 NPC에 붙는 전용 스크립트입니다.
/// DialogueController 대사 종료 시점에 퀘스트를 자동 수락 처리합니다.
/// </summary>
public class QuestGiverNPCController : MonoBehaviour
{
    [FormerlySerializedAs("questDatabaseSO")] [SerializeField] private QuestDataSO questDataSO;
    [SerializeField] private QuestPresenter questPresenter;

    public void TryGiveQuest()
    {
        if (questDataSO == null || questPresenter == null)
            return;

        /*// 순차적 퀘스트 제공: 아직 클리어되지 않은 첫 번째 퀘스트를 찾아 전달
        foreach (var quest in questDatabaseSO.quests)
        {
            if (!quest.isCleared)
            {
                questPresenter.RegisterQuest(quest);
                return;
            }
        }*/

        Debug.Log($"[QuestGiverNPCController] 모든 퀘스트가 완료되어 더 이상 줄 퀘스트가 없습니다.");
    }
}
