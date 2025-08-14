public class TempPoisonAbility : BaseAbility
{
    public override string Name { get; }
    public override string Description { get; }
    
    public override void Apply(PlayerController player)
    {
        WeaponTempEffect poisonEffect = new();
        
        /*if (equipWeapon.TryGetBehavior(out WeaponMeleeBehavior melee))
        {
            player.WeaponHandler.EquippedWeapon.WeaponEffects.Add(poisonEffect);
        }*/
    }
}