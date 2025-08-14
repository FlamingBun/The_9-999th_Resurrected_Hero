using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TowerClearUI : BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.Popup;
    public override bool IsEnabled => gameObject.activeSelf;



    [SerializeField] private Button townButton;
    
    private void Awake()
    {
        List<string> loadScenes = new()
        {
            Constant.TownSceneName
        };
        
        List<string> unloadScenes = new()
        {
            Constant.TowerSceneName,
            Constant.FloorSceneName
        };
        
        
        townButton.onClick.AddListener(() =>
        {
            SceneLoadManager.Instance
           .LoadAddtiveScenes(loadScenes, unloadScenes);
        });
    }


    public override void Enable()
    {
        gameObject.SetActive(true);
    }

    public override void Disable()
    {
        gameObject.SetActive(false);
    }
}
