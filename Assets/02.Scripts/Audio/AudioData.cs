using UnityEngine;


[CreateAssetMenu(fileName = "AudioData", menuName = "Scriptable Objects/Data/Audio")]
public class AudioData : ScriptableObject
{
    public AudioEntry[] Entries => entries;
    
    [SerializeField] private AudioEntry[] entries;
}
