using UnityEngine;
using UnityEngine.Audio;


[System.Serializable]
public class AudioEntry
{
    public string Key => key;
    public bool IsLoop => isLoop;
    public AudioMixerGroup MixerGroup => mixerGroup;
    public AudioClip Clip => clip;
    public float Volume => volume;


    [SerializeField] private string key;
    [SerializeField] private bool isLoop;
    [SerializeField] private AudioMixerGroup mixerGroup;
    [SerializeField] private AudioClip clip;
    [SerializeField][Range(0f, 1f)] private float volume = 1f;
}
