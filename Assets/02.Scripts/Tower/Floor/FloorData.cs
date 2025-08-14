using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "FloorData", menuName = "Scriptable Objects/Stage/FloorData")]
public class FloorData : ScriptableObject
{
    public int MaxBasicRoomCount => maxBasicRoomCount;
    public int MinBasicRoomCount => minBasicRoomCount;
    
    public  List<RoomController> StartRoomPrefabs => startRoomPrefabs;
    public  List<RoomController> BasicRoomPrefabs => basicRoomPrefabs;
    
    
    public  List<RoomController> TransporterRoomPrefabs => transporterRoomPrefabs;
    public  List<RoomController> ShopRoomPrefabs => shopRoomPrefabs;
    public List<RoomController> BlessingFountainRoomPrefabs => blessingFountainRoomPrefabs;
    public List<TowerCurseSO> TowerCurseList => towerCurseList;
    
    public Sprite FloorSelectIcon_Enable => floorSelectIcon_Enable;
    public Sprite FloorSelectIcon_Disable => floorSelectIcon_Disable;
    public Sprite FloorSelectIcon_Hover => floorSelectIcon_Hover;
    public int FloorSelectSlotMaxCount => floorSelectSlotMaxCount;

    

    [Min(1)] [SerializeField] private int maxBasicRoomCount;
    [Min(1)] [SerializeField] private int minBasicRoomCount;

    [Space(10f)]
    [Header("Base Rooms")]
    [SerializeField] private List<RoomController> startRoomPrefabs;
    [SerializeField] private List<RoomController> basicRoomPrefabs;
    [SerializeField] private List<RoomController> transporterRoomPrefabs;

    [Space(10f)]
    [Header("Additional Rooms")]
    [SerializeField] private List<RoomController> shopRoomPrefabs;
    [SerializeField] private List<RoomController> blessingFountainRoomPrefabs;


    [Space(10f)] 
    [SerializeField] private Sprite floorSelectIcon_Enable;
    [SerializeField] private Sprite floorSelectIcon_Disable;
    [SerializeField] private Sprite floorSelectIcon_Hover;
    [SerializeField] private int floorSelectSlotMaxCount;
    [SerializeField] private List<TowerCurseSO> towerCurseList;
}
