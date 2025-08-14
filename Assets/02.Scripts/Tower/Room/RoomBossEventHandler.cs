using UnityEngine;

public class RoomBossEventHandler : MonoBehaviour
{
    public int Order => 0;
    public bool IsCleared { get; set; }

    private LetterBoxUI _letterBoxUI;
    private PlayerController _player;
    
    private void Start()
    {
        IsCleared = false;

        var uiManager = UIManager.Instance;

        _letterBoxUI = uiManager.GetUI<LetterBoxUI>();
    }
    
}
