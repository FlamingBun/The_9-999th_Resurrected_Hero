using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI 오브젝트에 마우스를 올렸을 때 효과음을 재생합니다.
/// </summary>
public class UiButtonHoverSound : MonoBehaviour, IPointerEnterHandler
{
    [Tooltip("AudioManager에 등록된 사운드 키 (예: ButtonHoverClip)")]
    [SerializeField] private string sfxKey;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.7f;

    /// <summary>
    /// 마우스 커서가 UI 위로 진입할 때 호출됩니다.
    /// </summary>
    /// <param name="eventData">이벤트 데이터</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.Play(sfxKey, volume);
    }
}
