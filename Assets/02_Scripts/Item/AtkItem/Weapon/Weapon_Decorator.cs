namespace Apis
{
    public class Weapon_Decorator : IWeaponStat
    {
        private readonly IWeaponStat attachment;

        private readonly IWeaponStat decoratedWeapon;

        public Weapon_Decorator(IWeaponStat weapon, IWeaponStat attachment)
        {
            decoratedWeapon = weapon;
            this.attachment = attachment;
        }

        public BonusStat BonusStat => decoratedWeapon.BonusStat + attachment.BonusStat;
    }
}