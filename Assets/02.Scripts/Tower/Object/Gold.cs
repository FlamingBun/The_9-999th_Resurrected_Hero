using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gold : MonoBehaviour
{
    [Header("튕김 설정")]
    [SerializeField] float minJumpHeight = 0.3f;
    [SerializeField] float maxJumpHeight =0.5f;
    [SerializeField] float minJumpDistance = 0.3f;
    [SerializeField] float maxJumpDistance = 0.6f;
    [SerializeField] float minJumpDuration = 0.2f;
    [SerializeField] float maxJumpDuration = 0.4f;
    [SerializeField] float targetYposition = -1f;

    private SpriteRenderer _spriteRenderer;
    private Sequence _sequence;
    
    private void Awake()
    {
        _spriteRenderer =  GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.sortingOrder = 0;
        _spriteRenderer.material.SetFloat("_HitEffectBlend", 0f);
        _spriteRenderer.material.SetFloat("_Glow", 0f);
        transform.localScale = Vector3.one;
    }

    private void Start()
    {
        Bounce();
    }

    private void OnDestroy()
    {
        _sequence.Kill();
    }

    private void Bounce()
    {
        float height = Random.Range(minJumpHeight, maxJumpHeight);
        float distance = Random.Range(minJumpDistance, maxJumpDistance);
        float duration = Random.Range(minJumpDuration, maxJumpDuration);

        float upDuration = duration * 0.5f;
        float downDuration = duration * 0.5f;

        Vector3 startPos = transform.localPosition;
        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 targetPos = startPos + new Vector3(dir.x, dir.y, 0f) * distance;
        targetPos.y += targetYposition;

        Sequence bounceSeq = DOTween.Sequence()
            .Append(transform.DOLocalMoveX(targetPos.x, duration).SetEase(Ease.Linear))
            .Join(
                transform.DOLocalMoveY(startPos.y + height, upDuration).SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        transform.DOLocalMoveY(targetPos.y, downDuration).SetEase(Ease.InQuad);

                        transform.DOLocalRotate(new Vector3(0, 0, -90f), downDuration, RotateMode.Fast)
                            .SetEase(Ease.Linear)
                            .SetAutoKill(true)
                            .SetLink(gameObject);
                    })
            )
            .SetAutoKill(true)
            .SetLink(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.PlayerInstance.ModifyGold(1);

            UIManager.Instance.GetUI<StatusHUDUI>().UpdateGoldText();
            AudioManager.Instance.Play("DropCoinClip");

            _sequence = DOTween.Sequence();
        
            _spriteRenderer.material.SetFloat("_HitEffectBlend", 0f);
            _spriteRenderer.material.SetFloat("_Glow", 5f);
            
            _spriteRenderer.sortingOrder = 100;

            Color color = _spriteRenderer.color;
            
            color.a = 0f;
            
            _spriteRenderer.color = color;
            
            _sequence.Append(_spriteRenderer.DOFade(1f, 0.1f)).SetEase(Ease.OutQuad);
            _sequence.Join(_spriteRenderer.material.DOFloat(1f, "_HitEffectBlend", 0.1f)).SetEase(Ease.OutQuad).OnComplete(()=>
            {
                Destroy(gameObject);
            });
        }
    }
}