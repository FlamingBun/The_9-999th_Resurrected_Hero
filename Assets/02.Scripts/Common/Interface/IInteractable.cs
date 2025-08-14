using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public bool CanInteract { get; }
    public void Interact(PlayerController player);
    public void OnEnter();
    public void OnExit();
}
