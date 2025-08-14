using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(StatusHandler))]
[RequireComponent(typeof(StatusEffectHandler))]
public abstract class CharacterBaseController : MonoBehaviour, IDamageable
{
    public bool CanDamageable { get; set; }

    public event UnityAction<DamageInfo> OnTakeDamage;
    public event UnityAction OnDeath;
    
    public Vector2 LookDir { get; protected set; }
    public Vector2 MoveDir { get; protected set; }
    public Animator Anim { get; private set; }
    public Rigidbody2D Rigid { get; private set; }
    public StatHandler StatHandler { get; protected set; }
    public StatusHandler StatusHandler { get; private set; }
    public StatusEffectHandler StatusEffectHandler { get; private set; }

    
    public bool IsLockedMoveBack => _isLockedMoveBack;
    protected CharacterStateMachine StateMachine { get;  set; }

    
    [SerializeField] private SpriteRenderer modelSprite;
    
    protected bool isEnemy; 
    
    private bool _isLockedMoveBack;
    
    private DamagePopupUI _damagePopupUI;

    
    protected virtual void Awake()
    {
        Anim = GetComponentInChildren<Animator>();
        Rigid = GetComponent<Rigidbody2D>();
        StatHandler = GetComponent<StatHandler>();
        StatusHandler = GetComponent<StatusHandler>();
        StatusEffectHandler = GetComponent<StatusEffectHandler>();
        
        StateMachine = new(this);
    }

    protected virtual void Start()
    {
        _damagePopupUI  = UIManager.Instance.GetUI<DamagePopupUI>();
    }

    protected virtual void FixedUpdate()
    {
        StateMachine?.FixedUpdate();
    }

    
    protected virtual void Update()
    {
        StateMachine?.Update();
    }

    
    protected virtual void OnDestroy()
    {
    }


    public void AnimationEvent(string eventName)
    {
        StateMachine.AnimationEvent(eventName);
    }


    public virtual void TakeDamage(DamageInfo damageInfo)
    {
        //return;
        
        StatusHandler.ModifyStatus(StatType.Health,damageInfo.Damage * - 1f);
        
        OnTakeDamage?.Invoke(damageInfo);
        
        var health = StatusHandler.GetStatus(StatType.Health);

        if (health != null)
        {
            SetHitColor();
            
            _damagePopupUI.UseDamagePopup(this, (int)damageInfo.Damage, isEnemy,damageInfo.IsCrit);
            
            if (health.CurValue <= 0)
            {
                OnDeath?.Invoke();
            }
        }
    }

    public void ToggleLockMoveBack(bool locked) => _isLockedMoveBack = locked;

    public virtual void MoveBack(CharacterBaseController otherCharacter, UnityAction onComplete = null)
    {
        Vector2 dir =  (Vector2)transform.position - (Vector2)otherCharacter.transform.position;
        Vector2 pos = Rigid.position + dir.normalized * 6.5f * Time.deltaTime;

        Rigid.DOMove(pos, 0.1f).SetEase(Ease.OutCubic).OnComplete(() => onComplete?.Invoke());
    }

    private void SetHitColor()
    {
        modelSprite.material.SetColor(EnemyMaterialKey.ColorKey, EnemyConstant.DefaultColor);
        modelSprite.material.DOColor(EnemyConstant.HitColor, EnemyMaterialKey.ColorKey, EnemyConstant.hitEffectColorDuration).OnKill(()=>modelSprite.material.SetColor(EnemyMaterialKey.ColorKey, EnemyConstant.DefaultColor));
    }
}
