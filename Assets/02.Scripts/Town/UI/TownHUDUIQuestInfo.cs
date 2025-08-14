using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 퀘스트 패널을 열고 닫는 UI 컨트롤러입니다.
/// 버튼 클릭 시 패널 위치를 변경하며 DOTween으로 부드럽게 애니메이션 처리합니다.
/// </summary>
public class TownHUDUIQuestInfo: MonoBehaviour
{
    [SerializeField] private Button toggleButton;

    [SerializeField] private Vector2 openedPos;  // 패널이 열렸을 때의 anchoredPosition
    [SerializeField] private Vector2 closedPos;  // 패널이 닫혔을 때의 anchoredPosition

    [SerializeField] private float tweenTime;

    
    private RectTransform _rectTransform;

    private bool _isOpen;

    
    public void Init()
    {
        _rectTransform = GetComponent<RectTransform>();
        toggleButton.onClick.AddListener(ToggleInfo);
    }

    /// <summary>
    /// 버튼 클릭 시 호출되며, 퀘스트 패널을 열거나 닫습니다.
    /// DOTween을 사용해 부드럽게 이동 처리합니다.
    /// </summary>
    private void ToggleInfo()
    {
        Vector2 targetPos = _isOpen ? closedPos : openedPos;
        
        _isOpen = !_isOpen;
        
        _rectTransform.DOAnchorPos(targetPos, tweenTime).SetEase(Ease.OutQuad);
    }
}
