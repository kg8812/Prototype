namespace Apis
{
    public class Weapon_Config : IWeaponStat
    {
        public BonusStat BonusStat { get; } = new();
    }
}