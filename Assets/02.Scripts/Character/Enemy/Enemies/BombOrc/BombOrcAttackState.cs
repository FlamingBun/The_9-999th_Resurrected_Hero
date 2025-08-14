using DG.Tweening;
using UnityEngine;

public class BombOrcAttackState:RangeEnemyAttackState
{
    private SpriteRenderer _indicator;

    public BombOrcAttackState(EnemyController enemyController, EnemyStateMachine stateMachine) : base(enemyController, stateMachine)
    {
        BombOrcController bombOrcController = enemyController as BombOrcController;
        
        ObjectPoolManager.Instance.CreatePool(rangeAttackDataSO.rangeAttackData.projectileData.prefab.GetComponent<EnemyBomb>(), EnemyConstant.projectileDefaultCount);
        ObjectPoolManager.Instance.CreatePool(rangeAttackDataSO.rangeAttackData.smokeEffect.GetComponent<VFXHandler>());
        
        _indicator = bombOrcController.InstantiateIndicator(rangeAttackDataSO.indicator);
        _indicator.gameObject.SetActive(false);
    }

    protected override void OnHit()
    {
        SpawnAttackEffect();
        
        moveHandler.MakeMove(controller.Rigid, attackDirection, rangeAttackDataSO.attackMoveDistance);
        StartAndTrackCoroutine(rangeAttackHandler.FireBombRoutine(controller, rangeAttackDataSO, attackDirection, ShowIndicator ,() => { isAttack = false;}, _indicator.gameObject));
    }

    private void ShowIndicator(Vector3 indicatorPosition)
    {
        _indicator.transform.position = indicatorPosition;
        _indicator.gameObject.SetActive(true);
        
        Color color = _indicator.color;
        color.a = 0f;

        _indicator.color = color;

        _indicator.DOFade(1f, rangeAttackDataSO.rangeAttackData.bombFlightTime*0.8f).SetEase(Ease.OutQuart).OnComplete(() => { _indicator.gameObject.SetActive(false);});
    }
}
