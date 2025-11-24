
namespace Apis
{
    public class Buff_Barrier : BarrierBase
    {
        public Buff_Barrier(Buff buff) : base(buff)
        {
            onBarrierDestroy.AddListener(() => actor.RemoveSubBuff(buff,this));
        }

        public override SubBuffType Type => SubBuffType.Buff_Barrier;

        public override void PermanentApply()
        {
            base.PermanentApply();
            if (actor != null)
            {
                actor.AddBarrier(barrier);
            }
        }
        public override void TempApply(EventParameters parameters)
        {
            base.TempApply(parameters);
            if (actor != null)
            {
                actor.AddBarrier(barrier);
            }
        }
    }
}