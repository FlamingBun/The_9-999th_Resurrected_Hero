using UnityEngine;

[CreateAssetMenu(fileName = "BlessingFountainData", menuName = "Scriptable Objects/BlessingFountain")]
public class BlessingFountainBlessDataSO : ScriptableObject
{
    public int MaxBlessCount => maxBlessCount;
    public int MinBlessCount => minBlessCount;
    
    public BlessingFountainBlessEntry[] ModifierEntries => modifierEntries;

    [SerializeField] private int minBlessCount;
    [SerializeField] private int maxBlessCount;
    [SerializeField] private BlessingFountainBlessEntry[] modifierEntries;
}