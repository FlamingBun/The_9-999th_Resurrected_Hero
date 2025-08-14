using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Soul : MonoBehaviour
{
    [SerializeField] private float dist;

    private PlayerController _player;
    private TowerHUDUI _towerHUD;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sortingOrder = 0;
    }

    public void Spawn(PlayerController player)
    {
        _player = player;

        _towerHUD = UIManager.Instance.GetUI<TowerHUDUI>();

        StartCoroutine(MovementSequence());
    }
    
    private IEnumerator MovementSequence()
    {
        Vector3 startPosition = transform.position;
        Vector3 directionFromPlayer = (startPosition - _player.transform.position).normalized;
        Vector3 repelTargetPosition = startPosition + directionFromPlayer * dist;


        bool isComplete = false;
    
        transform.DOMove(repelTargetPosition, 0.5f).SetEase(Ease.OutQuad)
            .OnComplete(() => isComplete = true);

        yield return new WaitUntil(() => isComplete);

        Vector3 startPos = transform.position;
        
        float time = 0.1f;
        float timer = 0;
        
        while (timer <= time)
        {
            transform.position = Vector3.Lerp(startPos,  _player.transform.position, timer / time);
            
            timer += Time.deltaTime;

            yield return null;
        }
        
        Sequence sequence = DOTween.Sequence();
        
        _spriteRenderer.material.SetFloat("_HitEffectBlend", 0f);
        
        _spriteRenderer.sortingOrder = 100;
        
        sequence.Append(transform.DOScale(1.5f, 0.1f)).SetEase(Ease.OutQuad);
        sequence.Join(_spriteRenderer.material.DOFloat(1f, "_HitEffectBlend", 0.1f)).SetEase(Ease.OutQuad).OnComplete(()=>Destroy(gameObject));
    }

    private void OnDestroy()
    {
        AudioManager.Instance.Play("SoulDrainClip");

        _player.PlayerInstance.ModifySoul(1);
        _towerHUD.UpdateSoulText();
    }
}
