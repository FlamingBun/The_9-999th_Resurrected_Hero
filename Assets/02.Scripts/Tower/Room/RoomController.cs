using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEngine;


public enum RoomType
{
    BasicEnemy,
    Start,
    Boss,
    Shop,
    BlessingFountain,
    Transporter
}

public class RoomController: MonoBehaviour
{
    public FloorManager FloorManager { get; private set; }
    public Bounds RoomBounds => new (transform.position, _areaCollider.size);
    public Tilemap EnemySpawnTilemap;
    public Transform TPPoint => tpPoint;
    public Vector2Int Cell { get; set; }


    public Dictionary<Dir4, RoomController> ConnectedRooms => _connectedRooms;
    public RoomType RoomType => roomType;


    [SerializeField] private RoomType roomType;
    [SerializeField] private Transform tpPoint;
    [SerializeField] private Transform roomTilemapRoot;
    [SerializeField] private Tilemap minimapTilemap;
    [SerializeField] private Tilemap enemySpawnTilemap;
    [SerializeField] private LayerMask playerLayerMask;

    private bool isDiscovered;
    private Dictionary<Dir4, RoomController> _connectedRooms = new();
    private RoomPath[] _paths;
    private Tilemap[] _roomTilemaps;
    private BoxCollider2D _areaCollider;
    private RoomLightHandler _lightHandler;
    private Coroutine _roomEventRoutine;
    private PlayerController _enteredPlayer;
    private IRoomEvent[] _roomEvents;
    
    
    
    protected virtual void Awake()
    {
        _lightHandler = GetComponent<RoomLightHandler>();
        _areaCollider = GetComponent<BoxCollider2D>();
        _paths = GetComponentsInChildren<RoomPath>();
        _roomTilemaps = roomTilemapRoot.GetComponentsInChildren<Tilemap>();
        _roomEvents = GetComponents<IRoomEvent>();



        Dir4[] dirArray = (Dir4[])System.Enum.GetValues(typeof(Dir4));
        
        for (int i = 0; i < _paths.Length; i++)
        {
            _paths[i].Init(dirArray[i]);
        }
        
        minimapTilemap.gameObject.SetActive(false);
    }
    

    private void OnTriggerStay2D(Collider2D other)
    {
        if ((playerLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            FloorManager.EnterRoom(this);
        }
    }
    

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((playerLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            FloorManager.ExitRoom();
        }
    }
    
    

    public void Init(FloorManager floorManager)
    {
        FloorManager = floorManager;
      
        foreach (var roomEvent in _roomEvents)
        {
            roomEvent.Init(this);
        }
        
        foreach (var path in _paths)
        {
            foreach (var roomKvp in _connectedRooms)
            {
                if (roomKvp.Key == path.LookDir)
                {
                    path.ActivePath(_roomTilemaps);
                    break;
                }
            }
        }
        
        _lightHandler.Init(floorManager, this);
    }

   
    
    public void ConnectRoom(RoomController connectRoomController, Vector2Int targetDir)
    {
        Dir4 dir = Dir4.Up;
        
        if (targetDir == Vector2Int.up)
        {
            dir = Dir4.Up;
        }
        else if (targetDir == Vector2Int.down)
        {
            dir = Dir4.Down;
        }
        else if (targetDir == Vector2Int.right)
        {
            dir = Dir4.Right;
        }
        else if (targetDir == Vector2Int.left)
        {
            dir = Dir4.Left;
        }

        _connectedRooms[dir] = connectRoomController;
    }

    
    
    public void OnEnter()
    {
        if (!isDiscovered)
        {
            isDiscovered = true;

            if (_roomEventRoutine != null)
            {
                StopCoroutine(_roomEventRoutine);
            }

            _roomEventRoutine = StartCoroutine(RoomEventRoutine());
            
            UIManager.Instance.GetUI<TowerMapUI>().EnableDiscoveredRoomUI(this);
        }
        
        EnableMinimapTilemaps();
        
        _lightHandler.TurnOnLights();
    }
    
    
    
    public void OnExit()
    {
        DisableMinimapTilemaps();
        
        _lightHandler.TurnOffLights(isDiscovered);
    }
    


    // 임시 수정
    private void EnableMinimapTilemaps()
    {
        minimapTilemap.gameObject.SetActive(true);

        foreach (var path in _paths)
        {
            if (path.gameObject.activeInHierarchy)
            {
                path.ShowMinimapTilemap();
            }
        }
    }
    
    
    private void DisableMinimapTilemaps()
    {
        minimapTilemap.gameObject.SetActive(false);

        foreach (var path in _paths)
        {
            if (path.gameObject.activeInHierarchy)
            {
                path.HideMinimapTilemap();
            }
        }
    }

    
    private IEnumerator RoomEventRoutine()
    {
        foreach (var path in _paths) path.EnableBarrier();

        var player = FloorManager.Player;
        
        player.ToggleLockSprint(true);
        player.InputController.ToggleTowerMapInput(false);
        
        player.ToggleClampBounds(true, RoomBounds);
        
        _roomEvents = _roomEvents.OrderBy(roomEvent => roomEvent.Order).ToArray();
        
        foreach (var roomEvent in _roomEvents)
        {
            if (!roomEvent.IsCleared)
            {
                roomEvent.StartEvent();
                
                yield return new WaitUntil(() => roomEvent.IsCleared);
            }
        }
        
        foreach (var path in _paths) path.DisableBarrier();
        
        player.ToggleLockSprint(false);
        player.InputController.ToggleTowerMapInput(true);
        
        player.ToggleClampBounds(false, RoomBounds);
    }
}
