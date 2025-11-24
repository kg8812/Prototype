
using UnityEngine;

namespace Apis
{
    public class Debuff_Execution : Debuff_base
    {
        public Debuff_Execution(Buff effect) : base(effect)
        {
        }
        public override SubBuffType Type => SubBuffType.Debuff_Execution;


        private void Invoke(EventParameters parameters)
        {
            if (actor.CurHp / actor.MaxHp * 100 < amount[0])
            {
                DmgTextManager.ShowDmgText(actor.Position,9999,Color.white);
                actor.Die();
            }
        }
        public override void OnAdd()
        {
            base.OnAdd();
            actor.AddEvent(EventType.OnHpDown, Invoke);
            Invoke(new EventParameters(actor));
        }

        public override void OnRemove()
        {
            base.OnRemove();
            actor.RemoveEvent(EventType.OnHpDown, Invoke);
        }

        public override void PermanentApply()
        {
            base.PermanentApply();
            actor.AddEvent(EventType.OnHpDown, Invoke);
            Invoke(new EventParameters(actor));
        }
        public override void TempApply(EventParameters parameters)
        {
            base.TempApply(parameters);
            Invoke(parameters);

        }
    }
}