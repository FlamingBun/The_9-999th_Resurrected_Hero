using System.Collections.Generic;
using UnityEngine;

public class FloorState
{
    public FloorData Data { get; set; }
    public List<RoomController> TotalRooms { get; } = new();
    public Bounds TotalBounds { get; set; } = new();
    public RoomController StartRoomController { get; set; }
    public List<TowerCurseInstance> Features { get; set; } = new();
}