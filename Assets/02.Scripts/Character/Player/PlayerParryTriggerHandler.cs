using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryTriggerHandler : MonoBehaviour
{
    private PlayerController _player;

    private Vector2 _parryDir;

    public void Init(PlayerController player)
    {
        _player = player;
        
        gameObject.SetActive(false);
    }

    public void Enable(Vector2 parryDir)
    {
        _parryDir = parryDir;
        
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
 
    public bool TryParry(Vector2 enemyAttackDir)
    {
        float dotProduct = Vector3.Dot(_player.LookDir, -enemyAttackDir);

        if (dotProduct > 0)
        {
            Debug.Log("parry!");
            return true;
        }

        return false;
    }
}
    