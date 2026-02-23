namespace CraterSprite.Shared.Scripts;

public interface IDamageListener
{
    public void TakeDamage(float damageAmount, CharacterStats source);
}