
namespace Apis
{
    public class Weapon_Decorator : IWeaponStat
    {
        public BonusStat BonusStat => decoratedWeapon.BonusStat + attachment.BonusStat;         

        readonly IWeaponStat decoratedWeapon;
        readonly IWeaponStat attachment;

        public Weapon_Decorator(IWeaponStat weapon,IWeaponStat attachment)
        {
            decoratedWeapon = weapon;
            this.attachment = attachment;
        }
    }
}