using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerHandler : MonoBehaviour
{
    public event UnityAction<Collider2D> OnEnter;
    public event UnityAction<Collider2D> OnExit;
    public event UnityAction<Collider2D> OnStay;
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        OnEnter?.Invoke(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        OnStay?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OnExit?.Invoke(other);
    }
}
