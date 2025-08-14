using System.Collections;
using UnityEngine;

public class Stone : MonoBehaviour
{
    [SerializeField] private LayerMask collisionLayerMask;

    private WaitForSeconds _colliderLifeTime;
    
    private EnemyAreaAttackDataSO _attackData;
    private EnemyController _controller;

    private BoxCollider2D _collider;
    private Animator _animator;
    
    private bool _isHit;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        
        _colliderLifeTime = new WaitForSeconds(0.1f);
    }

    public void Init(EnemyAreaAttackDataSO attackData, EnemyController controller)
    {
        _attackData = attackData;
        _controller = controller;
        
        _collider.enabled = false;
        gameObject.SetActive(false);
    }

    public void Spawn(Vector3 position)
    {
        _isHit = false;

        gameObject.transform.position = position;
        
        _collider.enabled = false;

        gameObject.SetActive(true);
        
        _animator.SetBool(EnemyAnimationHashes.SpawnStone, true);
    }

    public void AnimationSpawnStoneEvent()
    {
        _collider.enabled = true;
        
        StartCoroutine(TurnOffAfterDuration(_attackData.stompAttackData.stoneLifeTime));
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isHit) return;
        
        if (((1 << collision.gameObject.layer) & collisionLayerMask) == 0)
            return;
        
        if (collision.TryGetComponent(out PlayerController player))
        {
            if (!player.CanDamageable) return;
            
            
            _isHit = true;
            
            Vector3 hitDir = collision.transform.position - transform.position;
            
            Vector3 hitPoint = collision.ClosestPoint(gameObject.transform.position);
            
            DamageInfo damageInfo = new DamageInfo(_controller.gameObject, _controller.GetFinalDamage(_attackData.damageMultiplier), hitPoint, hitDir, 0);
                        
            player.TakeDamage(damageInfo);
            
            Vector2 direction = (player.transform.position - transform.position).normalized;
            
            _controller.MoveHandler.MakeMove(player.Rigid, direction, _attackData.knockbackPower);
        }
    }

    private IEnumerator TurnOffAfterDuration(float duration)
    {
        yield return _colliderLifeTime;
        _collider.enabled = false;
        
        yield return new WaitForSeconds(duration);
        _animator.SetBool(EnemyAnimationHashes.SpawnStone, false);
        gameObject.SetActive(false);
    }
}
