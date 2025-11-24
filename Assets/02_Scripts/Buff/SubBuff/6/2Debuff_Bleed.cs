
using UnityEngine;

namespace Apis
{
    public class Debuff_Bleed : Debuff_DotDmg
    {
        public Debuff_Bleed(Buff buff) : base(buff)
        {
        }

        public override void OnAdd()
        {
            base.OnAdd();
            if (actor.SubBuffCount(Type) == 1)
            {
                Stat.SetValue(ActorStatType.CritHit,  option.amount[1]);
            }
        }

        protected override void OnTypeAdd()
        {
            base.OnTypeAdd();
            SpawnEffect(Define.EtcEffects.Bleeding,Vector2.zero);
        }

        protected override void OnTypeRemove()
        {
            base.OnTypeRemove();
            RemoveEffect();
        }

        public override SubBuffType Type => SubBuffType.Debuff_Bleed;

    }
}
