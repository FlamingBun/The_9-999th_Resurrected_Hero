using System.Collections;
using UnityEngine;

public class EnemyContactDamageCollider : MonoBehaviour
{
    [SerializeField] private LayerMask collisionLayerMask;

    private EnemyController _controller;
    
    private CircleCollider2D _circleCollider;

    private EnemyCollisionAttackDataSO _attackData;

    private WaitForSeconds _hitIntervalWaitForSeconds;
    
    private bool _isHitInterval;
    
    private void Awake()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        _circleCollider.enabled = false;
    }

    public void Init(EnemyController controller ,EnemyCollisionAttackDataSO attackData)
    {
        _attackData = attackData;

        _controller = controller;
        
        _circleCollider.enabled = true;
        _isHitInterval = false;

        _hitIntervalWaitForSeconds = new WaitForSeconds(_attackData.hitIntervalTime);

        _controller.OnDeath += DestroyContactDamageCollider;
    }

    public void OnDestroy()
    {
        if (_controller != null)
        {
            _controller.OnDeath -= DestroyContactDamageCollider;
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_isHitInterval) return;
        
        if (((1 << collision.gameObject.layer) & collisionLayerMask) == 0) return;

        if (collision.TryGetComponent(out PlayerController player))
        {
            if (!player.CanDamageable) return;
            
            Vector3 hitDir = collision.transform.position - _controller.transform.position;
                        
            DamageInfo damageInfo = new DamageInfo(_controller.gameObject, _controller.GetFinalDamage(_attackData.damageMultiplier), collision.transform.position, hitDir, 0);
                        
            player.TakeDamage(damageInfo);
            
            Vector2 direction = _controller.MoveHandler.GetDirection();
            
            _controller.MoveHandler.MakeMove(player.Rigid, direction, _attackData.knockbackPower);
            
            StartCoroutine(HitIntervalRoutine());   
        }
    }

    private IEnumerator HitIntervalRoutine()
    {
        _isHitInterval = true;
        
        yield return _hitIntervalWaitForSeconds;
        
        _isHitInterval = false;
    }

    private void DestroyContactDamageCollider()
    {
        _circleCollider.enabled = false;
        Destroy(this);
    }
}
