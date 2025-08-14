using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Scriptable Objects/Quest/QuestData")]
public class QuestDataSO : ScriptableObject
{
    public string title;
    
    [Space(10f)]
    [TextArea(2, 5)] public string description;

    [Space(10f)] 
    public bool isCollection;
    public int collectionCount;
}
