using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class IntroUI : BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.PopupBackground;

    public override bool IsEnabled => gameObject.activeInHierarchy;

    
    [SerializeField] private List<Image> cutImages = new();
    
    [SerializeField] private Button nextButton;

    [SerializeField] private float cutFadeInTime = 0.5f;
    

    protected PlayerController player;
    
    protected Tween cutTween;
    
    protected Image currentCutImage;
    
    protected int currentCutIndex;
    protected int cutCount;
    
    protected bool isLoadNextScene;

    protected virtual void Awake()
    {
        isLoadNextScene = false;
        
        currentCutIndex = 0;
        cutCount = cutImages.Count;

        foreach (Image cut in cutImages)
        {
            cut.color = new Color(1f, 1f, 1f, 0f);
        }

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(OnClickNextButton);
    }

    private void Start()
    {
        CutFadeIn(currentCutIndex);
    }

    public void Init(PlayerController player)
    {
        this.player = player;
    }
    
    protected virtual void OnClickNextButton()
    {
        if (isLoadNextScene) return;
        
        AudioManager.Instance.Play(key: "ButtonHoverClip");
        
        if (cutTween != null)
        {
            cutTween.Kill();
            currentCutImage.DOFade(1f, 0f).OnComplete(()=>cutTween = null);
            return;
        }
        
        if (currentCutIndex >= cutCount-1)
        {
            isLoadNextScene = true;
            var loadScenes = new List<string> { Constant.TownSceneName };
            SceneLoadManager.Instance.LoadAddtiveScenes(loadScenes).OnCompleteLoad(Disable);        
            return;
        }
        
        CutFadeIn(++currentCutIndex);
    }

    protected void CutFadeIn(int index)
    {
        currentCutImage = cutImages[index];
        cutTween = currentCutImage.DOFade(1f, cutFadeInTime).SetEase(Ease.InSine).OnComplete(()=>cutTween = null);
    }
    
    public override void Enable()
    {
        if (player != null)
        {
            player.InputController.ToggleInput(false);
        }

        gameObject.SetActive(true);
    }

    public override void Disable()
    {
        if (player != null)
            player.InputController.ToggleInput(true);
        
        AudioManager.Instance.StopLoopAudio("StartBGM");
        
        gameObject.SetActive(false);
    }
}
