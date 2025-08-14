using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour,IPoolable
{
    [SerializeField] private bool isRandomDirection;
    
    [SerializeField]private Vector3 moveVector;
    
    [SerializeField]private float disappearTime;
    [SerializeField]private float alphaFadeOutTime;

    [SerializeField] private float scaleUpModifier;
    [SerializeField] private float scaleDownModifier;
    
    [SerializeField] private float normalFontSize;
    [SerializeField] private float criticalFontSize;
    
    private Tween _tween;
    
    private TextMeshProUGUI _text;
    
    private RectTransform _rectTransform;
    private CharacterBaseController _controller;
    private Camera _worldCamera;
    
    private float _baseScale;
    private Vector3 _spawnControllerPos;
    
    private void Awake()
    {
        _text = transform.GetComponentInChildren<TextMeshProUGUI>();
        _rectTransform = GetComponent<RectTransform>();
        _baseScale= transform.localScale.x;
    }

    private void Update()
    {
        if (_worldCamera != null && _controller != null)
        {
            Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(_worldCamera, _spawnControllerPos);
            _rectTransform.position = screenPosition; 
        }
    }

    public void OpenPopup(string poolKey, CharacterBaseController controller, int damageAmount, bool isEnemy, bool isCriticalHit, Camera worldCamera)
    {
        _text.transform.localScale = Vector3.zero;

        _controller = controller;
        _worldCamera = worldCamera;

        _spawnControllerPos = _controller.transform.position;
        
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(_worldCamera, _controller.transform.position);
        
        _rectTransform = GetComponent<RectTransform>();
        
        Vector2 anchorPosition = _rectTransform.anchoredPosition + screenPosition;
        
        _rectTransform.anchoredPosition = anchorPosition;
        
        _text.SetText($"{damageAmount}");

        if (isEnemy)
        {
            if (!isCriticalHit)
            {
                _text.fontSize = normalFontSize;
                _text.color = Color.white;
            }
            else
            {
                _text.fontSize = criticalFontSize;
                _text.color = Color.yellow;
            }
        }
        else
        {
            _text.fontSize = normalFontSize;
            _text.color = new Color(1f, 0.45f, 0.35f);
        }

        PlayPopupTween(poolKey);
    }

    private void PlayPopupTween(string poolKey)
    {
        Sequence sequence = DOTween.Sequence();

        float halfOfDisappearTime = disappearTime * 0.5f;

        Vector3 moveUpVector = moveVector;
        Vector3 moveDownVector;
        
        if (isRandomDirection)
        {
            float angle = Random.Range(30f, 150f);
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            moveUpVector = direction * moveVector.magnitude;
        }

        moveDownVector = moveUpVector;
        moveDownVector.x = moveDownVector.x * 2f; 
        moveDownVector.y = -moveDownVector.y/2f;
        
        RectTransform rectTransform = _text.GetComponent<RectTransform>();

        sequence.Append(rectTransform.DOAnchorPos(rectTransform.anchoredPosition + (Vector2)moveUpVector, halfOfDisappearTime).SetEase(Ease.OutCubic));
        sequence.Join(rectTransform.DOScale(Vector3.one * _baseScale * scaleUpModifier, halfOfDisappearTime/2f).SetEase(Ease.OutQuad));
        sequence.AppendInterval(disappearTime*2f);
        sequence.Append(rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y - 40, disappearTime*2f).SetEase(Ease.InQuad));
        sequence.Join(DOTween.ToAlpha(
            () => _text.color,
            c => _text.color = c,
            0f,
            halfOfDisappearTime * 3f
        ));

        sequence.OnComplete(() =>
        {
            ObjectPoolManager.Instance.Return(poolKey, this);
        });

        _tween = sequence;
    }

    public void OnSpawn()
    {
        _tween?.Kill();
    }

    public void OnDespawn()
    {
        _tween?.Kill();
    }
}
