using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TownTutorialUITextInfo : MonoBehaviour
{
    [SerializeField] private RectTransform hidePoint;
    [SerializeField] private RectTransform showPoint;
    
    [SerializeField] private TextMeshProUGUI infoText;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        
        infoText.text = "";
        infoText.transform.position = hidePoint.position;
    }


    public void Enable(string enableText)
    {
        gameObject.SetActive(true);
        
        infoText.text = enableText;

        _canvasGroup.alpha = 0;
        _canvasGroup.DOFade(1, 0.33f).SetEase(Ease.OutQuad);

        infoText.rectTransform.position = hidePoint.position;
        infoText.rectTransform.DOMove(showPoint.position, 0.33f).SetEase(Ease.OutQuad);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void SetText(string targetText)
    {
        infoText.text = targetText;
    }
    
    
    
}
