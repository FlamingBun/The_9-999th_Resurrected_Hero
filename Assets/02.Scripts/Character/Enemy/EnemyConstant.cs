using UnityEngine;

public enum EnemyStates
{
    // Common State
    Idle,
    Trace,
    Hit,
    Dead,
    Evade,
    
    // Enemy State
    MeleeAttack,
    RangeAttack,
    DashAttack,
    FixedAttack,
    
    // Boss State
    FirstAttack,
    SecondAttack,
    ThirdAttack,
    FourthAttack,
}

public static class EnemyConstant
{
    public static readonly int randomPositionMaxCount = 30; // get random position 최대 횟수

    public static readonly int projectileDefaultCount = 150;
    
    public static readonly float stateDelayTime = 0.1f; // state 입장 즉시 변경 방지

    #region Initialize

    public static readonly float initializeEffectTime = 1.5f;
    
    public static readonly int pixelateStartValue = 4;
    public static readonly int pixelateEndValue = 120;
    public static readonly int pixelateDefaultValue = 512;
    
    #endregion Initialize
    
    #region Hit
    
    public static readonly Color DefaultColor = new Color(1,1,1);
    public static readonly Color HitColor = new Color(1,0,0);
    public static readonly float hitEffectColorDuration = 0.15f;
    
    #endregion Hit
    
    #region Death
    
    public static readonly float deathMoveHorizontalPower = 1.4f;

    public static readonly float deathMoveVerticalPower = 0.6f;

    public static readonly float deathJumpPower = 0.2f;
    public static readonly int deathJumpCount = 1;
    public static readonly float deathJumpDuration = 1.2f;

    public static readonly float deathJumpInterval = 0.3f;

    public static readonly float deathFadeTime = 6f;

    #endregion Death
}

public static class EnemyAnimationHashes
{
    public static readonly int Idle = Animator.StringToHash("Idle");
    
    public static readonly int Move = Animator.StringToHash("Move");
    public static readonly int MoveLoop = Animator.StringToHash("Move_Loop");
    
    public static readonly int Hit = Animator.StringToHash("Hit");
    public static readonly int Dead = Animator.StringToHash("Dead");
    
    public static readonly int Attack = Animator.StringToHash("Attack");
    
    public static readonly int WhirlWind = Animator.StringToHash("Attack_WhirlWind");
    public static readonly int WhirlWindLoop = Animator.StringToHash("Attack_WhirlWind_Loop");

    public static readonly int JumpAttack = Animator.StringToHash("Attack_JumpAttack");

    public static readonly int StompAttack = Animator.StringToHash("Attack_StompAttack");
    
    public static readonly int SpawnStone = Animator.StringToHash("SpawnStone");
    public static readonly int SpawnSmoke = Animator.StringToHash("SpawnSmoke");             
}

public static class EnemyAnimationEventKey
{
    public const string AttackStartKey = "AttackStart";
    public const string AttackHitKey = "AttackHit";
    public const string AttackEndKey = "AttackEnd";
    
    public const string HitFloorKey = "HitFloor";
    
    public const string MoveStartKey = "MoveStart";
    public const string MoveEndKey = "MoveEnd";

    public const string HitEndKey = "HitEnd";
}

public static class EnemyMaterialKey
{
    public static readonly int HitEffectBlendKey = Shader.PropertyToID("_HitEffectBlend"); 
    public static readonly int HitEffectGlowKey = Shader.PropertyToID("_HitEffectGlow");
    
    public static readonly int FadeAmountKey = Shader.PropertyToID("_FadeAmount");
    public static readonly int GreyScaleBlendKey = Shader.PropertyToID("_GreyscaleBlend");
    public static readonly int OutLineAlphaKey = Shader.PropertyToID("_OutlineAlpha");

    public static readonly int ColorKey = Shader.PropertyToID("_Color");
    
    public static readonly int PixelateSizeKey = Shader.PropertyToID("_PixelateSize");
}