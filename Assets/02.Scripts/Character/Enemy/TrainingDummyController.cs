using System.Collections.Generic;
using UnityEngine;

public class TrainingDummyController : EnemyController
{
    [SerializeField]private ParticleHandler _hitEffect;
    
    private float regenRate = 60f;
    private float maxHealth = 100000000;

    private Status _health;

    
    protected override void Awake()
    {
        base.Awake();
        ToggleLockMoveBack(true);
    }

    protected override void Start()
    {
        base.Start();
        
        ObjectPoolManager.Instance.CreatePool(_hitEffect);
        
        List<StatData> statDatas = new()
        {
            new()
            {
                statType = StatType.Health,
                value = maxHealth,
            }
        };
        
        StatHandler.Init(statDatas);
        StatusHandler.Init(StatHandler);
        
        _health = StatusHandler.GetStatus(StatType.Health);
        
        StatusHandler.OnStatusChanged += UpdateHealthBar;
        StatusHandler.OnStatusChanged += SetQuestCondition;
        StatusHandler.OnStatusChanged += SetHitAnim;
        
        EndInitializeEffect();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        StatusHandler.OnStatusChanged -= UpdateHealthBar;
        StatusHandler.OnStatusChanged -= SetQuestCondition;
        StatusHandler.OnStatusChanged -= SetHitAnim;
    }


    protected override void Update()
    {
        RegenerateHealth();
    }

    protected override void PlayHitEffect(DamageInfo damageInfo)
    {
        ParticleHandler hitEffect = ObjectPoolManager.Instance.Spawn<ParticleHandler>(_hitEffect.name);
        
        hitEffect.transform.position = damageInfo.HitPoint;

        float angle = Mathf.Atan2(damageInfo.HitDirection.y, damageInfo.HitDirection.x) * Mathf.Rad2Deg;

        hitEffect.transform.localRotation = Quaternion.Euler(0, 0, angle);
        
        hitEffect.Init(_hitEffect.name);

        AudioManager.Instance.Play("DummyHitClip");
    }

    protected override void SetStateMachine() { }

    public override void AnimationEvent(string key)
    {
        base.AnimationEvent(key);

        if (key == "Finish")
        {
            Anim.SetBool("isHit", false);
        }
    }
    

    private void RegenerateHealth()
    {
        if (_health.CurValue < maxHealth)
        {
            StatusHandler.ModifyStatus(StatType.Health, regenRate * Time.deltaTime);
        }
    }

    private void SetHitAnim(StatusEventData eventData)
    {
        if (eventData.StatType == StatType.Health)
        {
            if (eventData.CurValue < eventData.PreValue)
            {
                Anim.SetBool("isHit", true);
            }
        }
    }
    
    private void SetQuestCondition(StatusEventData eventData)
    {
        if (eventData.StatType == StatType.Health)
        {
            if (eventData.CurValue < eventData.PreValue)
            {
                QuestManager questManager = FindObjectOfType<QuestManager>();
                questManager?.IncrementQuestProgress("tutorial_003");
            }
        }
    }

    private void UpdateHealthBar(StatusEventData eventData)
    {
        if (eventData.StatType == StatType.Health)
        {
            healthBar.SetHealthBarValue(eventData.CurValue/ eventData.MaxValue);
        }
    }
}
