using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneUI : BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.PopupBackground;
    public override bool IsEnabled => gameObject.activeInHierarchy;
    
    [SerializeField] private Image slideImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float defaultFadeTime = 0.5f;          
    [SerializeField] private float defaultSlideDuration = 3f;
    [SerializeField] private bool defaultClickToAdvance = true;
    [SerializeField] private KeyCode defaultAdvanceKey = KeyCode.Space;
    [SerializeField] private KeyCode defaultSkipKey = KeyCode.Escape;

    private bool _isPlaying;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (slideImage != null) slideImage.preserveAspect = true;
        gameObject.SetActive(false);
    }

    
    public void Play(List<Sprite> slides,
                     float slideDuration,
                     float fadeSeconds,
                     bool clickToAdvance,
                     KeyCode advanceKey,
                     KeyCode skipKey,
                     Action onFinished)
    {
        if (slides == null || slides.Count == 0)
        {
            onFinished?.Invoke();
            return;
        }

        float useFade = (fadeSeconds > 0f) ? fadeSeconds : defaultFadeTime;
        float useDuration = (slideDuration > 0f) ? slideDuration : defaultSlideDuration;
        bool  useClick     = clickToAdvance;
        KeyCode useAdvance = (advanceKey != KeyCode.None) ? advanceKey : defaultAdvanceKey;
        KeyCode useSkip    = (skipKey    != KeyCode.None) ? skipKey    : defaultSkipKey;

        Enable();
        StopAllCoroutines();
        StartCoroutine(PlayRoutine(slides, useDuration, useFade, useClick, useAdvance, useSkip, onFinished));
    }

    public override void Enable()  => gameObject.SetActive(true);
    public override void Disable() => gameObject.SetActive(false);

    private IEnumerator PlayRoutine(List<Sprite> slides,
        float slideDuration,
        float fadeSeconds,
        bool clickToAdvance,
        KeyCode advanceKey,
        KeyCode skipKey,
        Action onFinished)
    
    // public override void Enable()
    // {
    //     gameObject.SetActive(true);
    // }
    //
    // public override void Disable()
    // {
    //     gameObject.SetActive(false);
    // }

    {
        _isPlaying = true;
        canvasGroup.alpha = 0f;

        for (int i = 0; i < slides.Count; i++)
        {
            slideImage.sprite = slides[i];

          
            yield return Fade(canvasGroup, 0f, 1f, fadeSeconds);

            
            float t = 0f;
            bool proceed = false;
            while (!proceed)
            {
                if (Input.GetKeyDown(skipKey))
                {
                    i = slides.Count - 1; 
                    proceed = true;
                    break;
                }

                if (clickToAdvance && (Input.GetKeyDown(advanceKey) || Input.GetMouseButtonDown(0)))
                {
                    proceed = true;
                    break;
                }

                t += Time.unscaledDeltaTime;
                if (t >= slideDuration)
                {
                    proceed = true;
                    break;
                }

                yield return null;
            }

          
            yield return Fade(canvasGroup, 1f, 0f, fadeSeconds);
        }

        _isPlaying = false;
        Disable();
        onFinished?.Invoke();
    }

    private IEnumerator Fade(CanvasGroup cg, float from, float to, float time)
    {
        if (cg == null)
            yield break;

        if (time <= 0f)
        {
            cg.alpha = to;
            yield break;                 
        }

        float elapsed = 0f;
        cg.alpha = from;
        while (elapsed < time)
        {
            elapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / time);
            yield return null;
        }
        cg.alpha = to;
    }
}
