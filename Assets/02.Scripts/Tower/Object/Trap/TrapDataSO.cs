using UnityEngine;

[CreateAssetMenu(
    fileName = "TrapData",
    menuName = "Scriptable Objects/Datas/Tower/Trap/TrapData")]
public class TrapDataSO : ScriptableObject
{
    public enum TriggerMode
    {
        OnEnter,
        OnStayTick,
        Timer
    }

    public string TrapId => trapId;
    public TriggerMode Mode => mode;
    public float Damage => damage;
    public float KnockbackForce => knockbackForce;
    public float TickInterval => tickInterval;
    public float ActivationDelay => activationDelay;
    public float ActiveDuration => activeDuration;
    public float Cooldown => cooldown;
    public LayerMask TargetMask => targetMask;
    public bool EnableOnRoomEnter => enableOnRoomEnter;
    public bool OneShot => oneShot;

    [SerializeField] private string trapId = "Spike";
    [SerializeField] private TriggerMode mode = TriggerMode.OnStayTick;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float knockbackForce = 4f;
    [SerializeField] private float tickInterval = 0.5f;          
    [SerializeField] private float activationDelay = 0f;         
    [SerializeField] private float activeDuration = 0.8f;        
    [SerializeField] private float cooldown = 1.0f;             
    [SerializeField] private LayerMask targetMask;                
    [SerializeField] private bool enableOnRoomEnter = true;       
    [SerializeField] private bool oneShot = false;               
}