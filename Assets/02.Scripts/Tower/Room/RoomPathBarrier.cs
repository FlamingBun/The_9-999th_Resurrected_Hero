using UnityEngine;

public class RoomPathBarrier : MonoBehaviour
{
    private ParticleSystem _particle;
    //private BoxCollider2D _collider2D;
    private PlayerController _player;


    public void Init()
    {
        _particle = GetComponentInChildren<ParticleSystem>();
        //_collider2D = GetComponentInChildren<BoxCollider2D>();

        //_collider2D.enabled = false;
    }

    public void Enable()
    {
        //_collider2D.enabled = true;
        _particle.Play();
    }
    
    public void Disable()
    {
        //_collider2D.enabled = false;
        _particle.Stop();
    }
}
