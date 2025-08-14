using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[Serializable]
public struct LightPulseData
{
    public Light2D light;
    public float startValue;
    public float endValue;
    public float pulseDuration;
}

public class LightPulse : MonoBehaviour
{
    public List<LightPulseData> lightPulseDatas;
    
    private List<Tween> lightPulseTweens = new();
    
    public void StartPulse()
    {
        foreach (LightPulseData lightData in lightPulseDatas)
        {
            lightData.light.intensity = lightData.startValue;
            lightPulseTweens.Add(lightData.light.DOIntensity(lightData.endValue, lightData.pulseDuration).SetLoops(-1, LoopType.Yoyo));
        }
    }

    public void StopPulse()
    {
        foreach (Tween lightPulseTween in lightPulseTweens)
        {
            lightPulseTween.Kill();
        }
    }
}
