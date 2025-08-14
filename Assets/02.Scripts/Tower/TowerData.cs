using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Objects/Tower/TowerData")]
public class TowerData : ScriptableObject
{
   public FloorData[] FloorDatas => floorDatas;

   [SerializeField] private FloorData[] floorDatas;
}


