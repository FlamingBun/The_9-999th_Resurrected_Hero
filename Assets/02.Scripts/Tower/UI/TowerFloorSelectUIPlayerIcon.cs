using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TowerFloorSelectUIPlayerIcon : MonoBehaviour
{

    [SerializeField] private Image playerIconRenderer;
    [SerializeField] private float floatingDist;
    [SerializeField] private float floatingDuration;
    [SerializeField] private float moveDuration;


    private TowerManager _towerManager;

    private void OnEnable()
    {
        StartFloatingAnimation();
    }

    public void Init(TowerManager towerManager)
    {
        _towerManager = towerManager;
    }

    public void MoveTo(TowerFloorSelectUIFloorSlot slot)
    {
        transform.DOMove(slot.transform.position, moveDuration)
            .SetEase(Ease.InOutSine);
    }

    private void StartFloatingAnimation()
    {
        RectTransform spriteTransform = playerIconRenderer.rectTransform;
        Vector3 startPos = spriteTransform.anchoredPosition;

        spriteTransform.DOLocalMoveY(startPos.y + floatingDist, floatingDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
   
}
