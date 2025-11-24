
namespace Apis
{
    public class Debuff_AtkHpLose : SubBuff
    {
        public Debuff_AtkHpLose(Buff buff) : base(buff)
        {
        }
        
        public override void PermanentApply()
        {
            base.PermanentApply();
            float dmg = buff.buffActor.Atk * amount[0] / 100;
            EventParameters parameters = new(target.GetComponent<IEventUser>(), actor)
            {
                hitData = new(){dmg = dmg}
            };

            actor.OnHit(parameters);
        }
        public override SubBuffType Type => SubBuffType.Debuff_AtkHpLose;
    }
}