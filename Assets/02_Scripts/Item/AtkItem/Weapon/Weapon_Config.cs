
namespace Apis
{
    public class Weapon_Config : IWeaponStat
    {       
        readonly BonusStat bonusStat = new();
        public BonusStat BonusStat { get { return bonusStat; } }
    }
}
