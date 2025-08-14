using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomEnemyEventHandler : MonoBehaviour, IRoomEvent
{
    public int Order => 0;
    public bool IsCleared => 
        _isStart && _spawnedEnemies.Count == 0;

    [SerializeField] private RoomEnemySpawnEventData[] enemySpawnEventDatas;
    
    private RoomController _room;
    private PlayerController _player;
    private RoomEnemySpawnEventData _selectEventData;
    private List<EnemyController> _spawnedEnemies = new();
    private List<Vector2Int> _spawnedPosList = new();
    private List<Vector2Int> _spawnablePointPos = new();
    
    private bool _isStart;
    private int _curSpawnCount;
    private int _maxSpawnCount;
    private const float MinSpawnDistanceToPlayer = 3;
    
    // TESTCODE
    // private void Update()
    // {
    //     //테스트
    //     if (Input.GetMouseButton(2))
    //     {
    //         if (_spawnedEnemies != null)
    //         {
    //             foreach (var enemy in _spawnedEnemies)
    //             {
    //                 Destroy(enemy.gameObject);
    //             }
    //             
    //             _spawnedEnemies.Clear();
    //         }
    //     }
    // }

    public void Init(RoomController room)
    {
        _room = room;

        foreach (Vector3Int cellPos in  room.EnemySpawnTilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile =  room.EnemySpawnTilemap.GetTile(cellPos);
            
            if (tile == null) continue;

            _spawnablePointPos.Add((Vector2Int)cellPos);
        }
    }


    public void StartEvent()
    {
        _isStart = true;
        
        _player = _room.FloorManager.Player;
        
        _spawnedEnemies = new();
        
        
        if (enemySpawnEventDatas.Length == 0) return;

        enemySpawnEventDatas = enemySpawnEventDatas.OrderBy(_ => Random.value).ToArray();

        _selectEventData = enemySpawnEventDatas[0];
        
        _maxSpawnCount = (int)(_selectEventData.maxSpawnCount * _room.FloorManager.GetFeatureMultiplier(TowerCurseType.EnemyCount));

        
        for (int i = 0; i < _selectEventData.limitSpawnCount; i++)
        {
            if (i < _maxSpawnCount)
            {
                SpawnRandomEnemies();
            }
        }
    }

    public void SpawnRandomEnemies()
    {
        if (_spawnablePointPos.Count == 0) return;
        if (_selectEventData.spawnEnemyDatas.Count == 0) return;
        
        
        var shuffledSpawnPoints = _spawnablePointPos.OrderBy(_ => Random.value).ToArray();

        for (int i = 0; i < shuffledSpawnPoints.Length; i++)
        {
            if (_spawnedPosList.Count == _spawnablePointPos.Count)
            {
                _spawnedPosList.Clear();
            }
            
            Vector2Int point = new Vector2Int(shuffledSpawnPoints[i].x, shuffledSpawnPoints[i].y);
            
            if(!_spawnedPosList.Contains(point) && ((Vector2)_player.transform.position - point).magnitude > MinSpawnDistanceToPlayer)
            {
                int randNum = Random.Range(0, _selectEventData.spawnEnemyDatas.Count);
                
                EnemyDataSO enemyData = _selectEventData.spawnEnemyDatas[randNum];
                EnemyController spawnEnemy = Instantiate(enemyData.enemyPrefab, transform);

                spawnEnemy.transform.position = new Vector3(point.x, point.y) + _room.RoomBounds.center;
                
                if (enemyData.isBoss == true)
                {
                    // TODO : 임시
                    GoblinBossController goblinBossController = spawnEnemy as GoblinBossController;
                    goblinBossController.roomBounds = _room.RoomBounds;
                }
         
                spawnEnemy.Init(enemyData, _room.FloorManager);
                spawnEnemy.OnDeath += () => CheckRemainEnemy(spawnEnemy);

                _curSpawnCount++;
                _spawnedEnemies.Add(spawnEnemy);

                return;
            }
        }
    }

    
    private void CheckRemainEnemy(EnemyController deathEnemy)
    {
        if (_spawnedEnemies.Count > 0)
        {
            _spawnedEnemies.Remove(deathEnemy);

            if (_curSpawnCount < _maxSpawnCount)
            {
                SpawnRandomEnemies();
            }
        }
    }

}
