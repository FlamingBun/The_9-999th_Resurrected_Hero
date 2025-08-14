using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum CanvasLayer
{
    HUDBackground,
    HUD,
    PopupBackground,
    Popup,
    Overlay
}

public class UIManager: Singleton<UIManager>
{
    private Dictionary<string, BaseUI> _createdUIs = new();
    private Dictionary<Scene, List<BaseOverlayCanvas>> _sceneCreatedCanvases = new();

    private readonly string _resourcesPath = "UI/";
    private readonly string _canvasPrefabName = "BaseOverlayCanvas";
    
    
    
    public T GetUI<T>() where T : BaseUI
    {
        if (_createdUIs.TryGetValue(typeof(T).Name, out var ui))
        {
            return ui as T;
        }

        return null;
    }
    
    
    public T CreateUI<T> (Scene targetScene) where T : BaseUI
    {
        var createdUI = GetUI<T>();
        
        if (createdUI != null)  return createdUI;
        
        
        
        var uiName = typeof(T).Name;
        
        var uiPrefab = Resources.Load<BaseUI>($"{_resourcesPath}{uiName}");
        
        if (uiPrefab == null) return null;

        
        CanvasLayer layer = uiPrefab.Layer;

        if (!_sceneCreatedCanvases.ContainsKey(targetScene))
        {
            _sceneCreatedCanvases[targetScene] = new();
        }
        
        
        
        BaseOverlayCanvas canvas = null;

        var canvasList = _sceneCreatedCanvases[targetScene];
            
        for (int i = 0; i < canvasList.Count; i++)
        {
            if (canvasList[i].Layer == layer)
            {
                canvas = canvasList[i];
                break;
            }
        }
        
        
        
        if (canvas == null)
        {
            var canvasPrefab = Resources.Load<BaseOverlayCanvas>($"{_resourcesPath}{_canvasPrefabName}");
            
            canvas = Instantiate(canvasPrefab);
            canvas.Init(layer);
            
            canvasList.Add(canvas);
            
            SceneManager.MoveGameObjectToScene(canvas.gameObject, targetScene);
        }
       

        var createUI = Instantiate(uiPrefab, canvas.transform);
        createUI.gameObject.SetActive(false);
        
        _createdUIs.Add(uiName, createUI);
        canvas.ChildUINameList.Add(uiName);
        
        return createUI as T;
    }

    public void DestroySceneCanvas(Scene scene)
    {
        if (_sceneCreatedCanvases.ContainsKey(scene))
        {
            foreach (var canvas in _sceneCreatedCanvases[scene])
            {
                foreach (var childUIName in canvas.ChildUINameList)
                {
                    if (_createdUIs.ContainsKey(childUIName))
                    {
                        _createdUIs.Remove(childUIName);
                    }
                }
            }
            
            _sceneCreatedCanvases.Remove(scene);
        }
    }
}