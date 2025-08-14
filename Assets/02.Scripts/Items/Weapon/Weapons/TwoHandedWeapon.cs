using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TwoHandedWeapon : BaseWeapon
{
    public override Transform GetHandleHand() => mainHand;

}

