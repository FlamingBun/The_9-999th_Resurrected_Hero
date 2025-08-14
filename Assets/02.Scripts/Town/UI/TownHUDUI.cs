using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class TownHUDUI : BaseUI
{
    public override CanvasLayer Layer => CanvasLayer.HUD;
    public override bool IsEnabled => gameObject.activeSelf;

    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI tutorialGuideText;

    private TownTutorialUITextInfo _tutorialUITextInfo;
    private TownHUDUIQuestInfo _questInfo;

    
    private void Awake()
    {
        _tutorialUITextInfo = GetComponentInChildren<TownTutorialUITextInfo>();
    }

    
    public override void Enable()
    {
        gameObject.SetActive(true);
    }

    public override void Disable()
    {
        gameObject.SetActive(false);
    }

    public void EnableTutorialText(string text)
    {
        _tutorialUITextInfo.Enable(text);
    }

    public void DisableTutorialText()
    {
        _tutorialUITextInfo.Disable();
    }

}
