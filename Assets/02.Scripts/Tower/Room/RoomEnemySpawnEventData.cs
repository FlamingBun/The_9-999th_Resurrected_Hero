using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemySpawnEventData", menuName = "Scriptable Objects/Data/EnemySpawnEventData")]
public class RoomEnemySpawnEventData : ScriptableObject
{
    public int maxSpawnCount;
    public int limitSpawnCount;
    public List<EnemyDataSO> spawnEnemyDatas;
}
