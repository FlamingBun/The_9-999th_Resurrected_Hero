using UnityEngine;

public class FloorTransporter : MonoBehaviour, IInteractable
{
    public bool CanInteract { get; set; }

    private TowerManager _towerManager;
    private InteractGuideUI _interactGuideUI;
    
    public void Interact(PlayerController player)
    {
        _towerManager.ClearFloor();
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
    
    
    public void Start()
    {
        _towerManager = FindAnyObjectByType<TowerManager>();

        _interactGuideUI = UIManager.Instance.GetUI<InteractGuideUI>();

        CanInteract = true;
    }
}
