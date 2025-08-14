using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TownItemShopUI : BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.Popup;
    public override bool IsEnabled => gameObject.activeSelf;
    

    [SerializeField] private TownItemShopUISlot slotPrefab;
    [SerializeField] private Transform slotsParent;
    
    [SerializeField] private TextMeshProUGUI itemTitleText;
    [SerializeField] private TextMeshProUGUI itemDescText;
    [SerializeField] private TextMeshProUGUI itemStatText;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private Button executeButton;
    [SerializeField] private TextMeshProUGUI executeText;
    
    private TownItemShopHandler _townItemShopHandler;
    private PlayerInstance _playerInstance;
    private PlayerController _player;
    private TownItemShopUISlot _selectedSlot;

    private bool _isEquipable;

    public override void Enable()
    {
        AudioManager.Instance.Play(key: "OpenShopUIClip");

        gameObject.SetActive(true);
        
        executeButton.onClick.AddListener(OnExecuteButton);
        
        _player.InputController.EnableTownItemShopUIInputs();
        _player.InputController.OnClosePopupUI += Disable;
    }

    public override void Disable()
    {
        gameObject.SetActive(false);
        AudioManager.Instance.Play(key: "UiButtonClickClip");

        _player.InputController.EnablePlayerInputs();
        _player.InputController.OnClosePopupUI -= Disable;
    }

    public void InitPlayer(PlayerController player)
    {
        _player = player;
        _playerInstance = player.PlayerInstance;
    }
    
    public void InitShop(TownItemShopHandler townItemShopHandler)
    {
        for (int i = 0; i < slotsParent.childCount; i++)
        {
            Destroy(slotsParent.GetChild(i).gameObject);
        }
        
        _townItemShopHandler = townItemShopHandler;
        
        InitSlots();
    }

    public void OnSlotButton(TownItemShopUISlot slot)
    {
        if (_selectedSlot != null)
        {
            _selectedSlot.ToggleSelectedBox(false);
        }

        _selectedSlot = slot;
        
        _selectedSlot.ToggleSelectedBox(true);
        
        SetItemInfo(slot.ItemData);
    }
    
    public void SetItemInfo(ItemDataSO data)
    {
        itemTitleText.text = data.itemName;
        itemDescText.text = data.description;
        itemIconImage.sprite = data.icon;
        itemIconImage.SetNativeSize();

        itemStatText.text = "";
        AudioManager.Instance.Play(key: "UiButtonClickClip");

        foreach (var mod in data.statModifierDatas)
        {
            itemStatText.text +=
                $"{ConvertModType(mod.statType)} + {mod.value}\n";
        }

        _isEquipable = false;
        
        foreach (var item in _playerInstance.ownedItems)
        {
            if (data == item.Data)
            {
                _isEquipable = true;
                
                if (_player.EquipmentHandler.EquippedItems.ContainsValue(item))
                {
                    executeButton.gameObject.SetActive(false);
                    return;
                }
            }
        }

        executeButton.gameObject.SetActive(true);

        executeText.text = _isEquipable ? "장착 하기" : "구매 하기";
    }

    private string ConvertModType(StatType statType)
    {
        return statType switch
        {
            StatType.AttackDamage => "공격력",
            StatType.AttackRange => "공격 범위",
            StatType.AttackSpeed => "공격 속도",
            StatType.MoveSpeed => "이동 속도",
            _ => ""
        };
    }

    private void OnExecuteButton()
    {
        if (_selectedSlot == null) return;

        var itemData = _selectedSlot.ItemData;
      
        foreach (var item in _playerInstance.ownedItems)
        {
            if (itemData == item.Data)
            {
                AudioManager.Instance.Play(key: "EquipClip1");
                AudioManager.Instance.Play(key: "EquipClip2");

                _player.EquipmentHandler.Equip(item);
                executeButton.gameObject.SetActive(false);
                return;
            }
        }

        if (_townItemShopHandler.TryPurchase(itemData))
        {
            AudioManager.Instance.Play(key: "BuyWeaponClip");
            _selectedSlot.TogglePriceInfo(false);
            executeText.text = "장착 하기";
        }
    }

    private void InitSlots()
    {
        foreach (var itemData in _townItemShopHandler.HandleItemDatas)
        {
            var spawnSlot = Instantiate(slotPrefab, slotsParent);
            spawnSlot.Init(itemData);
            spawnSlot.transform.SetAsLastSibling();
            
            bool isOwnedItem = false;
            
            foreach (var item in _playerInstance.ownedItems)
            {
                if (itemData == item.Data)
                {
                    isOwnedItem = true;
                    
                    spawnSlot.TogglePriceInfo(false);

                    if (_player.EquipmentHandler.EquippedItems.ContainsValue(item))
                    {
                        SetItemInfo(item.Data);
                        spawnSlot.ToggleSelectedBox(true);

                        _selectedSlot = spawnSlot;
                        break;
                    }
                }
            }

            if (!isOwnedItem)
            {
                spawnSlot.TogglePriceInfo(true);
            }
        }
    }
}