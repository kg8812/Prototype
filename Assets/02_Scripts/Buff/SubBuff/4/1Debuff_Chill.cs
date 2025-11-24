
using UnityEngine;

namespace Apis
{
    public class Debuff_Chill : Debuff_CC
    {
        public Debuff_Chill(Buff buff) : base(buff)
        {
            BuffDatabase.DataLoad.TryGetSubBuffOption(401, out var option);

            amount = option.amount;
            Stat.AddRatio(ActorStatType.MoveSpeed, -amount[0]);
        }

        public override void OnAdd()
        {
            base.OnAdd();
            if (actor.SubBuffCount(Type) >= 2)
            {
                actor.RemoveType(Type);
                actor.SubBuffManager.AddCC(target.GetComponent<IEventUser>(), SubBuffType.Debuff_Frozen,amount[1]);
            }
        }

        protected override void OnTypeAdd()
        {
            base.OnTypeAdd();
            
            SpawnEffect(Define.EtcEffects.ColdAir,Vector2.zero);
        }

        protected override void OnTypeRemove()
        {
            base.OnTypeRemove();
            RemoveEffect();
        }

        public override SubBuffType Type => SubBuffType.Debuff_Chill;
    }
}