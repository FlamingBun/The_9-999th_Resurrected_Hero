using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenIndicatorUI : BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.HUD;
    public override bool IsEnabled => gameObject.activeSelf;
    
    
    [SerializeField] private RectTransform indicatorObject;

    
    private Camera mainCamera;
    private RectTransform _canvasRectTransform;
    
    private List<(Transform, RectTransform)> _targetIndicators = new();

    private Vector2 _margin = new Vector2(25, 25);
    
    
    void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        
        _canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }
    
    void LateUpdate()
    {
        for (int i = _targetIndicators.Count - 1; i >= 0; i--)
        {
            if (_targetIndicators[i].Item1 == null)
            {
                Destroy(_targetIndicators[i].Item2.gameObject);
                _targetIndicators.RemoveAt(i);
            }
            else
            {
                UpdateIndicatorForTarget(_targetIndicators[i]);
            }
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

    private void UpdateIndicatorForTarget((Transform, RectTransform) item)
    {
        Transform targetPoint = item.Item1;
        RectTransform indicator = item.Item2;
        
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(targetPoint.position);

        bool isOnScreen = screenPoint.x < Screen.width - _margin.x &&
                          screenPoint.x > 0 + _margin.x &&
                          screenPoint.y < Screen.height - _margin.y &&
                          screenPoint.y > 0 + _margin.y;


        if (isOnScreen)
        {
            indicator.gameObject.SetActive(false);
            return;
        }

        indicator.gameObject.SetActive(true);

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Vector3 targetDirection = screenPoint - screenCenter;

        if (screenPoint.z < 0) // 뒤에 있을 경우 방향 반전
            targetDirection *= -1;

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x);
        float slope = Mathf.Tan(angle);

        float clampedX, clampedY;
        
        float halfWidth  = (Screen.width  / 2f) - _margin.x;
        float halfHeight = (Screen.height / 2f) - _margin.y;

        if (Mathf.Abs(slope) < (halfHeight / halfWidth))
        {
            clampedX = Mathf.Sign(targetDirection.x) * halfWidth;
            clampedY = clampedX * slope;
        }
        else
        {
            clampedY = Mathf.Sign(targetDirection.y) * halfHeight;
            clampedX = clampedY / slope;
        }
        
        /*if (Mathf.Abs(slope) < (Screen.height / 2f) / (Screen.width / 2f))
        {
            clampedX = Mathf.Sign(targetDirection.x) * Screen.width / 2f;
            clampedY = clampedX * slope;
        }
        else
        {
            clampedY = Mathf.Sign(targetDirection.y) * Screen.height / 2f;
            clampedX = clampedY / slope;
        }*/


        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRectTransform,
            new Vector2(clampedX + Screen.width / 2f, clampedY + Screen.height / 2f),
            null,
            out localPos
        );

        indicator.anchoredPosition = localPos;
        indicator.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
    }

    public void AddTarget(Transform target)
    {
        foreach (var value in _targetIndicators)
        {
            if (value.Item1 == target)
            {
                return;
            }
        }

        var indicator = Instantiate(indicatorObject, transform);
        
        _targetIndicators.Add((target, indicator));
    }

    public void RemoveTarget(Transform target)
    {
        int targetIndex = -1;

        for (int i = 0; i < _targetIndicators.Count; i++)
        {
            if (_targetIndicators[i].Item1 == target)
            {
                targetIndex = i;
                break;
            }
        }
        
        if (targetIndex > -1)
        {
            Destroy(_targetIndicators[targetIndex].Item2.gameObject);
            
            _targetIndicators.RemoveAt(targetIndex);
        }
    }
}
