using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    private List<IInteractable> _interactableList = new();
    private IInteractable _Interactable;

    private PlayerController _player;
    private TriggerHandler _interactTriggerHandler;

    public List<Collider2D> test;
    
    
    public void Init(PlayerController player, TriggerHandler interactTriggerHandler)
    {
        _player = player;

        _interactTriggerHandler = interactTriggerHandler;
        _interactTriggerHandler.OnEnter += OnEnter;
        _interactTriggerHandler.OnExit += OnExit;
    }

    private void OnDestroy()
    {
        _interactTriggerHandler.OnEnter -= OnEnter;
        _interactTriggerHandler.OnExit -= OnExit;
    }

    private void Update()
    {
        IInteractable newBestInteractable = FindClosetInteractable();

        if (newBestInteractable != _Interactable)
        {
            _Interactable?.OnExit();
            
            _Interactable = newBestInteractable;

            newBestInteractable?.OnEnter();
        }
    }
    
    public void Interact()
    {
        if (_Interactable != null && _Interactable.CanInteract)
        {
            _Interactable.Interact(_player);
        }
    }
    
    private void OnEnter(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            if (!_interactableList.Contains(interactable))
            {
                _interactableList.Add(interactable);
                test.Add(other);
            }
        }
    }
    


    private void OnExit(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            // 후보 리스트에 있으면 제거
            if (_interactableList.Contains(interactable))
            {
                _interactableList.Remove(interactable);
                test.Remove(other);
            }
        }
    }
    
  
    private IInteractable FindClosetInteractable()
    {
        if (_interactableList.Count == 0)
        {
            return null;
        }

        return _interactableList
            .OrderBy(interactable => 
                Vector2.Distance(_player.transform.position, ((Component)interactable).transform.position))
            .FirstOrDefault();
    }
}
