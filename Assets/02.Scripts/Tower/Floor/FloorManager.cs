using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FloorManager : MonoBehaviour
{
    public PlayerController Player => _towerManager.Player;
    public FloorMinimapCamera FloorMinimapCamera => _floorMinimapCamera;
    public FloorState State => _state;

    public Light2D discoverRoomLight;
    
    private TowerManager _towerManager;
    private RoomController _curEnteredRoomController;
    private FloorState _state;
    private AstarPath _astarPath;
    private FloorMinimapCamera _floorMinimapCamera;
    private RoomGenerator _roomGenerator;
    
    //UI
    private TowerHUDUI _towerHUD;
    private TowerMapUI _mapUI;

    private bool isScan = false;
    
    private void Start()
    {
        _astarPath = FindAnyObjectByType<AstarPath>();
        _towerManager = FindAnyObjectByType<TowerManager>();
        _floorMinimapCamera = FindAnyObjectByType<FloorMinimapCamera>();
        _roomGenerator = FindAnyObjectByType<RoomGenerator>();

        _state = new()
        {
            Features = _towerManager.CurCurseList,
            Data = _towerManager.CurFloorData
        };
        
        _roomGenerator.GenerateRooms(this);
        
        
        foreach (var room in State.TotalRooms)
        {
            room.Init(this);
        }
        
        InitUI();
        

        Player.MoveDirectTo(State.StartRoomController.transform.position);
        
        EnterRoom(State.StartRoomController);
        
        CameraManager.Instance.SetTargetToFollowCamera(Player.transform);
        
        Player.InputController.OnEnableFloorMapUI += _mapUI.Enable;

    }

    private void LateUpdate()
    {
        if (!isScan)
        {
            isScan = true;
            ScanAstarPath();
        }
    }


    public void OnDestroy()
    {
        Player.InputController.OnEnableFloorMapUI -= _mapUI.Enable;
    }

    public void EnterRoom(RoomController enteredRoomController)
    {
        if (_curEnteredRoomController != null) return;
        
        enteredRoomController.OnEnter();
        
        _mapUI.SetEnteredUI(enteredRoomController);
        _towerHUD.UpdateMinimap(enteredRoomController);
        
        _curEnteredRoomController = enteredRoomController;
    }

    public void ExitRoom()
    {
        if (_curEnteredRoomController != null)
        {
            _curEnteredRoomController.OnExit();

            _curEnteredRoomController = null;
        }
    }

    public float GetFeatureMultiplier(TowerCurseType type)
    {
        if (_state.Features != null)
        {
            foreach (var feature in _state.Features)
            {
                if (feature.CurseType == type)
                {
                   return feature.multiplier;
                }
            }
        }

        return 1;
    }

    private void InitUI()
    {
        var uiManager = UIManager.Instance;

        _mapUI = uiManager.GetUI<TowerMapUI>();
        _mapUI.Init(this);
        _mapUI.Disable();

        _towerHUD = uiManager.GetUI<TowerHUDUI>();
        _towerHUD.InitMinimap(this);
        _towerHUD.UpdateFloorText();
        _towerHUD.ClearGemEffect();
        _towerHUD.Enable();
        
        var floorSelectUI = uiManager.GetUI<TowerFloorSelectUI>();
        floorSelectUI.Disable();
    }

    private void ScanAstarPath()
    {
        var totalBounds = State.TotalBounds;
        _astarPath.data.gridGraph.center = totalBounds.center;
        _astarPath.data.gridGraph.SetDimensions(Mathf.CeilToInt(totalBounds.size.x) * 2, Mathf.CeilToInt(totalBounds.size.y) * 2, 1f);
        AstarPath.active.Scan();
    }
}
