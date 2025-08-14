using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundType
{
    MasterVolume,
    BGMVolume,
    SFXVolume
}

public class SettingManager : MonoBehaviour
{
    private const float muteDecibels = -80f;
    

    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private float defaultVolume = 0.6f;

    private Dictionary<SoundType, float> _soundVolumes = new();

    private Dictionary<SoundType, bool> _soundMutes = new();
    
    private Dictionary<SoundType, string> _soundSettingKeys = new();
    
    public void Init()
    {
        foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
        {
            _soundSettingKeys[type] = type.ToString();
            _soundVolumes[type] = defaultVolume;
            _soundMutes[type] = false;
        }
        
        SoundInit();
    }
    
    public bool IsMuted(SoundType type)
    {
        return _soundMutes[type];
    }
    
    public float GetVolume(SoundType type)
    {
        return _soundVolumes[type];
    }

    public void ToggleSoundMute(SoundType type)
    {
        _soundMutes[type] = !_soundMutes[type];
        SetVolume(type);
    }

    public void SetVolume(SoundType type)
    {
        if (_soundMutes[type])
        {
            audioMixer.SetFloat(_soundSettingKeys[type], muteDecibels);
        }
        else
        {
            float volume = _soundVolumes[type];
            audioMixer.SetFloat(_soundSettingKeys[type], LinearToDecibels(volume));
        }
    }

    public void SetVolume(SoundType type, float volume)
    {
        _soundVolumes[type] = volume;
        
        if (!_soundMutes[type])
        {
            if (volume < 0.1f)
            {
                audioMixer.SetFloat(_soundSettingKeys[type], muteDecibels);
            }
            else
            {
                audioMixer.SetFloat(_soundSettingKeys[type], LinearToDecibels(volume));   
            }   
        }
        
        PlayerPrefs.SetFloat(_soundSettingKeys[type], volume);
    }
    
    private void SoundInit()
    {
        int count = Enum.GetValues(typeof(SoundType)).Length;

        for (int i = 0; i < count; i++)
        {
            string soundKey = _soundSettingKeys[(SoundType)i];
            
            if (PlayerPrefs.HasKey(soundKey))
            {
                float volume = PlayerPrefs.GetFloat(soundKey);
                
                _soundVolumes[(SoundType)i] = volume;
                SetVolume((SoundType)i,volume);
            }
            else
            {
                float volume = _soundVolumes[(SoundType)i];
                
                SetVolume((SoundType)i, volume);
            }
        }
    }
    
    
    private float LinearToDecibels(float linear)
    {
        return Mathf.Log10(linear) * 20f;
    }
}
