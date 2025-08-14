using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerWeaponHandler : MonoBehaviour
{
    public BaseWeapon UseWeapon => _useWeapon;

    [SerializeField] private LayerMask attackLayerMask;
    [SerializeField] private Transform weaponRoot;
    [SerializeField] private WeaponHitVFX hitVFXPrefab;

    private PlayerController _player;
    private PlayerEquipmentHandler _equipmentHandler;

    private BaseWeapon _useWeapon;

    private ObjectPool<WeaponHitVFX> _hitVFXPool;    
    private Vector2 _hitPos;
    private Vector2 _hitSize;
    private float _hitAngle;
    
    public void Init(PlayerController player)
    {
        _player = player;
        
        _equipmentHandler = _player.EquipmentHandler;

        SetUseWeapon(player.PlayerInstance.equippedItems[ItemType.Equipment]);
        
        _equipmentHandler.OnItemEquipped += SetUseWeapon;
        
        _hitVFXPool = ObjectPoolManager.Instance.CreatePool(hitVFXPrefab);
    }

    private void OnDestroy()
    {
        _equipmentHandler.OnItemEquipped -= SetUseWeapon;
    }


    public void HandleWeapon()
    {
        if (UseWeapon == null) return;

        var handleHand = UseWeapon.GetHandleHand();
        
        float rotAngle = Mathf.Atan2(_player.LookDir.y, _player.LookDir.x) * Mathf.Rad2Deg;

        handleHand.localRotation = Quaternion.Euler(0f, 0f, rotAngle * _player.LookAxis.x);
        
        Vector3 weaponLocalScale =  handleHand.localScale;
        
        weaponLocalScale.y = _player.LookAxis.x;
        weaponLocalScale.x =  _player.LookAxis.x;
        
        handleHand.localScale = weaponLocalScale;
    }
    
    
    public void Attack(float damageMult, Vector2 hitSize, Vector2 attackDir)
    {
        var attackRange = _player.StatHandler.GetStat(StatType.AttackRange).Value;

        UseWeapon.Trail.localScale = Vector3.one * attackRange;
        
        _hitSize = hitSize * attackRange;
        _hitAngle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        _hitPos = (Vector2)UseWeapon.transform.position + attackDir * (_hitSize.x  * 0.5f);

        // 휘두르는 사운드 재생
        AudioManager.Instance.Play("GreatSwordSwingClip");

        bool isImpulse = false;

        float damage = _player.StatHandler.GetStat(StatType.AttackDamage).Value * damageMult;
        
        float critChance = _player.StatHandler.GetStat(StatType.CriticalChance).Value; 
        
        bool isCritical = Random.Range(0, 100) < critChance;

        if (isCritical)
        {
            damage *= _player.StatHandler.GetStat(StatType.CriticalDamageMult).Value; ;
        }
        
        damage = (float)Math.Round(damage, 2);
        
        foreach (var hit in Physics2D.OverlapBoxAll(_hitPos, _hitSize, _hitAngle, attackLayerMask))
        {
            if (hit.TryGetComponent(out IDamageable damageable))
            {
                if(!damageable.CanDamageable) continue;

                Vector3 hitDir = hit.transform.position - _player.transform.position;

                var damageInfo = new DamageInfo(_player.gameObject, damage, hit.transform.position, hitDir, 0, isCritical);
                damageable.TakeDamage(damageInfo);
                
                var hitVFX = _hitVFXPool.Spawn();
                hitVFX.Init(_hitVFXPool, hit.transform.position, hitDir, isCritical);
                
                if (!isImpulse)
                {
                    CameraManager.Instance.PlayerOnAttackHitImpulse();

                    isImpulse = true;
                }
            }
        }
    }

  
    
    private void OnDrawGizmos()
    {
        Quaternion rotation = Quaternion.Euler(0f, 0f, _hitAngle);
        Matrix4x4 matrix = Matrix4x4.TRS(_hitPos, rotation, Vector3.one);
        Gizmos.matrix = matrix;

        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawWireCube(Vector3.zero, _hitSize);

        Gizmos.matrix = Matrix4x4.identity;
    }

    private void SetUseWeapon(ItemInstance itemInstance)
    {
        if (itemInstance.Data.itemType == ItemType.Equipment)
        {
            var weaponPrefabItem = Instantiate(itemInstance.Data.prefab, weaponRoot);

            if (weaponPrefabItem.TryGetComponent(out BaseWeapon weapon))
            {
                if (_useWeapon != null)
                {
                    Destroy(_useWeapon.gameObject);
                }
                
                _useWeapon = weapon;
            }
        }
    }
}
