using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrapController : MonoBehaviour
{
    public TrapDataSO Data => data;
    public bool IsActive => _isActive;

    [SerializeField] private TrapDataSO data;
    [SerializeField] private Animator animator;           
    [SerializeField] private string animatorTrigger = "Activated";
    [SerializeField] private AudioSource sfx;             
    [SerializeField] private AudioClip activateClip;    
    [SerializeField] private Collider2D hitCollider;      
    
    private bool _isActive;
    private bool _isOnGlobalCooldown;
    private readonly Dictionary<int, float> _nextHitTimeByTarget = new(); 

    private void Awake()
    {
        if (hitCollider == null) hitCollider = GetComponent<Collider2D>();
        if (hitCollider != null) hitCollider.isTrigger = true;
        _isActive = false;
    }

    private void OnEnable()
    {
        if (data != null && data.EnableOnRoomEnter)
            Activate();
    }

    public void Activate()
    {
        if (_isActive) return;
        StartCoroutine(CoActivateSequence());
    }

    public void Deactivate()
    {
        _isActive = false;
        if (animator != null) animator.ResetTrigger(animatorTrigger);
        //hitCollider.enabled = false;
    }

    private IEnumerator CoActivateSequence()
    {
        _isActive = true;

        if (data.ActivationDelay > 0f)
            yield return new WaitForSeconds(data.ActivationDelay);

        if (animator != null) animator.SetTrigger(animatorTrigger);
        if (sfx != null && activateClip != null) sfx.PlayOneShot(activateClip);

        if (data.Mode == TrapDataSO.TriggerMode.Timer)
        {
            float endTime = Time.time + data.ActiveDuration;
            while (Time.time < endTime && _isActive)
                yield return null;

            yield return StartCoroutine(CoCooldown());
            if (data.OneShot) Deactivate();
        }
        else
        {
           
        }
    }

    private IEnumerator CoCooldown()
    {
        _isOnGlobalCooldown = true;
        yield return new WaitForSeconds(data.Cooldown);
        _isOnGlobalCooldown = false;
    }

    private void TryDamage(Collider2D other)
    {
        if (!_isActive) return;
        if (_isOnGlobalCooldown && data.Mode == TrapDataSO.TriggerMode.Timer) return;
        if (((1 << other.gameObject.layer) & data.TargetMask) == 0) return;

        if (other.TryGetComponent(out IDamageable damageable) && damageable.CanDamageable)
        {
            int id = other.GetInstanceID();
            float now = Time.time;

            // OnStayTick: per-target tickInterval 적용
            if (data.Mode == TrapDataSO.TriggerMode.OnStayTick)
            {
                if (_nextHitTimeByTarget.TryGetValue(id, out float next) && now < next) return;
                _nextHitTimeByTarget[id] = now + data.TickInterval;
            }
            
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 dir = (other.transform.position - transform.position).normalized;

            var info = new DamageInfo(gameObject, data.Damage, hitPoint, dir, data.KnockbackForce);
            damageable.TakeDamage(info);

            if (data.Mode == TrapDataSO.TriggerMode.OnEnter || data.Mode == TrapDataSO.TriggerMode.Timer)
                StartCoroutine(CoCooldown());

            if (data.OneShot) Deactivate();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (data == null) return;
        if (data.Mode == TrapDataSO.TriggerMode.OnEnter)
            TryDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (data == null) return;
        if (data.Mode == TrapDataSO.TriggerMode.OnStayTick || data.Mode == TrapDataSO.TriggerMode.Timer)
            TryDamage(other);
    }
    
    public void ForceResetTickForAllTargets()
    {
        _nextHitTimeByTarget.Clear();
    }
}