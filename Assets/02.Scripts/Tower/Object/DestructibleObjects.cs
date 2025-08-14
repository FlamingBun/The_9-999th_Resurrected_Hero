using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DestructibleObjects : MonoBehaviour, IDamageable
{
    public bool CanDamageable => true;

    [SerializeField] GameObject destructablePrefab;
    [SerializeField] GameObject goldPrefab;

    private TowerDropObjectHandler _dropObjectHandler;


    private void Awake()
    {
        _dropObjectHandler = GetComponent<TowerDropObjectHandler>();
    }


    private void ExplodeThisGameObject()
    {
        AudioManager.Instance.Play("ObjectBreakClip");

        GameObject destructable = Instantiate(destructablePrefab, transform.position, Quaternion.identity);

        Scene currentScene = gameObject.scene;
        SceneManager.MoveGameObjectToScene(destructable, currentScene);

        if (_dropObjectHandler != null)
        {
            _dropObjectHandler.SpawnGold();
        }
        
        Destroy(gameObject);
    }


    public void TakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.Attacker.TryGetComponent(out PlayerController player))
        {
            ExplodeThisGameObject();
        }
    }
}
