
using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    public abstract CanvasLayer Layer { get; }

    public abstract bool IsEnabled { get; }
    public abstract void Enable();
    public abstract void Disable();


 
}
