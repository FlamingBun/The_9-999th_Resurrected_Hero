using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "FloorCurse", menuName = "Scriptable Objects/Tower/FloorCurse")]

public class TowerCurseSO : ScriptableObject
{
    public string title;
    public TowerCurseType curseType;
    public float multiplier;
    [TextArea] public string featureDescription;
}
