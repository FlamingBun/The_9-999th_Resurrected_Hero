public class TempProjectileCountAbility : BaseAbility
{
    public override string Name => "투사체 개수 증가";
    
    public override string Description => "투사체의 개수를 1만큼 증가";
    
    private StatModifier modifier;

    public override void Apply(PlayerController player)
    {
        //modifier = new StatModifier(1, StatModType.Flat);
        
        //player.WeaponHandler.EquippedWeapon.StatHandler.AddModifier(WeaponStatType.ProjectileCount, modifier);
    }

}
