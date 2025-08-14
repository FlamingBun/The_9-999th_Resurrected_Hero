using UnityEngine;
using UnityEngine.SceneManagement;

public class TowerDropObjectHandler : MonoBehaviour
{
    [SerializeField] private Gold goldPrefab;
    [SerializeField] private Soul soulPrefab;


    private PlayerController _player;
    private FloorManager _floorManager;
    
    private float _goldDropChance = 0.1f;
    private float _soulDropChance = 1f;


    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerController>();
        _floorManager = FindAnyObjectByType<FloorManager>();
    }


    public void SpawnGold()
    {
        float randNum = Random.Range(0f, 1f);
        
        if (randNum < _goldDropChance)
        {
            var gold = Instantiate(goldPrefab);
            gold.transform.position = transform.position;
            
            SceneManager.MoveGameObjectToScene(gold.gameObject,  gameObject.scene);
        }
    }

    public void SpawnSoul()
    {
        float randNum = Random.Range(0f, 1f);

        var soulDropChance = _soulDropChance * _floorManager.GetFeatureMultiplier(TowerCurseType.SoulDrop);
        
        if (randNum < soulDropChance)
        {
            var soul = Instantiate(soulPrefab);
            soul.transform.position = transform.position;
            soul.Spawn(_player);

            SceneManager.MoveGameObjectToScene(soul.gameObject,  gameObject.scene);
        }
    }


}
