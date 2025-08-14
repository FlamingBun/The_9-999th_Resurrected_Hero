using System.Collections;
using UnityEngine;

public class JumpAttackCollider : MonoBehaviour
{
    [SerializeField] private LayerMask collisionLayerMask;

    private EnemyAreaAttackDataSO _attackData;
    private EnemyController _controller;
    
    private PolygonCollider2D _collider;
    
    private bool _isHit;

    private void Awake()
    {
        _collider = GetComponent<PolygonCollider2D>();
        _collider.enabled = false;
    }

    public void Init(EnemyAreaAttackDataSO attackData, EnemyController controller)
    {
        _attackData = attackData;
        _controller = controller;
        
        gameObject.SetActive(false);
    }

    public void Spawn(Vector3 position)
    {
        _isHit = false;
        
        gameObject.transform.position = position;
        
        gameObject.SetActive(true);

        _collider.enabled = true;
        
        StartCoroutine(DelayDeactivate(0.1f));
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

    private IEnumerator DelayDeactivate(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
