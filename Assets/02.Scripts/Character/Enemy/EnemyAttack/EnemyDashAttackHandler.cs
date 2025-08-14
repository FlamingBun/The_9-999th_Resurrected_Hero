using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;
using Random = UnityEngine.Random;

public class EnemyDashAttackHandler
{
    private bool _isPlayerHitInterval;


    public IEnumerator DashRoutine(EnemyController controller, Vector2 direction, EnemyDashAttackDataSO attackData, Action callback = null)
    {
        EnemyDashAttackData dashAttackData = attackData.dashAttackData;

        bool isMoved = false;

        // 돌진 사운드 재생
        AudioManager.Instance.Play("MonsterChargeClip");

        if (!DOTween.IsTweening(controller.Rigid.GetInstanceID()))
        {
            DOTween.Kill(controller.Rigid.GetInstanceID());
        }
        
        controller.MoveHandler.AILerp.enabled = false;
        
        controller.Rigid.DOMove(controller.Rigid.position + (direction * dashAttackData.dashRange), dashAttackData.dashDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            isMoved = true;
            controller.MoveHandler.AILerp.enabled = true;
        }).SetId(controller.Rigid.GetInstanceID());
        
        
        while (!isMoved)
        {
            Collider2D[] results = new Collider2D[1];
            if (Physics2D.OverlapBoxNonAlloc(controller.transform.position + (Vector3)dashAttackData.dashBoxOffset, dashAttackData.dashBoxSize, 0f, results, LayerMask.GetMask("Player", "Obstacle")) > 0)
            {
                Collider2D hit = results[0];

                if (hit != null)
                {
                    if (hit.TryGetComponent(out PlayerController player))
                    {
                        if (player.CanDamageable)
                        {
                            VFXHandler impact = ObjectPoolManager.Instance.Spawn<VFXHandler>(attackData.impact.name);

                            impact.transform.position = hit.transform.position;

                            impact.Init(attackData.impact.name);

                            Vector3 hitDir = hit.transform.position - controller.transform.position;
                        
                            DamageInfo damageInfo = new DamageInfo(controller.gameObject, controller.GetFinalDamage(attackData.damageMultiplier), hit.transform.position, hitDir, 0);
                        
                            player.TakeDamage(damageInfo);

                            controller.MoveHandler.MakeMove(player.Rigid, direction, attackData.knockbackPower);
                        }
                    }
                }
                callback?.Invoke();
                yield break;
            }

            yield return null;
        }
        callback?.Invoke();
    }

    public IEnumerator WhirlWindRoutine(EnemyController controller, EnemyDashAttackDataSO attackData, Action callback = null)
    {
        var whirlWindData = attackData.whirlWindData;
        var dashData = attackData.dashAttackData;
        
        Vector2 direction = controller.MoveHandler.GetDirection();
        Vector2 yOffset = new Vector2(0f, whirlWindData.yOffset);
        
        float currentTime = 0f;
        
        _isPlayerHitInterval = false;

        while (currentTime < whirlWindData.whirlWindDuration)
        {
            currentTime += Time.fixedDeltaTime;

            //bool isHitObstacle = TryReflectIfObstacle(controller, whirlWindData, ref direction, yOffset);
            bool isHitObstacle = TryFollowIfObstacle(controller, whirlWindData, ref direction, yOffset);
            
            if (!_isPlayerHitInterval)
            {
                TryHitPlayer(controller, attackData, dashData, direction, yOffset);
            }

            float modifier = isHitObstacle ? 4f : 1f;
            float moveDistance = whirlWindData.moveSpeed * Time.fixedDeltaTime * modifier;

            controller.MoveHandler.MakeMove(controller.Rigid, direction, moveDistance, Time.fixedDeltaTime * modifier);

            yield return new WaitForSeconds(Time.fixedDeltaTime * modifier);
        }

        callback?.Invoke();
    }
    
    private bool TryFollowIfObstacle(EnemyController controller, EnemyWhirlWindAttackData whirlData, ref Vector2 direction, Vector2 yOffset)
    {
        Collider2D[] circleHits = new Collider2D[1];
        int hitCount = Physics2D.OverlapCircleNonAlloc(controller.Rigid.position + yOffset, whirlData.obstacleCheckDistance, circleHits, LayerMask.GetMask("Obstacle"));

        if (hitCount == 0) return false;

        Vector2 collisionPoint = circleHits[0].ClosestPoint(controller.Rigid.position + yOffset);
        Vector2 toCollision = collisionPoint - (controller.Rigid.position + yOffset);

        SpawnSmoke(collisionPoint, whirlData.whirlWindSmoke.name, controller);
        
        RaycastHit2D hit = Physics2D.Raycast(controller.Rigid.position + yOffset, toCollision, whirlData.obstacleCheckDistance * 10f, LayerMask.GetMask("Obstacle"));
        if (hit.collider == null) return false;

        direction = ((Vector2)controller.Player.transform.position - hit.point).normalized; 
            
        return true;
    }
    
    
    private bool TryReflectIfObstacle(EnemyController controller, EnemyWhirlWindAttackData whirlData, ref Vector2 direction, Vector2 yOffset)
    {
        Collider2D[] circleHits = new Collider2D[1];
        int hitCount = Physics2D.OverlapCircleNonAlloc(controller.Rigid.position + yOffset, whirlData.obstacleCheckDistance, circleHits, LayerMask.GetMask("Obstacle"));

        if (hitCount == 0) return false;

        Vector2 collisionPoint = circleHits[0].ClosestPoint(controller.Rigid.position + yOffset);
        Vector2 toCollision = collisionPoint - (controller.Rigid.position + yOffset);

        SpawnSmoke(collisionPoint, whirlData.whirlWindSmoke.name, controller);
        
        RaycastHit2D hit = Physics2D.Raycast(controller.Rigid.position + yOffset, toCollision, whirlData.obstacleCheckDistance * 10f, LayerMask.GetMask("Obstacle"));
        if (hit.collider == null) return false;

        Vector2 reflected = Vector2.Reflect(toCollision, hit.normal).normalized;
        float extraAngle = Random.value < 0.5f ? 20f : -20f;

        int maxTry = 18;

        for (int i = 0; i < maxTry; i++)
        {
            if (Physics2D.Raycast(controller.Rigid.position + yOffset, reflected, whirlData.obstacleCheckDistance * 2f, LayerMask.GetMask("Obstacle")).collider == null)
                break;

            reflected = Quaternion.Euler(0, 0, extraAngle) * reflected;
        }

        if (Vector2.Angle(direction, reflected) > 160f)
        {
            reflected = Quaternion.Euler(0, 0, extraAngle) * reflected;
        }

        direction = reflected;
        return true;
    }

    private void TryHitPlayer(EnemyController controller, EnemyDashAttackDataSO attackData, EnemyDashAttackData dashData, Vector2 direction, Vector2 yOffset)
    {
        Collider2D[] results = new Collider2D[1];
        if (Physics2D.OverlapBoxNonAlloc(controller.Rigid.position + yOffset, dashData.dashBoxSize, 0f, results, LayerMask.GetMask("Player")) == 0)
            return;

        Collider2D hitCollider = results[0];
        if (hitCollider == null || !hitCollider.TryGetComponent(out PlayerController player))
            return;
        
        VFXHandler impact = ObjectPoolManager.Instance.Spawn<VFXHandler>(attackData.impact.name);
        impact.transform.position = player.transform.position;
        impact.Init(attackData.impact.name);
        
        Vector3 hitDir = hitCollider.transform.position - controller.transform.position;
                        
        DamageInfo damageInfo = new DamageInfo(controller.gameObject, controller.GetFinalDamage(attackData.damageMultiplier), hitCollider.transform.position, hitDir, 0);

        if (player.CanDamageable)
        {
            player.TakeDamage(damageInfo);
        }
        
        float randomAngle = Random.Range(-60f, 60f);
        Vector2 knockbackDir = Quaternion.Euler(0, 0, randomAngle) * direction;
        controller.MoveHandler.MakeMove(player.Rigid, knockbackDir, attackData.knockbackPower);
        
        controller.StartCoroutine(HitIntervalRoutine(attackData.whirlWindData.hitInterval));
    }

    private void SpawnSmoke(Vector3 position, string key, EnemyController controller)
    {
        VFXHandler smoke = ObjectPoolManager.Instance.Spawn<VFXHandler>(key);
                
        smoke.transform.position = position;
                
        smoke.Init(key);
    }


    private IEnumerator HitIntervalRoutine(float interval)
    {
        _isPlayerHitInterval = true;

        yield return new WaitForSeconds(interval);

        _isPlayerHitInterval = false;
    }
}
