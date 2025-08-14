using UnityEngine;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// 퀘스트 수락 시 말풍선처럼 잠깐 보여주는 퀘스트 알림 UI입니다.
/// 플레이어가 아무 키나 누르면 사라집니다.
/// </summary>
public class QuestUI : BaseUI
{
    public UnityEvent onHide;

    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Vector3 wordOffset;

    private RectTransform rectTransform;
    private Camera mainCamera;
    private Transform target;
    private QuestEntry questEntry;
    private bool isFollowing = false;

    public override CanvasLayer Layer => CanvasLayer.Popup;
    public override bool IsEnabled => gameObject.activeSelf;
    public string QuestId => questEntry?.questId;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (!isFollowing || target == null || mainCamera == null)
            return;

        Vector3 worldPos = target.position + wordOffset;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
        rectTransform.position = screenPos;

        if (Input.anyKeyDown)
        {
            Hide();
        }

        if (questEntry != null)
        {
            descriptionText.text = questEntry.GetProgressText();
        }
    }

    public override void Enable()
    {
        gameObject.SetActive(true);
    }

    public override void Disable()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 퀘스트 텍스트를 표시하고, 플레이어가 아무 키나 누를 때까지 표시됩니다.
    /// 카메라가 있을 경우, NPC 머리 위에 따라붙습니다.
    /// </summary>
    public void Show(QuestEntry quest, Transform npcTransform)
    {
        Logger.Log($"[QuestUI.Show] 호출됨 - quest: {quest?.questId}");

        questEntry = quest;
        descriptionText.text = quest.GetProgressText();
        target = npcTransform;
        isFollowing = true;
        Enable();
    }

    /// <summary>
    /// 진행도 등 텍스트 수동 갱신
    /// </summary>
    public void UpdateText()
    {
        if (questEntry != null)
        {
            descriptionText.text = questEntry.GetProgressText();
        }
    }

    /// <summary>
    /// 외부에서 직접 제거를 원할 때 호출 (ex. 퀘스트 완료 시)
    /// </summary>
    public void RemoveSelf()
    {
        Logger.Log($"[QuestUI] 제거 요청 - questId: {QuestId}");
        Disable();
    }


    private void Hide()
    {
        isFollowing = false;
        Disable();
        onHide?.Invoke();
    }
}
