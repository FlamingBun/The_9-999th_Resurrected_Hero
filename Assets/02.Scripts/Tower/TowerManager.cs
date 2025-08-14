using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TowerManager : MonoBehaviour
{
    public TowerData TowerData => towerData;
    public FloorData CurFloorData => towerData.FloorDatas[_curFloorIndex];
    public PlayerController Player { get; private set; }
    public List<TowerCurseInstance> CurCurseList => _curseList;
    public int CurFloorIndex => _curFloorIndex;

    [SerializeField] private TowerData towerData;
    [SerializeField] private PlayerController playerPrefab;

    private UIManager _uiManager;
    private TowerFloorSelectUI _towerFloorSelectUI;
    private List<TowerCurseInstance> _curseList = new();

    private int _curFloorIndex;
    
    private void Start()
    {
        var gameManager = GameManager.Instance;

        Player = gameManager.Player;
        
        InitUI();

        Time.timeScale = 1;
        AudioManager.Instance.Play("TowerBGM");
    }

    private void OnDestroy()
    {
        AudioManager.Instance.StopLoopAudio("TowerBGM");

        Player.ToggleClampBounds(false, new Bounds());
        
        ObjectPoolManager.Instance.DeSpawnAll();
    }

    // TESTCODE
    // public void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         ClearFloor();     
    //         Player.PlayerInstance.SetSoul(1000);
    //     }
    // }
    //
    // private void Update()
    // {
    //     //테스트용 코드
    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         ClearFloor();
    //     }
    //
    //     if (Input.GetKeyDown(KeyCode.R))
    //     {
    //         //임시 재생성
    //         
    //         List<string> loadScenes = new() 
    //         {
    //             Constant.FloorSceneName,
    //         };
    //     
    //         SceneLoadManager.Instance.LoadAddtiveScenes(loadScenes, loadScenes);
    //     }
    // }

    public void LoadNextFloor(TowerCurseSO addCurseSO)
    {

        
        List<string> loadScenes = new() 
        {
            Constant.FloorSceneName,
        };
        
        
        SceneLoadManager.Instance.LoadAddtiveScenes(loadScenes, loadScenes);
        
        
        foreach (var feature in _curseList)
        {
            if (feature.CurseType == addCurseSO.curseType)
            {
                feature.ModifyMult(addCurseSO);
                return;
            }
        }
        _curseList.Add(new TowerCurseInstance(addCurseSO));
    }
    
    
    public void ClearFloor()
    {
        Player.StatHandler.RemoveModifiersFromSource(Constant.FloorSceneName);
        
        if (_curFloorIndex == TowerData.FloorDatas.Length - 1)
        {
            AudioManager.Instance.StopLoopAudio("TowerBGM");
            ClearTower();
            return;
        }
        
        _curFloorIndex++;
        _towerFloorSelectUI.Enable();
    }

    

    private void ClearTower()
    {
        _uiManager.GetUI<EndingUI>().Enable();
    }
    
    
    private void InitUI()
    {
        _uiManager = UIManager.Instance;

        var thisScene = gameObject.scene;

        var towerHUDUI = _uiManager.CreateUI<TowerHUDUI>(thisScene);
        towerHUDUI.Init(this, Player);

        
        var blessingFountainUI = _uiManager.CreateUI<TowerBlessingFountainUI>(thisScene);
        blessingFountainUI.Init(Player);

        
        _towerFloorSelectUI = _uiManager.CreateUI<TowerFloorSelectUI>(thisScene);
        _towerFloorSelectUI.Init(this);

        _uiManager.CreateUI<TowerMapUI>(thisScene);
        _uiManager.CreateUI<TowerRoomShopUI>(thisScene);
        //_uiManager.CreateUI<TowerClearUI>(thisScene);
        _uiManager.CreateUI<EndingUI>(thisScene).Init(Player);
        _uiManager.CreateUI<TowerDeathUI>(thisScene).Init(this);
    }

    
    
}