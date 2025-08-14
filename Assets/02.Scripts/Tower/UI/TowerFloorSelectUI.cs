using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerFloorSelectUI : BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.Overlay;
    public override bool IsEnabled => gameObject.activeSelf;

    public TowerFloorSelectUIFloorSlot SelectedSlot => _selectedSlot;

    [SerializeField] private TowerFloorSelectUIFloorEntry floorEntryPrefab;
    [SerializeField] private TowerFloorSelectUIPlayerIcon playerIconPrefab;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [SerializeField] private TextMeshProUGUI featuresText;
    
    private TowerManager _towerManager;
    private ScrollRect _scrollRect;
    private TowerFloorSelectUIPlayerIcon _playerIcon;
    private TowerFloorSelectUIFloorSlot _selectedSlot;
    private TowerFloorSelectUIFloorEntry _curEntry;
    private List<TowerFloorSelectUIFloorEntry> _entries = new();


    private void Awake()
    {
        _scrollRect = GetComponentInChildren<ScrollRect>();
    }


    public void Init(TowerManager towerManager)
    {
        _towerManager = towerManager;

        for (int i = 0; i <  towerManager.TowerData.FloorDatas.Length; i++)
        {
            var entry = CreatEntry(towerManager.TowerData.FloorDatas[i], i + 1);
            
            if (i == 0)
            {
                if (entry.Slots.Count > 0)
                {
                    _selectedSlot = entry.Slots[0];
                }
            }
            
            _entries.Add(entry);
        }

        if (_entries.Count == 0) return;

        _playerIcon = Instantiate(playerIconPrefab, _entries[0].transform);
        _playerIcon.Init(_towerManager);
    }


 
    public override void Enable()
    {
        _towerManager.Player.InputController.ToggleInput(false);
        gameObject.SetActive(true);

        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            canvasGroup.blocksRaycasts = true;
        });
        
        
        featuresText.text = "";
        
        foreach (var feature in _towerManager.CurCurseList)
        {
            featuresText.text +=
                $"{feature.featureDescription}  " +
                $"{Mathf.RoundToInt((feature.multiplier - 1) * 100)}%\n";
        }
        
        

        Canvas.ForceUpdateCanvases();
        
        var targetEntryIndex = _towerManager.CurFloorIndex;

        _curEntry = _entries[targetEntryIndex];

        for (int i = 0; i < _entries.Count; i++)
        {
            if (_entries[i] == _curEntry)
            {
                _entries[i].SetActiveState(true);

                foreach (var slot in _entries[i].Slots)
                {
                    slot.ToggleDottedLine(true);
                }
                
                MoveScrollToFloor(_entries[targetEntryIndex].transform.position);
                
                continue;
            }
            
            _entries[i].SetActiveState(false);
        }

      
    }

    public override void Disable()
    {
        _towerManager.Player.InputController.ToggleInput(true);
        gameObject.SetActive(false);
    }

    public void OnSelectSlot(TowerFloorSelectUIFloorSlot selectSlot)
    {
        _selectedSlot = selectSlot;
        
        
        AudioManager.Instance.Play(key: "ClickFloorUIClip");

        canvasGroup.blocksRaycasts = false;
        
        foreach (var slot in _curEntry.Slots)
        {
            if (selectSlot != slot)
            {
                slot.ToggleDottedLine(false);
            }
        }
        
        _playerIcon.MoveTo(selectSlot);

        StopAllCoroutines();
        StartCoroutine(SceneLoadDelay(selectSlot));
    }

    IEnumerator SceneLoadDelay(TowerFloorSelectUIFloorSlot selectSlot)
    {
        yield return new WaitForSeconds(1f);

        _playerIcon.transform.SetParent(selectSlot.transform);

        _towerManager.LoadNextFloor(selectSlot.Curse);
    }
    
    private void MoveScrollToFloor(Vector2 targetPos)
    {
        Vector3 localPosInViewport = _scrollRect.viewport.InverseTransformPoint(targetPos);

        Vector3 viewportCenter = _scrollRect.viewport.rect.center;

        Vector3 delta = localPosInViewport - viewportCenter;

        Vector2 newAnchoredPos = _scrollRect.content.anchoredPosition - new Vector2(delta.x, delta.y);
        _scrollRect.content.anchoredPosition = newAnchoredPos;
    }
    
    
    private TowerFloorSelectUIFloorEntry CreatEntry(FloorData floorData, int floor)
    {
        var floorEntry = Instantiate(floorEntryPrefab, _scrollRect.content.transform);
        floorEntry.transform.SetAsFirstSibling();
        floorEntry.Init(this, floorData, floor);

        return floorEntry;
    }

}
