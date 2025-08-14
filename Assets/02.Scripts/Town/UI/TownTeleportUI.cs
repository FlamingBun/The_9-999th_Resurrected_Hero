using UnityEngine;
using UnityEngine.UI;

public class TownTeleportUI : BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.Popup;
    public override bool IsEnabled => gameObject.activeSelf;
    
    
    private TownManager _townManager;
    private PlayerController _player;
    
    public void Init(TownManager townManager)
    {
        _townManager = townManager;
        _player = townManager.Player;
    }
    
    
    public override void Enable()
    {
        gameObject.SetActive(true);
    }

    public override void Disable()
    {
        gameObject.SetActive(false);
    }

    private void Teleport(Transform destination)
    {
        _player.transform.position = destination.position;
    }
}
