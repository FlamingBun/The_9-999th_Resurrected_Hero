using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameManager : Singleton<GameManager>
{
    public PlayerController Player { get; private set; }
    public DataManager DataManager => _dataManager;
    public SettingManager SettingManager => _settingManager;

    private DataManager _dataManager;
    private SettingManager _settingManager;
    
    private GameResources _gameResource;

    public Texture2D cursorTexture;


    
    private const string GameResourcesPath = "GameResources";
    
    protected override void Awake()
    {
        base.Awake();
        
        Application.targetFrameRate = 144;

        _gameResource = Resources.Load<GameResources>(GameResourcesPath);

        _dataManager = GetComponentInChildren<DataManager>();
        _dataManager.Init();

        _settingManager = GetComponentInChildren<SettingManager>();
    }

    private void Start()
    {
        _settingManager.Init();
        
        var baseScene = SceneManager.GetSceneByName(Constant.BaseSceneName);
        
        var uiManager = UIManager.Instance;
        
        var startScreen = uiManager.CreateUI<StartScreenUI>(baseScene);
        var introUI =  uiManager.CreateUI<IntroUI>(baseScene);
        var statusHUD = uiManager.CreateUI<StatusHUDUI>(baseScene);
        var letterBox = uiManager.CreateUI<LetterBoxUI>(baseScene);

        uiManager.CreateUI<DamagePopupUI>(baseScene).Enable();
        uiManager.CreateUI<StatusMenuUI>(baseScene);
        uiManager.CreateUI<InteractGuideUI>(baseScene);
        uiManager.CreateUI<QuestUI>(baseScene);
        uiManager.CreateUI<FadeScreenUI>(baseScene);
        uiManager.CreateUI<SettingUI>(baseScene);
        uiManager.CreateUI<ScreenIndicatorUI>(baseScene).Enable();
        
        
        Player = Instantiate(_gameResource.playerPrefab);
        Player.Init(_dataManager.LoadPlayer());
        
        letterBox.Init(Player);
        
        startScreen.Init(Player);
        startScreen.Enable();
        
        introUI.Init(Player);
        
        statusHUD.Init(Player);
        statusHUD.Enable();
        
        Vector2 hotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);

        CursorMode cursorMode = CursorMode.Auto;

        Cursor.SetCursor(cursorTexture, hotspot, cursorMode);
        
        AudioManager.Instance.Play("StartBGM");
    }
    
    
    public void Death()
    {
        StartCoroutine(DeathRoutine());
    }
    
    private IEnumerator DeathRoutine()
    {
        var uiManager = UIManager.Instance;
        var fadeScreen = uiManager.GetUI<FadeScreenUI>();

        bool isFade = false;
        
        fadeScreen.Enable();
        fadeScreen.FadeIn(2f, () =>
        {
            ObjectPoolManager.Instance.DeSpawnAll();
            isFade = true;
        });

        Time.timeScale = 0.3f;

        yield return new WaitUntil( () => fadeScreen.IsEnabled);
        uiManager.GetUI<TowerDeathUI>().Enable();
        fadeScreen.Disable();
        Time.timeScale = 0;
    }
}
