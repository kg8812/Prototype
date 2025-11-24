
namespace Apis
{
    public class Debuff_MaxHpLose : SubBuff
    {
        public Debuff_MaxHpLose(Buff effect) : base(effect)
        {
        }

        public override SubBuffType Type => SubBuffType.Debuff_MaxHpLose;

        public override void PermanentApply()
        {
            base.PermanentApply();

            float dmg = actor.MaxHp * amount[0] / 100;
            EventParameters parameters = new(target.GetComponent<IEventUser>(), actor)
            {
                hitData = new(){dmg = dmg}
            };

            actor.OnHit(parameters);
        }
    }
}