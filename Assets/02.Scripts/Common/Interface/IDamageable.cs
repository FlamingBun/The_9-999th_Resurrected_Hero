public interface IDamageable
{
    public bool CanDamageable { get; }
    public void TakeDamage(DamageInfo damageInfo);
}
