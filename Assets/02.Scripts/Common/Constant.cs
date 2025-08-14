using UnityEngine;

public static class Constant
{
    public const string BaseSceneName = "BaseScene";
    public const string TownSceneName = "TownScene";
    public const string TowerSceneName = "TowerScene";
    public const string FloorSceneName = "Tower_FloorScene";
    
    public const float CameraOrthoSize = 8.75f;
}


public static class PlayerConstant
{
    public static readonly int MoveHash = Animator.StringToHash("Move");
    
    public static readonly int AttackHash = Animator.StringToHash("@Attack");
    public static readonly int ParryHash = Animator.StringToHash("Parry");
    public static readonly int ComboAttackHash = Animator.StringToHash("ComboAttack");
    public static readonly int DodgeAttackHash = Animator.StringToHash("DodgeAttack");

    public static readonly int BasicHash = Animator.StringToHash("@Basic");
    public static readonly int BasicAttackComboCountHash = Animator.StringToHash("BasicMeleeAttackComboCount");

    public static readonly int ChargeHash = Animator.StringToHash("@Charge");

    
    public const string StartKey = "Start";
    public const string StartAttackKey = "StartAttack";
    public const string HitKey = "Hit";
    public const string FinishAttackKey = "FinishAttack";
    public const string FinishKey = "Finish";

    public const float SprintSpeedMultiplier = 1.5f;
}

