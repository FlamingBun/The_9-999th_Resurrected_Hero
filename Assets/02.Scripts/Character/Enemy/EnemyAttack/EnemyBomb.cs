using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBomb : EnemyThrowableObject
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private Animator _animator;

    private SpriteRenderer _spriteRenderer;
    
    private CapsuleCollider2D _capsuleCollider;

    private EnemyRangeAttackDataSO _rangeAttackData;

    public GameObject indicatorObj; 
    
    private bool _isHit;

    private string _smokeName;
    
    protected void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
    }
    

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isHit) return;
        
        if (layerMask.value == (layerMask.value | (1 << collision.gameObject.layer)))
        {
            if (collision.TryGetComponent(out PlayerController player))
            {
                if (!player.CanDamageable) return;
                    
                
                _isHit = true;
                
                Vector3 hitDir = collision.transform.position - transform.position;
                        
                Vector3 hitPoint = collision.ClosestPoint(transform.position);
                
                DamageInfo damageInfo = new DamageInfo(controller?.gameObject, damage, hitPoint, hitDir, 0);
                
                player.TakeDamage(damageInfo);
                
                if (hasKnockback)
                {
                    Vector2 moveDirection = player.transform.position - transform.position;
                    
                    controller?.MoveHandler.MakeMove(player.Rigid, moveDirection, knockbackPower);
                }   
                
                SpawnImpact();
                _capsuleCollider.enabled = false;
            }
        }
    }

    public override void Init(EnemyController enemyController, EnemyBaseAttackDataSO attackData, EnemyProjectileData projectileData, Vector2 direction)
    {
        base.Init(enemyController, attackData, projectileData, direction);
        
        _rangeAttackData = attackData as EnemyRangeAttackDataSO;

        _smokeName = _rangeAttackData.rangeAttackData.smokeEffect.name;
        
        _animator.speed = 0f;
        _capsuleCollider.enabled = false;
        _isHit = false;
    }
    
    private void Explode()
    {
        // 폭발 사운드
        AudioManager.Instance.Play("MonsterBombClip");

        VFXHandler smoke = ObjectPoolManager.Instance.Spawn<VFXHandler>(_smokeName);
                
        smoke.transform.position = transform.position;
                
        smoke.Init(_smokeName);
        
        _capsuleCollider.enabled = true;

        if (indicatorObj != null)
        {
            indicatorObj.SetActive(false);
        }
    }
    
    public void ParabolaMove(Transform bomb, Vector3 startPosition, Vector3 targetPosition, float height, float duration)
    {
        bool isExploded = false;
        
        DOVirtual.Float(0, 1, duration, t =>
        {
            float easedT =  easeCurve.Evaluate(t);

            Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, easedT);
            
            float arc = 4 * height * easedT * (1 - easedT);
            
            currentPos.y += arc;

            bomb.position = currentPos;

            if (!isExploded && easedT > 0.9f)
            {
                isExploded = true;
                _animator.speed = 1f; 
            }
        });
    }

    public void AnimationExplodeEvent()
    {
        Explode();
    }
    
    public void AnimationExplodeEndEvent()
    {
        _capsuleCollider.enabled = false;
        ObjectPoolManager.Instance.Return(data.prefab.name, this);
    }

    public override void OnDespawn()
    {
        _spriteRenderer.sprite = defaultSprite;
    }
}
