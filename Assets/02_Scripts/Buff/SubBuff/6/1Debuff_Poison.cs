using Apis;
using UnityEngine;

namespace Apis
{
    public class Debuff_Poison : Debuff_DotDmg
    {
        public Debuff_Poison(Buff buff) : base(buff)
        {
        }


        public override void OnAdd()
        {
            base.OnAdd();
            if (actor == null) return;
            if (actor.SubBuffCount(Type) == 1)
            {
                actor.AddEvent(EventType.OnDeath, Invoke);
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            if (actor == null) return;
            actor.RemoveEvent(EventType.OnDeath,Invoke);
        }

        protected override void OnTypeAdd()
        {
            base.OnTypeAdd();
            
            SpawnEffect(Define.EtcEffects.Poison,Vector2.zero);
        }

        protected override void OnTypeRemove()
        {
            base.OnTypeRemove();
            RemoveEffect();
        }

        void Invoke(EventParameters parameters)
        {
            AttackObject atk = GameManager.Factory
                .Get(FactoryManager.FactoryType.AttackObject, "PoisonEffect", actor.Position)
                .GetComponent<AttackObject>();
            
            atk.Init(actor,new AtkBase(actor),option.amount[1]);
        }

        public override SubBuffType Type => SubBuffType.Debuff_Poison;

    }
}