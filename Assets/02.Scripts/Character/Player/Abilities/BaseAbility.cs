using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAbility 
{
    public abstract string Name { get; }
    public abstract string Description { get; }

    public abstract void Apply(PlayerController player);
}
