using System.Collections;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class AudioSourceHandler : MonoBehaviour
{
    public string Key { get; private set; }
    
    public AudioSource AudioSource => _audioSource;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    

    public void Play(string key, float volume, AudioEntry entry, AudioManager audioManager)
    {
        Key = key;

        _audioSource.outputAudioMixerGroup = entry.MixerGroup;
        _audioSource.loop = entry.IsLoop;
        _audioSource.volume = volume;

        if (entry.IsLoop)
        {
            _audioSource.clip = entry.Clip;
            _audioSource.Play();
        }
        else
        {
            _audioSource.clip = null; // PlayOneShot 쓸 땐 clip 초기화 안 해도 되지만 안전하게 제거
            _audioSource.PlayOneShot(entry.Clip, volume);
            StartCoroutine(ReturnCoroutine(entry.Clip.length, audioManager)); // PlayOneShot은 clip 길이 기준으로 반환
        }
    }

    IEnumerator ReturnCoroutine(float clipLength, AudioManager rootManager)
    {
        yield return new WaitForSeconds(clipLength);

        _audioSource.Stop();
        rootManager.AudioPool.Enqueue(this);
    }
}
