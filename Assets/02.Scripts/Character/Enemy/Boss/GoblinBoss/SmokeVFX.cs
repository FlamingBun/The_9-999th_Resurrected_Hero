using UnityEngine;

public class SmokeVFX : MonoBehaviour
{
    private Animator _animator;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        transform.parent = null;
        gameObject.SetActive(false);
    }

    public void Spawn()
    {
        gameObject.SetActive(true);
        _animator.SetTrigger(EnemyAnimationHashes.SpawnSmoke);
    }

    public void OnSmokeAnimationEnd()
    {
        gameObject.SetActive(false);
    }
}