using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TownItemShopUISlot : MonoBehaviour, IPoolable, IPointerEnterHandler, IPointerExitHandler
{
    public ItemDataSO ItemData => _itemData;
    
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject priceInfoObj;
    [SerializeField] private Button selectButton;

    [SerializeField] private Image selectedBox;   
    [SerializeField] private Image outline;
    
    private ItemDataSO _itemData;
    private TownItemShopHandler _townItemShopHandler;
    private TownItemShopUI _shopUI;
    private PlayerInstance _playerInstance;
    private PlayerEquipmentHandler _equipmentHandler;

    
    private void Start()
    {
        _shopUI = GetComponentInParent<TownItemShopUI>();

        selectButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.Play("ShopItemClickClip"); // 사운드 재생 추가
            _shopUI.OnSlotButton(this);
        });
    }

    public void Init(ItemDataSO itemData)
    {
        _itemData = itemData;
        nameText.text = itemData.itemName;
        priceText.text = itemData.price.ToString();
        
        selectedBox.gameObject.SetActive(false);
        outline.gameObject.SetActive(false);
    }

    public void TogglePriceInfo(bool enable)
    {
        priceInfoObj.SetActive(enable);
    }
    
    public void ToggleSelectedBox(bool enable)
    {
        selectedBox.gameObject.SetActive(enable);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.gameObject.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        outline.gameObject.SetActive(false);
    }


    public void OnSpawn() { }
    public void OnDespawn() { }
}