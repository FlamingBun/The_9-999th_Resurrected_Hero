using System;
using UnityEngine;

public class InitializeImpact:MonoBehaviour
{
    public static readonly int start = Animator.StringToHash("Start");
    
    private Action _callback;
    
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Init(Action callback = null)
    {
        _animator.SetTrigger(start);
    }

    public void OnEnd()
    {
        gameObject.SetActive(false);
    }
}
