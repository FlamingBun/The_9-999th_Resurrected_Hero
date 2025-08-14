using UnityEngine;

public class RoomTrapEventHandler : MonoBehaviour, IRoomEvent
{
    public int Order => -10;          
    public bool IsCleared => _isCleared;

    [SerializeField] private Transform trapRoot; 

    private bool _isCleared;
    private RoomController _room;
    
    public void Init(RoomController room)
    {
        _room = room;
    }


    public void StartEvent()
    {
        if (trapRoot != null)
        {
            for (int i = 0; i < trapRoot.childCount; i++)
            {
                var go = trapRoot.GetChild(i).gameObject;
                go.SetActive(true);

                if (go.TryGetComponent(out TrapController trap))
                {
                    trap.Activate();
                }
            }
        }

        _isCleared = true; 
    }
}