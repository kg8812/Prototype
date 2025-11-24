
namespace Apis
{
    public class Weapon_Stat : IWeaponStat
    {
        public BonusStat BonusStat => config.BonusStat;

        readonly Weapon_Config config;

        public Weapon_Stat (Weapon_Config config)
        {
            this.config = config;
        }
    }
}