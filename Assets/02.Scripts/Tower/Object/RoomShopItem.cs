using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider2D))]
public class RoomShopItem : MonoBehaviour, IInteractable
{
    public ItemInstance ItemInstance => _itemInstance;
    public bool CanInteract => _canInteract;

    [SerializeField] private SpriteRenderer itemIconRenderer;
    [SerializeField] private TextMeshProUGUI itemPriceText;

    private ItemInstance _itemInstance;
    private RoomShopEventHandler _eventHandler;

    private bool _canInteract = false;

    public void Interact(PlayerController player)
    {
        if (_eventHandler.TryBuyItem(player))
        {
            Destroy(gameObject);
        }
    }

    public void OnEnter()
    {
        _canInteract = true;
        _eventHandler.OnItemTriggerEntered(this);
    }

    public void OnExit()
    {
        _canInteract = false;
        _eventHandler.OnItemTriggerExited();
    }

    public void Init(RoomShopEventHandler eventHandler, ItemInstance itemInstance)
    {
        _eventHandler = eventHandler;
        
        _itemInstance = itemInstance;
        
        itemPriceText.text = itemInstance.price.ToString();
        StartFloatingAnimation();
    }

    private void StartFloatingAnimation()
    {
        float moveDistance = 0.25f;  // 위아래 이동 거리
        float duration = 1f;         // 한 방향 이동 시간

        Transform spriteTransform = itemIconRenderer.transform;
        Vector3 startPos = spriteTransform.localPosition;

        // Y축으로 위아래 무한 반복 (sprite 오브젝트만)
        spriteTransform.DOLocalMoveY(startPos.y + moveDistance, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
