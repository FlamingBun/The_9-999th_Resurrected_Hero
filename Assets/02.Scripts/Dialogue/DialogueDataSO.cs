using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DialogData", menuName = "Scriptable Objects/NPC/DialogData")]
public class DialogueDataSO : ScriptableObject
{
    public string npcId;
    public string voiceKey;
    public List<DialogueEntry> dialogEntries;
    
}
