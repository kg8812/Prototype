namespace Apis
{
    public class Buff_DefBarrier : SubBuff
    {
        public Buff_DefBarrier(Buff buff) : base(buff)
        {
        }

        public override void OnAdd()
        {
            base.OnAdd();
            actor.AddEvent(EventType.OnBeforeHit,Invoke);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            actor.RemoveEvent(EventType.OnBeforeHit,Invoke);
        }

        public override void TempApply(EventParameters parameters)
        {
            base.TempApply(parameters);
            Invoke(parameters);
        }

        public override SubBuffType Type => SubBuffType.Buff_DefBarrier;

        void Invoke(EventParameters parameters)
        {
            parameters.hitData.dmg = 0;
            actor.RemoveSubBuff(buff,this);
        }
    }
}