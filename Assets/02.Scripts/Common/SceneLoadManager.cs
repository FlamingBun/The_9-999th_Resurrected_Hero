using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoadManager : Singleton<SceneLoadManager>
{
    private FadeScreenUI FadeScreenUI
    {
        get
        {
            if (_fadeScreenUI == null)
            {
                _fadeScreenUI = UIManager.Instance.GetUI<FadeScreenUI>();
                
                if (_fadeScreenUI == null)
                {
                    Debug.LogError("LoadingScreenUI를 찾을 수 없음");
                }
            }

            return _fadeScreenUI;
        }
    }

    private event UnityAction _onLoadCompleteCallback;

    private FadeScreenUI _fadeScreenUI;
    private bool _isLoading = false;


    public SceneLoadManager LoadAddtiveScenes(List<string> scenesToLoad, List<string> scenesToUnload = null)
    {
        if (_isLoading)
        {
            Debug.LogWarning("이미 씬 로딩이 진행 중입니다.");
            
            return this;
        }

        StartCoroutine(SceneLoadRoutine(scenesToLoad, scenesToUnload));
        
        return this;
    }
    
    

    public SceneLoadManager OnCompleteLoad(UnityAction callBack)
    {
        _onLoadCompleteCallback = callBack;
        
        return this;
    }
    

    private IEnumerator SceneLoadRoutine(List<string> scenesToLoad, List<string> scenesToUnload)
    {
        _isLoading = true;

        bool isFade = false;
        
        FadeScreenUI.Enable();
        FadeScreenUI.FadeIn(1f, () =>
        {
            isFade = true;
        });
        
        yield return new WaitUntil(() => isFade);
        yield return new WaitForSecondsRealtime(0.5f);

        var operations = new List<AsyncOperation>();

        if (scenesToUnload != null)
        {
            foreach (var sceneName in scenesToUnload)
            {
                if (SceneManager.GetSceneByName(sceneName).isLoaded)
                {
                    operations.Add(SceneManager.UnloadSceneAsync(sceneName));
                }
            }
        }

        foreach (var sceneName in scenesToLoad)
        {
            operations.Add(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive));
        }

        while (operations.Any(op => !op.isDone))
        {
            float totalProgress = operations.Sum(op => op.progress);
            float averageProgress = totalProgress / operations.Count;
            yield return null; 
        }
        
        _onLoadCompleteCallback?.Invoke();
        _onLoadCompleteCallback = null;
        
        FadeScreenUI.FadeOut(1f, () =>
        {
            FadeScreenUI.Disable();
            _isLoading = false;
        });
    }
}
