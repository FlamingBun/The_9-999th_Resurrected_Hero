using System;
using DG.Tweening;
using UnityEngine;

public class FloorMinimapCamera : MonoBehaviour
{
    [SerializeField] private float followSpeed;
    private Vector2 _clampMin;
    private Vector2 _clampMax;
    private PlayerController _player;

    private Camera _camera;
    private bool _isMoving = false;

    public void Init(PlayerController player)
    {
        _camera = GetComponent<Camera>();
        _player = player;
    }
    

    public void MoveTo(Vector3 position)
    {
        _isMoving = true;
        
        position.z = transform.position.z;

        transform.DOMove(position, 0.33f).SetEase(Ease.OutCubic).OnComplete(() => _isMoving = false);
    }

    public void SetClampBounds(Vector2 min, Vector2 max)
    {
        _clampMin = min;
        _clampMax = max;
    }

    private void LateUpdate()
    {
        if (_player == null ||
            _camera == null || 
            _isMoving) return;
        
        float vertExtent = _camera.orthographicSize;
        float horzExtent = vertExtent * _camera.aspect;
        
        float minX = _clampMin.x + horzExtent;
        float maxX = _clampMax.x - horzExtent;
        float minY = _clampMin.y + vertExtent;
        float maxY = _clampMax.y - vertExtent;

        if (minX > maxX)
        {
            minX = (_clampMin.x + _clampMax.x) / 2;
            maxX = minX;
        }
        if (minY > maxY)
        {
            minY = (_clampMin.y + _clampMax.y) / 2;
            maxY = minY;
        }
   
        Vector3 targetPos = transform.position;

        targetPos.x = Mathf.Clamp(_player.transform.position.x, minX, maxX);
        targetPos.y = Mathf.Clamp(_player.transform.position.y, minY, maxY);

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
    }
}