namespace Apis
{
    public partial class Buff
    {
        public interface IApplyType
        {
            void Apply(IBuffUser actor, EventParameters parameters);
        }

        public class PermanentApply : IApplyType
        {
            private readonly Buff buff;

            public PermanentApply(Buff buff)
            {
                this.buff = buff;
            }

            public void Apply(IBuffUser actor, EventParameters _)
            {
                if (buff.ActivatedSubBuff == null) return;

                buff.ActivatedSubBuff.target = actor.gameObject;
                buff.ActivatedSubBuff.PermanentApply();
            }
        }

        public class NormalApply : IApplyType
        {
            private readonly Buff buff;

            public NormalApply(Buff buff)
            {
                this.buff = buff;
            }

            public void Apply(IBuffUser actor, EventParameters _)
            {
                actor.AddSubBuff(buff.buffUser, buff, buff.ActivatedSubBuff);
            }
        }

        public class TempApply : IApplyType
        {
            private readonly Buff buff;

            public TempApply(Buff buff)
            {
                this.buff = buff;
            }

            public void Apply(IBuffUser actor, EventParameters parameters)
            {
                if (parameters != null)
                    buff.ActivatedSubBuff?.TempApply(parameters);
            }
        }
    }
}