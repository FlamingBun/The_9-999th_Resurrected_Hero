using System.Collections.Generic;
using UnityEngine;

public class TowerCurseInstance
{
    public TowerCurseType CurseType;
    public float multiplier;
    public string featureDescription;

    public TowerCurseInstance(TowerCurseSO curseSO)
    {
        CurseType = curseSO.curseType;
        multiplier = curseSO.multiplier;
        featureDescription = curseSO.featureDescription;
    }

    public void ModifyMult(TowerCurseSO curseSO)
    {
        multiplier *= curseSO.multiplier;
        multiplier = Mathf.Floor(multiplier * 10) / 10;
    }
}