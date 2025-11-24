
namespace Apis
{
    public class Buff_LifeSteal : SubBuff
    {
        public Buff_LifeSteal(Buff effect) : base(effect)
        {
        }

        public override SubBuffType Type => SubBuffType.Buff_LifeSteal;


        public override void OnAdd()
        {
            base.OnAdd();
            actor.AddEvent(EventType.OnAfterAtk,Invoke);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            actor.RemoveEvent(EventType.OnAfterAtk, Invoke);
        }

        public override void PermanentApply()
        {
            base.PermanentApply();
            actor.AddEvent(EventType.OnAfterAtk, Invoke);
        }
        public override void TempApply(EventParameters parameters)
        {
            base.TempApply(parameters);
            Invoke(parameters);
        }
        void Invoke(EventParameters parameters)
        {
            float dmg = parameters.hitData.dmgReceived;

            actor.CurHp += dmg * amount[0] / 100;
        }
    }
}