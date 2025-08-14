using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// 퀘스트 완료 시 잠깐 뜨는 UI입니다. DOTween으로 자동 페이드 처리됩니다.
/// </summary>
public class QuestCompleteUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private CanvasGroup canvasGroup;

    private float fadeDuration = 0.5f;
    private float stayDuration = 1.2f;

    /// <summary>
    /// 텍스트 설정
    /// </summary>
    public void SetText(string message)
    {
        messageText.text = message;
    }

    /// <summary>
    /// 팝업 애니메이션 재생
    /// </summary>
    public void PlayAnimation()
    {
        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one;

        Sequence seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(1f, fadeDuration))
           .AppendInterval(stayDuration)
           .Append(canvasGroup.DOFade(0f, fadeDuration))
           .OnComplete(() => Destroy(gameObject));
    }
}
