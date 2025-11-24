using UnityEngine;

namespace Apis
{
    public class Debuff_Burn : Debuff_DotDmg
    {
        public Debuff_Burn(Buff buff) : base(buff)
        {
        }
        
        public override void OnAdd()
        {
            base.OnAdd();
            Stat.SetValue(ActorStatType.DmgReduce, 0);
            if (actor.SubBuffCount(SubBuffType.Debuff_Burn) == 1)
            {
                Stat.SetValue(ActorStatType.DmgReduce , -option.amount[1]);
            }
        }

        protected override void OnTypeAdd()
        {
            base.OnTypeAdd();
            SpawnEffect(Define.EtcEffects.Burn,Vector2.zero);
        }

        protected override void OnTypeRemove()
        {
            base.OnTypeRemove();
            RemoveEffect();
        }

        public override SubBuffType Type => SubBuffType.Debuff_Burn;
    }
}