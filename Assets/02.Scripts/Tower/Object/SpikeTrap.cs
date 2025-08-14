using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public Animator animator;
    public float damage = 10f;
        
    [SerializeField] private TrapController _trap;
    
    private void Reset()
    {
        if (_trap == null) _trap = GetComponent<TrapController>();
        if (_trap == null) _trap = gameObject.AddComponent<TrapController>();
    }

    private void Awake()
    {
        if (_trap == null) _trap = GetComponent<TrapController>();
        if (_trap == null) _trap = gameObject.AddComponent<TrapController>();
        if (_trap != null && animator != null);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if (collision.gameObject.name == "MoveBackTriggerHandler")
        // {
        //     ActivateTrap();
        // }
    }

    private void ActivateTrap()
    {
        // if (animator != null)
        //     animator.SetTrigger("Activated");
        if (_trap != null) _trap.Activate();
    }
}
