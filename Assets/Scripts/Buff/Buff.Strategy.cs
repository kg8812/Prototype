namespace Apis
{
    public partial class Buff
    {
        public interface IApplyType
        {
            void Apply(Actor actor, EventParameters parameters);
        }

        public class PermanentApply : IApplyType
        {
            private readonly Buff buff;

            public PermanentApply(Buff buff)
            {
                this.buff = buff;
            }

            public void Apply(Actor actor, EventParameters _)
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

            public void Apply(Actor actor, EventParameters _)
            {
                actor.AddSubBuff(buff.buffActor, buff, buff.ActivatedSubBuff);
            }
        }

        public class TempApply : IApplyType
        {
            private readonly Buff buff;

            public TempApply(Buff buff)
            {
                this.buff = buff;
            }

            public void Apply(Actor actor, EventParameters parameters)
            {
                if (parameters != null)
                    buff.ActivatedSubBuff?.TempApply(parameters);
            }
        }
    }
}