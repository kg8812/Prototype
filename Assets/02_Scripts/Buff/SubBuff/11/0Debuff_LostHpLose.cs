namespace Apis
{
    public class Debuff_LostHpLose : SubBuff
    {
        public override SubBuffType Type => SubBuffType.Debuff_LostHpLose;

        public Debuff_LostHpLose(Buff effect) : base(effect)
        {
        }

        public override void PermanentApply()
        {
            base.PermanentApply();
            float dmg = (actor.MaxHp - actor.CurHp) * amount[0] / 100;
            EventParameters parameters = new(target.GetComponent<IEventUser>(), actor)
            {
                hitData = new(){dmg = dmg}
            };
            actor.OnHit(parameters);
        }

    }
}