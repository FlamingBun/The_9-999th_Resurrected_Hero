using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TowerRoomShopUI : BaseUI
{
    public override bool IsEnabled => gameObject.activeSelf;
    
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI priceText;
    private CanvasGroup _canvasGroup;

    public override CanvasLayer Layer => CanvasLayer.HUD;
    public float fadeEffectDuration = 0.1f;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public override void Enable()
    {
        gameObject.SetActive(true);

        _canvasGroup.alpha = 0;
        _canvasGroup.DOKill();
        _canvasGroup.DOFade(1, 0.1f);
    }

    public override void Disable()
    {
        _canvasGroup.DOKill();
        _canvasGroup.DOFade(0, 0.1f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void SetItem(ItemInstance itemInstance)
    {
        //UI의 이름과 가격을 SO에 맞게 변경
        itemNameText.text = itemInstance.Data.itemName;
        descriptionText.text = itemInstance.Data.description;
        priceText.text = itemInstance.price.ToString();
    }
    
    
    private IEnumerator FadeInCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < fadeEffectDuration)
        {
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeEffectDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _canvasGroup.alpha = 1f;
    }
    

    private IEnumerator FadeOutCoroutine(float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float startAlpha = _canvasGroup.alpha;

        while (elapsed < fadeEffectDuration)
        {
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeEffectDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

}
