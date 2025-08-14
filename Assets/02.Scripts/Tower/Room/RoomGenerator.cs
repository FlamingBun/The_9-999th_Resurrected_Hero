using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private Transform roomsRoot;

    
    //연결 할 수 있는 방들 후보
    private readonly Queue<Vector2Int> _roomCellQueue = new();
    
    //만들어진 방들의 셀 위치를 가지고 있는 딕셔너리
    private readonly Dictionary<Vector2Int, RoomController> _roomGrid = new();
    
    private FloorManager _floorManager;
    
    private int _targetBasicRoomCount;
    
 
    public void GenerateRooms(FloorManager floorManager)
    {
        _floorManager = floorManager;
        
        //특수한 방들을 제외한 일반 방들의 생성 개수 설정
        _targetBasicRoomCount =
            Random.Range(_floorManager.State.Data.MinBasicRoomCount,
                _floorManager.State.Data.MaxBasicRoomCount + 1);
        
        
        GenerateBasicRooms();
        GenerateEdgeRooms();

        
        //추가적인 floorState 정보 주입
        Bounds totalBounds = new Bounds();
        
        foreach (var roomKvp in _roomGrid)
        {
            var room = roomKvp.Value;
            
            if(totalBounds.size == Vector3.zero)
            {
                totalBounds = room.RoomBounds;
            }
            else
            {
                totalBounds.Encapsulate(room.RoomBounds);
            }
            
            _floorManager.State.TotalRooms.Add(room);
        }

        _floorManager.State.TotalBounds = totalBounds;
    }


    /// <summary>
    /// 특수한 방들이 아닌 일반적인 4방위 검사 하는 방들 생성
    /// </summary>
    private void GenerateBasicRooms()
    {
        int loopCount = 0;
        
        while (_roomGrid.Count < _targetBasicRoomCount)
        {
            if (loopCount++ > 10000)
                throw new System.Exception("무한 루프!");

            
            //초기 1회 정 중앙에 생성
            if (_roomCellQueue.Count == 0)
            {
                if (_roomGrid.Count > 0)
                    throw new System.Exception("생성 가능한 위치 없음!");
                
                
                Vector2Int startCell = Vector2Int.zero;
                    
                RoomController createRoomController = CreateRoom(startCell, GetCreateBasicRoomPrefabs());
                createRoomController.transform.position = Vector3.zero;
                createRoomController.name = GetRoomName($"{_roomGrid.Count}");

                _roomCellQueue.Enqueue(startCell);
                _roomGrid[startCell] = createRoomController;
                    
                continue;
            }
            
            Vector2Int baseCell = _roomCellQueue.Dequeue();
            RoomController roomController = _roomGrid[baseCell];

            
            //연결될 방의 개수를 랜덤하게 조정
            int maxConnectCount = Random.Range(1, 4);
            int curConnectCount = 0;
            
            foreach (var aroundCell in GetRandomAroundCell(baseCell))
            {
                if (_roomGrid.ContainsKey(aroundCell)) continue;

                RoomController createRoomController = CreateRoomToCheckAround(roomController, aroundCell, GetCreateBasicRoomPrefabs());
                
                if(createRoomController == null) continue;
                
                createRoomController.name = GetRoomName($"{_roomGrid.Count}");

                ConnectRooms(createRoomController, roomController);

                _roomCellQueue.Enqueue(aroundCell);
                _roomGrid[aroundCell] = createRoomController;
                
                curConnectCount++;

                
                if (_roomGrid.Count == _targetBasicRoomCount ) return;

                if (maxConnectCount == curConnectCount) break;
            }
        }
    }

    
    /// <summary>
    /// 외곽에 생성되어야 하는 방들을 생성
    /// </summary>
    private void GenerateEdgeRooms()
    {
        List<RoomController> usedEdgeRooms = new();

        
        if (_floorManager.State.Data.StartRoomPrefabs.Count > 0)
        {
            var startRoom = CreateRoomToEdge(usedEdgeRooms, _floorManager.State.Data.StartRoomPrefabs);
            startRoom.name = GetRoomName("Start");
        
            _roomGrid[startRoom.Cell] = startRoom;
            _floorManager.State.StartRoomController = startRoom;
        }
        

        if (_floorManager.State.Data.TransporterRoomPrefabs.Count > 0)
        {
            var endRoom = CreateRoomToEdge(usedEdgeRooms, _floorManager.State.Data.TransporterRoomPrefabs);
            endRoom.name = GetRoomName("End");
        
            _roomGrid[endRoom.Cell] = endRoom;
        }

        if (_floorManager.State.Data.ShopRoomPrefabs.Count > 0)
        {
            var shopRoom = CreateRoomToEdge(usedEdgeRooms, _floorManager.State.Data.ShopRoomPrefabs);
            shopRoom.name = GetRoomName("Shop");

            _roomGrid[shopRoom.Cell] = shopRoom;
        }
        
        if (_floorManager.State.Data.BlessingFountainRoomPrefabs.Count > 0)
        {
            var blessRoom = CreateRoomToEdge(usedEdgeRooms, _floorManager.State.Data.BlessingFountainRoomPrefabs);
            blessRoom.name = GetRoomName("Bless");

            _roomGrid[blessRoom.Cell] = blessRoom;
        }
    }

    
    private RoomController CreateRoom(Vector2Int targetCell, RoomController createPrefab)
    {
        var createRoom = Instantiate(createPrefab, roomsRoot);
        createRoom.Cell = targetCell;

        return createRoom;
    }

    
    /// <summary>
    /// 주변 방들을 생성, 배치, 유효한 위치인지 검사
    /// </summary>
    private RoomController CreateRoomToCheckAround(RoomController roomController, Vector2Int aroundCell, RoomController targetPrefab)
    {
        RoomController createRoomController = CreateRoom(aroundCell,  targetPrefab);
                            
        Vector2 createDir = aroundCell - roomController.Cell;
            
        float roomsSpacing = createDir.x != 0
            ? (_roomGrid[roomController.Cell].RoomBounds.size.x + createRoomController.RoomBounds.size.x) * 0.5f
            : (_roomGrid[roomController.Cell].RoomBounds.size.y + createRoomController.RoomBounds.size.y) * 0.5f;
    
        createRoomController.transform.position =  (Vector2)roomController.transform.position + createDir * roomsSpacing;

        if (!IsValidBounds(createRoomController))
        {
            Destroy(createRoomController.gameObject);
            return null;
        }

        return createRoomController;
    }

    
    /// <summary>
    /// 외곽에 유효한 방들만 생성
    /// </summary>
    private RoomController CreateRoomToEdge(List<RoomController> usedRooms, List<RoomController> targetPrefabs)
    {
        foreach (var roomKvp in _roomGrid)
        {
            //사용된 방들은 검사에 사용되지 않도록 제한 => 시작방에 끝방이 붙는등
            if (usedRooms.Contains(roomKvp.Value))  continue;
            
            var baseRoom = roomKvp.Value;
            var baseCell = roomKvp.Key;

            
            // 주변방들이 유효한지 검사 후 생성
            foreach (var aroundCell in GetRandomAroundCell(baseCell))
            {
                if(_roomGrid.ContainsKey(aroundCell)) continue;
                
                foreach (var targetPrefab in targetPrefabs.OrderBy( _=> Random.value).ToArray())
                {
                    RoomController createdRoomController = CreateRoomToCheckAround(baseRoom, aroundCell, targetPrefab);

                    if (createdRoomController == null) continue;
                        
                    ConnectRooms(createdRoomController, baseRoom);
                        
                    usedRooms.Add(createdRoomController);
                        
                    return createdRoomController;
                }
            }
        }
        
        throw new System.Exception("외곽에 생성할 수 있는 방이 없음!");
    }
    
    
    /// <summary>
    /// 양방향의 방들끼리만 통로
    /// </summary>
    private void ConnectRooms(RoomController createdRoomController, RoomController connectRoomController)
    {
       connectRoomController.ConnectRoom(createdRoomController,createdRoomController.Cell - connectRoomController.Cell);
       createdRoomController.ConnectRoom(connectRoomController,connectRoomController.Cell - createdRoomController.Cell);
    }
    
  
    /// <summary>
    /// 일반 방 생성에 필요한 방들을 결정
    /// </summary>
    /// <returns></returns>
    private RoomController GetCreateBasicRoomPrefabs()
    {
        return _floorManager.State.Data.BasicRoomPrefabs[Random.Range(0, _floorManager.State.Data.BasicRoomPrefabs.Count)];
    }
    

    /// <summary>
    /// 입력 셀의 인접한 4방향 셀을 랜덤으로 변환후 반환
    /// </summary>
    private Vector2Int[] GetRandomAroundCell(Vector2Int inputCell)
    {
        Vector2Int[] aroundCell = 
        {
            inputCell + Vector2Int.left,
            inputCell + Vector2Int.right,
            inputCell + Vector2Int.up,
            inputCell + Vector2Int.down,
        };

        return aroundCell.OrderBy(_ => Random.value).ToArray();
    }
    
    
    /// <summary>
    /// 해당 방이 겹치는 곳이 있는 지 검사
    /// </summary>
    private bool IsValidBounds(RoomController targetRoomController, float adjustmentValue = 0.97f)
    {
        //방들의 Bounds 는 실수형이므로 근사값에도 겹쳐진다고 판단하기 때문에
        //축소 시킨 임시 Bounds 로 검사를 시행
        Bounds targetCheckBounds = new Bounds(targetRoomController.RoomBounds.center, targetRoomController.RoomBounds.size * adjustmentValue);
        
        foreach (var roomKvp in _roomGrid)
        {
            if (targetCheckBounds.Intersects(roomKvp.Value.RoomBounds))
            {
                return false;
            }
        }
        
        return true;
    }

    private string GetRoomName(string addName) => $"Room - {addName}";
}
