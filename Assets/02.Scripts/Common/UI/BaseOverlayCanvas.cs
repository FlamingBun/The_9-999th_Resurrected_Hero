using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseOverlayCanvas : MonoBehaviour
{
    public List<string> ChildUINameList { get; } = new();
    public CanvasLayer Layer => _canvasLayer;
    
    private Canvas _canvas;
    
    private CanvasLayer _canvasLayer;
    
    
    public void OnDestroy()
    {
        UIManager.Instance.DestroySceneCanvas(gameObject.scene);
    }

    public void Init(CanvasLayer layer)
    {
        _canvasLayer = layer;
        
        _canvas = GetComponent<Canvas>();
        _canvas.sortingOrder = (int)layer;
        _canvas.gameObject.name =  $"Canvas_{layer}";
    }
}
