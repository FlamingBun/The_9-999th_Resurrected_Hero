using System.Collections.Generic;
using DG.Tweening;

public class EndingUI : IntroUI
{
    private List<string> _loadScenes;
    private List<string> _unloadScenes;
    
    protected override void Awake()
    {
        base.Awake();
        
        _loadScenes = new()
        {
            Constant.TownSceneName
        };
        
        _unloadScenes = new()
        {
            Constant.TowerSceneName,
            Constant.FloorSceneName
        };
    }

    public override void Enable()
    {
        base.Enable();
        
        AudioManager.Instance.Play("StartBGM");
    }
    
    
    protected override void OnClickNextButton()
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
            SceneLoadManager.Instance.LoadAddtiveScenes(_loadScenes, _unloadScenes).OnCompleteLoad(Disable);
            return;
        }
        
        CutFadeIn(++currentCutIndex);
    }

    public override void Disable()
    {
        if (player != null)
            player.InputController.ToggleInput(true);
        
        AudioManager.Instance.StopLoopAudio("StartBGM");
    }
}
