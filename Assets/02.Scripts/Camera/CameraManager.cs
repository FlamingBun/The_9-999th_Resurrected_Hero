using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance => _instance;
    
    private static CameraManager _instance;

    public Camera MainCamera => _mainCamera;

    
    [SerializeField] private CinemachineVirtualCamera followCamera;
    [SerializeField] private CinemachineVirtualCamera focusVirtualCamera;
    [SerializeField] private CinemachineImpulseSource takeDamageImpulseSource;
    [SerializeField] private CinemachineImpulseSource attackHitImpulseSource;

    
    private Camera _mainCamera;
    private CinemachineConfiner2D _followCameraConfiner;

    private PlayerController _player;

    
    protected void Awake()
    {
        _instance = this;
        
        _mainCamera = Camera.main;
        
        _followCameraConfiner = followCamera.GetComponent<CinemachineConfiner2D>();
    }
    

  
    public void EnableFocus(Transform target)
    {
        focusVirtualCamera.Follow = target;
        focusVirtualCamera.Priority = 10;
    }

    public void DisableFocus()
    {
        focusVirtualCamera.Priority = 0;
    }
    

    public void PlayTakeDamageImpulse(StatusEventData eventData)
    {
        if (eventData.StatType == StatType.Health)
        {
            if (eventData.CurValue < eventData.PreValue)
            {
                takeDamageImpulseSource.GenerateImpulse();
            }
        }
    }

    public void PlayerOnAttackHitImpulse()
    {
        attackHitImpulseSource.GenerateImpulse();
    }

    public void SetConfiner(Collider2D confinerCollider)
    {
        _followCameraConfiner.m_BoundingShape2D = confinerCollider;
    }

    public void SetTargetToFollowCamera(Transform target)
    {
        followCamera.Follow = target;
    }

    public void Teleport(Transform target, Vector2 posDelta)
    {
        followCamera.OnTargetObjectWarped(target, posDelta);
        followCamera.PreviousStateIsValid = false;
    }
}
