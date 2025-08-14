using System;
using System.Collections.Generic;
using UnityEngine;


public class TownTransporter : MonoBehaviour, IInteractable
{
    public bool CanInteract { get; set; }

    private InteractGuideUI _interactGuideUI;

    private void Start()
    {
        _interactGuideUI = UIManager.Instance.GetUI<InteractGuideUI>();
        CanInteract = true;

    }

    public void Interact(PlayerController player)
    {
        AudioManager.Instance.Play(key: "EnterTowerClip");

        List<string> loadScenes = new()
        {
            Constant.TowerSceneName,
            Constant.FloorSceneName
        };
        
        List<string> unloadScenes = new()
        {
            Constant.TownSceneName
        };
        
        SceneLoadManager.Instance.LoadAddtiveScenes(loadScenes, unloadScenes);

        CanInteract = false;
    }

    public void OnEnter()
    {
        _interactGuideUI.InitTarget(transform);
        _interactGuideUI.Enable();
    }

    public void OnExit()
    {
        _interactGuideUI.Disable();
    }
}
