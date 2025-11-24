
namespace Apis
{
    public class Debuff_BonusDmg : Debuff_base
    {
        public Debuff_BonusDmg(Buff effect) : base(effect)
        {
        }

        public override SubBuffType Type => SubBuffType.Debuff_BonusDmg;

        public override void OnAdd()
        {
            base.OnAdd();
            actor.AddEvent(EventType.OnAfterHit, Invoke);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            actor.RemoveEvent(EventType.OnAfterHit, Invoke);

        }
        public override void PermanentApply()
        {
            base.PermanentApply();
            actor.AddEvent(EventType.OnAfterHit, Invoke);
        }

        public override void TempApply(EventParameters parameters)
        {
            base.TempApply(parameters);
            Invoke(parameters);
        }
        void Invoke(EventParameters parameters)
        {
            EventParameters temp = new(actor, target.GetComponent<Actor>())
            {
                hitData = new(){dmg = amount[0]}
            };
            actor.OnHit(temp);
        }
    }
}