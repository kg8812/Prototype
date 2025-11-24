using Apis;
using Sirenix.OdinInspector;

public class ThrowRushWeapon : Weapon
{
    [LabelText("투척 설정")] public RushAtk.RushInfo rushInfo;

    private IWeaponAttack iAttack;

    public override IWeaponAttack IAttack => iAttack ??= new RushAtk(this, rushInfo);
}
