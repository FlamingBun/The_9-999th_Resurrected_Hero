using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class RoomPath : MonoBehaviour
{
    public Dir4 LookDir => _lookDir;
    
    
    [SerializeField] private Tilemap minimapTilemap;
    
    
    private Tilemap[] _tilemaps;
    private RoomPathBarrier _barrier;
    
    private Dir4 _lookDir;

    
    public void Init(Dir4 lookDir)
    {
        _barrier = GetComponentInChildren<RoomPathBarrier>();
        _tilemaps = GetComponentsInChildren<Tilemap>();

        
        _lookDir = lookDir;
        
        _barrier.Init();
        
        minimapTilemap.gameObject.SetActive(false);
        
        gameObject.SetActive(false);
    }
    
    
    
    public void ActivePath(Tilemap[] roomTilemaps)
    {
        gameObject.SetActive(true);

        
        RemoveRootTilemaps(roomTilemaps);
    }


    public void EnableBarrier() => _barrier.Enable();
    public void DisableBarrier() => _barrier.Disable();
    
    
    
    public void ShowMinimapTilemap() =>  minimapTilemap.gameObject.SetActive(true);
    public void HideMinimapTilemap() =>  minimapTilemap.gameObject.SetActive(false);

    
    
    private void RemoveRootTilemaps(Tilemap[] roomTilemaps)
    {
        foreach (var tilemap in _tilemaps)
        {
            foreach (Vector3Int cellPos in  tilemap.cellBounds.allPositionsWithin)
            {
                TileBase tile = tilemap.GetTile(cellPos);
            
                if (tile == null) continue;
                
                foreach (var roomTilemap in roomTilemaps)
                {
                    if (roomTilemap.HasTile(cellPos))
                    {
                        roomTilemap.SetTile(cellPos, null);
                    }
                }
            }
        }
    }
}
