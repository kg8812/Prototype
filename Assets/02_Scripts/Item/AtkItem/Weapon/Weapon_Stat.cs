namespace Apis
{
    public class Weapon_Stat : IWeaponStat
    {
        private readonly Weapon_Config config;

        public Weapon_Stat(Weapon_Config config)
        {
            this.config = config;
        }

        public BonusStat BonusStat => config.BonusStat;
    }
}