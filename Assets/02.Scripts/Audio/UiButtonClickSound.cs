using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI 버튼 클릭 시 효과음을 재생하는 컴포넌트입니다.
/// </summary>
public class UiButtonClickSound : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("AudioManager에 등록된 사운드 키 (예: ButtonClickClip)")]
    [SerializeField] private string sfxKey;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    /// <summary>
    /// UI 요소 클릭 시 호출됩니다.
    /// </summary>
    /// <param name="eventData">이벤트 데이터</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.Play(sfxKey, volume);
    }
}
