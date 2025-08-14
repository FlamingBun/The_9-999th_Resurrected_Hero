using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;


public abstract class BaseWeapon : MonoBehaviour
{
    public event UnityAction OnStart;
    public event UnityAction OnStartAttack;
    public event UnityAction OnHit;
    public event UnityAction OnFinishAttack;
    public event UnityAction OnFinish;
    public Animator Anim => _animator;
    
    public int CurComboIndex => _curComboIndex;
    public int MaxComboCount => weaponData.comboEntries.Length;
    public Transform Trail => trail;
    
    [SerializeField] protected Transform mainHand;
    [SerializeField] protected Transform mainWeapon;
    [SerializeField] protected Transform subHand;
    [SerializeField] protected Transform subWeapon;
    [SerializeField] private Transform trail;

    [Space(10f)]
    [SerializeField] private WeaponData weaponData;
    

    private SortingGroup _mainSortingGroup;
    private SortingGroup _subSortingGroup;
    private Animator _animator;
    private Coroutine _animCoroutine;
    
    private int _curComboIndex = 0;
    

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _mainSortingGroup = mainWeapon.GetComponent<SortingGroup>();
        _subSortingGroup = subWeapon.GetComponent<SortingGroup>();
    }

    private void Update()
    {
        _mainSortingGroup.sortingOrder = (int)mainWeapon.transform.localPosition.z;
        _subSortingGroup.sortingOrder = (int)subWeapon.transform.localPosition.z;
    }


    public abstract Transform GetHandleHand();

    public void OnEquip()
    {
    }

    public void OnUnequip()
    {
        Destroy(gameObject);
    }

    public void AnimationEvent(string eventKey)
    {
        switch (eventKey)
        {
            case PlayerConstant.StartKey :OnStart?.Invoke(); break;
            case PlayerConstant.StartAttackKey : OnStartAttack?.Invoke(); break;
            case PlayerConstant.HitKey : OnHit?.Invoke(); break;
            case PlayerConstant.FinishAttackKey : OnFinishAttack?.Invoke(); break;
            case PlayerConstant.FinishKey : OnFinish?.Invoke(); break;
        }
    }

    public Vector2 GetComboAttackHitBoxSize()
    {
        if (_curComboIndex < weaponData.comboEntries.Length)
        {
            return weaponData.comboEntries[_curComboIndex].HitBoxSize;
        }

        return Vector2.zero;
    }
    
    public Vector2 GetDodgeAttackHitBoxSize()
    {
        return weaponData.dodgeAttackEntry.HitBoxSize;
    }

    public float GetComboAttackDashDist()
    {
        if (_curComboIndex < weaponData.comboEntries.Length)
        {
            return weaponData.comboEntries[_curComboIndex].DashDist;
        }

        return 0;
    }

    public float GetDodgeAttackDashDist()
    {
        return weaponData.dodgeAttackEntry.DashDist;
    }

    public float GetComboAttackDamageMult()
    {
        if (_curComboIndex < weaponData.comboEntries.Length)
        {
            return weaponData.comboEntries[_curComboIndex].DamageMultiplier;
        }

        return 0;
    }

    public float GetDodgeAttackDamageMult()
    {
        return weaponData.dodgeAttackEntry.DamageMultiplier;
    }


    public void SetComboIndex(int value)
    {
        _curComboIndex = value;
    }
    
    public void AddCombo()
    {
        if (_curComboIndex < weaponData.comboEntries.Length - 1)
        {
            _curComboIndex += 1;
            return;
        }

        _curComboIndex = 0;
    }

}
