
namespace Apis
{
    public class Debuff_CurHpLose : SubBuff
    {
        public Debuff_CurHpLose(Buff buff) : base(buff)
        {
        }
        public override void PermanentApply()
        {
            base.PermanentApply();
            float dmg = actor.CurHp * amount[0] / 100;
            EventParameters parameters = new(target.GetComponent<IEventUser>(), actor)
            {
                hitData = new(){dmg = dmg}
            };

            actor.OnHit(parameters);
        }
        public override SubBuffType Type => SubBuffType.Debuff_CurHpLose;
    }
}