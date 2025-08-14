using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerDeathUI : BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.Overlay;

    public override bool IsEnabled => gameObject.activeInHierarchy;

    [SerializeField] private Button townButton;
    [SerializeField] private TextMeshProUGUI floorText;

    private TowerManager _towerManager;
    private CanvasGroup _canvasGroup;
    
    public void Init(TowerManager towerManager)
    {
        _towerManager = towerManager;

        _canvasGroup = townButton.GetComponent<CanvasGroup>();
       
        
        townButton.onClick.AddListener(() =>
        {
            List<string> loadScenes = new()
            {
                Constant.TownSceneName,
            };
            
            List<string> unloadScenes = new()
            {
                Constant.TowerSceneName,
                Constant.FloorSceneName
            };

            SceneLoadManager.Instance.LoadAddtiveScenes(loadScenes, unloadScenes).OnCompleteLoad(() => Time.timeScale = 1);
        });
    }

  

    public override void Enable()
    {

        
        gameObject.SetActive(true);
        
        _canvasGroup.alpha = 0f; 
        _canvasGroup.DOFade(1f, 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear)
            .SetUpdate(false);

        floorText.text = (_towerManager.CurFloorIndex + 1).ToString() + "F";
    }

    public override void Disable()
    {
        gameObject.SetActive(false);
    }
}
