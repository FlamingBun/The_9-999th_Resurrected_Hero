using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    public Queue<AudioSourceHandler> AudioPool => _audioPool;

    private AudioSourceHandler _audioSourceHandlerPrefab;
    private Dictionary<string, AudioEntry> _audioEntries = new();
    
    
    private List<AudioSourceHandler> _playingLoopHandlers = new();
    private Queue<AudioSourceHandler> _audioPool = new();
    
    
    private readonly string _audioPath = "Audio/";
    private readonly string _audioDataPath = "AudioData";
    private readonly string _audioSourcePath = "AudioSource";
    
    
    protected override void Awake()
    {
        base.Awake();
        
        var audioData = Resources.Load<AudioData>(_audioPath + _audioDataPath);
        
        _audioSourceHandlerPrefab = Resources.Load<AudioSourceHandler>(_audioPath + _audioSourcePath);
        
        if (audioData == null || _audioSourceHandlerPrefab == null)
        {
            Debug.LogError("잘못된 리소스");
            return;
        }
        
        
        foreach (var item in audioData.Entries)
        {
            _audioEntries[item.Key] = item;
        }        
    }
    
    
    public void Play(string key, float volume = 1)
    {
        if (_audioEntries.TryGetValue(key, out AudioEntry entry))
        {
            AudioSourceHandler audioSourceHandler = _audioPool.Count > 0 ? _audioPool.Dequeue() : Instantiate(_audioSourceHandlerPrefab, transform);

            if (entry.IsLoop)
                _playingLoopHandlers.Add(audioSourceHandler);

            audioSourceHandler.Play(key, volume * entry.Volume, entry, this);
        }
    }
    
    
    public void StopLoopAudio(string key)
    {
        var targetHandler = _playingLoopHandlers.FirstOrDefault((handler) => handler.Key == key);

        if (targetHandler != null)
        {
            targetHandler.AudioSource.Stop();
            
            _audioPool.Enqueue(targetHandler);

            _playingLoopHandlers.Remove(targetHandler);
        }
    }

    /// <summary>
    /// 현재 재생 중인 모든 루프 사운드를 정지시킵니다.
    /// 특정 키를 제외할 수 있습니다.
    /// </summary>
    /// <param name="exceptionKey">정지시키지 않을 루프 사운드의 키</param>
    public void StopAllLoopAudiosExcept(string exceptionKey = "")
    {
        foreach (var handler in _playingLoopHandlers.ToList()) // 리스트 복사본을 사용해 안전하게 순회
        {
            if (handler.Key != exceptionKey)
            {
                handler.AudioSource.Stop();
                _audioPool.Enqueue(handler);
                _playingLoopHandlers.Remove(handler);
            }
        }
    }
}