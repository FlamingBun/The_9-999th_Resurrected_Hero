using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TowerFloorSelectUIFloorEntry : MonoBehaviour
{
    public List<TowerFloorSelectUIFloorSlot> Slots => _slots;
    
    [SerializeField] private TowerFloorSelectUIFloorSlot slotPrefab;
    [SerializeField] private Transform slotParent;
    
    [Space(5)]
    [SerializeField] private TextMeshProUGUI floorText;
    
    [Space(5)]
    [SerializeField] private CanvasGroup curseInfoCanvasGroup;
    [SerializeField] private TextMeshProUGUI curseInfoTitleText;
    [SerializeField] private TextMeshProUGUI curseInfoDescriptionText;

    private TowerFloorSelectUI _parentUI;
    private FloorData _floorData;
    private List<TowerFloorSelectUIFloorSlot> _slots = new();

    public void Init(TowerFloorSelectUI parentUI, FloorData floorData, int floor)
    {
        _parentUI = parentUI;
        _floorData = floorData;

        floorText.text = floor + "F";
        
        curseInfoDescriptionText.text = "";
        
        CreateSlots(floorData);

        OnPointerExitToSlot();
    }

    public void SetActiveState(bool active)
    {
        if (active)
        {
            floorText.color = Color.white;
        }
        else
        {
            floorText.color = Color.gray;
        }
        
        foreach (var slot in _slots)
        {
            slot.ToggleActiveState(active, _parentUI.SelectedSlot);
        }
    }

    public void OnPointerEnterToSlot(TowerFloorSelectUIFloorSlot enterSlot)
    {
        curseInfoCanvasGroup.alpha = 0;
        curseInfoCanvasGroup.DOFade(1, 0.1f);
        curseInfoCanvasGroup.transform.position = enterSlot.transform.position;
        

        curseInfoTitleText.text = enterSlot.Curse.title;


        bool hasCurseDescription = enterSlot.Curse.featureDescription != "";
        
        curseInfoDescriptionText.gameObject.SetActive(hasCurseDescription);

        if (!hasCurseDescription) return;
        
        curseInfoDescriptionText.text = "";
        curseInfoDescriptionText.text +=
            $"{enterSlot.Curse.featureDescription}  " +
            $"{(enterSlot.Curse.multiplier - 1) * 100}%\n";
    }

    public void OnPointerExitToSlot()
    {
        curseInfoCanvasGroup.DOFade(0, 0.1f);
    }


    public void OnSelectSlot(TowerFloorSelectUIFloorSlot selectSlot)
    {
        _parentUI.OnSelectSlot(selectSlot);
    }

    public void CreateSlots(FloorData floorData)
    {
        List<TowerCurseSO> useFeatures = new();

        for (int i = 0; i < floorData.FloorSelectSlotMaxCount; i++)
        {
            var createSlot = Instantiate(slotPrefab, slotParent.transform);
            _slots.Add(createSlot);
        }

        for (int i = 0; i < _slots.Count; i++)
        {
            int loopCount = 0;

            if (floorData.TowerCurseList.Count > 0)
            {
                while (true)
                {
                    loopCount++;
                    if (loopCount > 100) break;
                
                    int randIndex = Random.Range(0, floorData.TowerCurseList.Count);

                    var target = floorData.TowerCurseList[randIndex];

                    if (target != null)
                    {
                        if (!useFeatures.Contains(target))
                        {
                            useFeatures.Add(target);
                            _slots[i].Init(this, floorData, target);
                            break;
                        }
                    }
                }
            }
          
        }
    }
}
