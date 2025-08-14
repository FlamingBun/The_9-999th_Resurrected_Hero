using System;
using DG.Tweening;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;


public class EnemyMoveHandler:MonoBehaviour
{
    public AILerp AILerp { get =>_aiLerp; }

    private EnemyController _controller;
    
    private Transform _playerTransform;  
    private Transform _tempTransform;
    
    private AILerp _aiLerp;
    private AIDestinationSetter _aiDestinationSetter;

    private StatHandler _statHandler;
    
    private float _evadeRange;
    private float _evadeSpeed;

    private bool _isFollowTarget;
    
    public event Action<bool> OnFollowTargetChange;
    
    private void Awake()
    {
        _aiLerp = GetComponent<AILerp>();
        _aiDestinationSetter = GetComponent<AIDestinationSetter>();

    }
    
    private void OnDisable()
    {
        if (_tempTransform != null &&_tempTransform.gameObject != null)
        {
            Destroy(_tempTransform.gameObject);
        }
    }

    public void Init(EnemyController controller, Transform playerTransform)
    {
        _controller = controller;
        _statHandler = _controller.StatHandler;
        
        _evadeRange = _statHandler.GetStat(StatType.EvadeRange).Value;
        _evadeSpeed = _statHandler.GetStat(StatType.EvadeSpeed).Value;
        
        _tempTransform = new GameObject("TempTarget").transform;
        _tempTransform.position = transform.position;
        _tempTransform.parent = transform.parent;

        _playerTransform = playerTransform;
    }
    public void FollowPlayer()
    {
        _aiDestinationSetter.target = _playerTransform;
        TargetChange(true);
        StartRun();
    }
    

    public void Evade()
    {
        _tempTransform.position = GetEvadePosition(out bool isSuccess);

        if (!isSuccess)
        {
            _controller.EnemyStateMachine.ChangeAttackState();
            return;
        }
        
        _aiDestinationSetter.target = _tempTransform;
        TargetChange(false);
        StartEvade();
    }

    public void StopMove()
    {
        _aiDestinationSetter.target = null;
        _aiLerp.speed = 0f;
    }
    
    private void StartRun()
    {
        _aiLerp.speed = _statHandler.GetStat(StatType.MoveSpeed).Value;
        _aiLerp.canMove = true;
    }

    private void StartEvade()
    {
        _aiLerp.speed = _evadeSpeed;
        _aiLerp.canMove = true;
    }
    
    public Vector2 GetRandomFromTarget(Transform target, float distance, Bounds bounds) 
    {
        for (int i = 0; i < EnemyConstant.randomPositionMaxCount; i++)
        {
            float angle = Random.Range(0f, 2f * Mathf.PI);

            
            float x = target.position.x + Mathf.Cos(angle) * distance;
            float y = target.position.y + Mathf.Sin(angle) * distance;
            Vector2 randomPosition = new Vector2(x, y);

            if (!bounds.Contains(randomPosition))
                continue;
            
            Collider2D[] results = new Collider2D[1];
            if (Physics2D.OverlapBoxNonAlloc(randomPosition, new Vector2(1, 1), 0f, results, LayerMask.GetMask("Obstacle")) == 0)
            {
                return randomPosition;
            }
        }
        return target.position + new Vector3(distance, 0f, 0f);
    }
    
    
    public Vector2 GetDirection()
    {
        if (_isFollowTarget)
        {
            return (_playerTransform.position - transform.position).normalized;
        }
        else
        {
            return (_tempTransform.position - transform.position).normalized;   
        }
    }

    public float GetDistanceFromPlayer()
    {
        return Vector3.Distance(transform.position, _playerTransform.position);
    }

    public void MakeMove(Rigidbody2D target, Vector2 direction, float distance = 0.2f, float duration = 0.1f, Action onEnd = null)
    {
        if (DOTween.IsTweening(target.GetInstanceID())) return;
        
        if (target.TryGetComponent(out AILerp path))
        {
            path.enabled = false;
            target.DOMove(target.position + (direction * distance), duration).OnComplete(() =>
            {
                path.enabled = true;
                onEnd?.Invoke();
            }).SetId(target.GetInstanceID());
        }
        else
        {
            target.DOMove(target.position + (direction * distance), duration).OnComplete(()=> onEnd?.Invoke()).SetId(target.GetInstanceID());
        }
    }
    

    private Vector2 GetEvadePosition(out bool isSuccess)
    {
        for (int i = 0; i < EnemyConstant.randomPositionMaxCount; i++)
        {
            Vector2 directionFromTarget = ((Vector2)(transform.position - _playerTransform.position)).normalized;

            float angle = Random.Range(-60f, 60f);
            Vector2 directionToEvade = Quaternion.Euler(0, 0, angle) * directionFromTarget;

            Vector2 evadePosition = (Vector2)transform.position + (directionToEvade * _evadeRange);

            if (Physics2D.Raycast(transform.position, directionToEvade, _evadeRange * 1.5f, LayerMask.GetMask("Obstacle")))
            {
                continue;
            }

            Collider2D[] results = new Collider2D[1];
        
            if (Physics2D.OverlapBoxNonAlloc(evadePosition, Vector2.one, 0f,results ,LayerMask.GetMask("Obstacle")) == 0)
            {
                isSuccess = true;
                return evadePosition;
            }    
        }
        
        isSuccess = false;
        return transform.position;
    }

    public void TargetChange(bool isFollowTarget)
    {
        _isFollowTarget = isFollowTarget;
        OnFollowTargetChange?.Invoke(_isFollowTarget);
    }
}
