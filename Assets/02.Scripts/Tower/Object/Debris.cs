using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class Debris : MonoBehaviour
{
    [Header("움직임 세팅")]
    //파편이 튈 때 최소/최대 이동거리
    [SerializeField] float minDistance = 0.3f;
    [SerializeField] float maxDistance = 0.8f;
    //파편이 튈 때 최소/최대 높이
    [SerializeField] float minHeight = 0.3f;
    [SerializeField] float maxHeight = 0.8f;
    //파편이 떨어지는 애니메이션의 최소/최대 시간
    [SerializeField] float minAnimDuration = 0.2f;
    [SerializeField] float maxAnimDuration = 0.4f;

    [SerializeField] float targetYposition = -0.5f;

    [Header("회전 세팅")]
    [SerializeField] float minAngle = -360f;
    [SerializeField] float maxAngle = 360f;

    [SerializeField] float minRotationDuration = 0.5f;
    [SerializeField] float maxRotationDuration = 1.5f;

    [Header("부서지는 시간")]
    [SerializeField] bool dontDestroy = false;
    [SerializeField] float minDestroyDelay = 0.3f;
    [SerializeField] float maxDestroyDelay = 1.0f;


    class DebrisPiece
    {
        public Transform trans;
        public Tween moveTween;
        public Tween rotateTween;
        public bool isSleeping;
    }

    void Start()
    {
        List<Transform> debris = new List<Transform>();
        foreach (Transform child in transform)
        {
            debris.Add(child);
        }

        foreach (var child in debris)
        {
            var piece = new DebrisPiece();
            piece.trans = child;
            piece.isSleeping = false;

            PlayDebrisAnimations(piece);
        }
    }

    void PlayDebrisAnimations(DebrisPiece piece)
    {
        DOTween.Kill(piece.trans);

        float distance = Random.Range(minDistance, maxDistance);
        float height = Random.Range(minHeight, maxHeight);
        float animationDuration = Random.Range(minAnimDuration, maxAnimDuration);

        Vector3 startPos = piece.trans.localPosition;
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector3 targetPos = startPos + new Vector3(randomDir.x, randomDir.y, 0) * distance;
        targetPos.y += targetYposition;

        piece.moveTween = DOTween.Sequence()
            .Append(piece.trans.DOLocalMoveX(targetPos.x, animationDuration).SetEase(Ease.Linear))
            .Join(
                piece.trans.DOLocalMoveY(startPos.y + height, animationDuration * 0.5f).SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    piece.trans.DOLocalMoveY(targetPos.y, animationDuration * 0.5f).SetEase(Ease.InQuad);
                })
            )
            .OnComplete(() =>
            {
                Vector3 finalPos = piece.trans.localPosition;
                finalPos.y += Random.Range(0f, -0.3f);
                piece.trans.localPosition = finalPos;

                piece.isSleeping = true;
                if (piece.rotateTween != null && piece.rotateTween.IsActive())
                {
                    piece.rotateTween.Kill();
                }

                float randomDelay = Random.Range(minDestroyDelay, maxDestroyDelay);
                if (!dontDestroy)
                {
                    Destroy(piece.trans.gameObject, randomDelay);
                }
            })
            .SetAutoKill(true)
            .SetLink(piece.trans.gameObject);

        float angle = Random.Range(minAngle, maxAngle);
        float duration = Random.Range(minRotationDuration, maxRotationDuration);

        piece.rotateTween = piece.trans.DOLocalRotate(new Vector3(0, 0, angle), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental)
            .SetAutoKill(false)
            .SetLink(piece.trans.gameObject);
    }
}
