using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public static class DOTweenLight2DExt
{
    /// <summary>
    /// Light2D.intensity 값을 DOTween으로 애니메이션
    /// </summary>
    public static Tweener DOIntensity(this Light2D target, float endValue, float duration)
    {
        return DOTween.To(() => target.intensity, x => target.intensity = x, endValue, duration);
    }
}
public class RoomLightHandler: MonoBehaviour
{
    private Dictionary<Light2D, float> _roomLights = new();
    private List<LightPulse> _lightPulses = new();
    private List<Tween> _activeTweens = new();
    private Light2D _discoverLight;

    private void OnDestroy()
    {
        KillAllTweens();
    }

    public void Init(FloorManager floorManager, RoomController room)
    {
        GenerateDiscoverLight(room.RoomBounds, floorManager.discoverRoomLight);
        
        if (!Application.isPlaying)
            return;
        
        Light2D[] lights = GetComponentsInChildren<Light2D>(true);

        _lightPulses = GetComponentsInChildren<LightPulse>().ToList();
        
        foreach (var light in lights)
        {
            _roomLights[light] = light.intensity;
            
            light.intensity = 0f;
            light.gameObject.SetActive(false);
        }
    }

  

    
    /// <summary>
    /// Lighting On
    /// </summary>
    public void TurnOnLights()
    {
        KillAllTweens();

        foreach (var lightPair in _roomLights)
        {
            Light2D light = lightPair.Key;
            float targetIntensity = lightPair.Value;

            light.gameObject.SetActive(true);

            Tween tween = light.DOIntensity(targetIntensity, 1f).SetEase(Ease.OutQuad);
            _activeTweens.Add(tween);
        }

        foreach (LightPulse lightPulse in _lightPulses)
        {
            lightPulse.StartPulse();
        }
    }

    /// <summary>
    /// Lighting Off
    /// </summary>
    public void TurnOffLights(bool isDiscovered)
    {
        if (isDiscovered)
        {
            FixDiscoverLight();
        }
        
        foreach (LightPulse lightPulse in _lightPulses)
        {
            lightPulse.StopPulse();
        }
        
        KillAllTweens();

        foreach (var lightPair in _roomLights)
        {
            Light2D light = lightPair.Key;
            
            Tween tween = light.DOIntensity(0f, 0.5f).SetEase(Ease.InQuad).OnComplete(()=>light.gameObject.SetActive(false));
            _activeTweens.Add(tween);
        }
    }

    private void KillAllTweens()
    {
        foreach (var tween in _activeTweens)
        {
            if (tween != null && tween.IsActive())
            {
                tween.Kill();
            }
        }
        _activeTweens.Clear();
    }
    

   
    private void GenerateDiscoverLight(Bounds bounds, Light2D discoverLightPrefab)
    {
        _discoverLight = Instantiate(discoverLightPrefab, transform);
        _discoverLight.transform.position = bounds.center;
        _discoverLight.lightType = Light2D.LightType.Freeform;

        Vector3[] lightShapePath = new Vector3[4];
        
        float halfWidth = bounds.extents.x;
        float halfHeight = bounds.extents.y;

        halfWidth -= 1f;
        halfHeight -= 1f;
        
        lightShapePath[0] = new Vector3(-halfWidth, -halfHeight, 0); // 좌하단
        lightShapePath[1] = new Vector3( halfWidth, -halfHeight, 0); // 우하단
        lightShapePath[2] = new Vector3( halfWidth,  halfHeight, 0); // 우상단
        lightShapePath[3] = new Vector3(-halfWidth,  halfHeight, 0); // 좌상단

        _discoverLight.SetShapePath(lightShapePath);
        _discoverLight.shapeLightFalloffSize = 4;
        _discoverLight.falloffIntensity = 0.5f;
    }
    
    private void FixDiscoverLight()
    {
        if (_roomLights.ContainsKey(_discoverLight))
        {
            _discoverLight.intensity = _roomLights[_discoverLight];
            _roomLights.Remove(_discoverLight);
        }
    }
}