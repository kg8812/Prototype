using System;

namespace Apis
{
    public class HitImmuneWeapon : Weapon
    {
        private Guid guid;
        public override void Init()
        {
            base.Init();
            IAttack?.OnAfterAtk.AddListener(() =>
            {
                guid = Player.AddHitImmunity();
            });
        }

        public override void BeforeAttack()
        {
            base.BeforeAttack();
            Player.RemoveHitImmunity(guid);
        }
    }
}