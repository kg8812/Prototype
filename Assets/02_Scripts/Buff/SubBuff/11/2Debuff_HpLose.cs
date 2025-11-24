
namespace Apis
{
    public class Debuff_HpLose : SubBuff
    {       
        public Debuff_HpLose(Buff effect) : base(effect)
        {
        }

        public override SubBuffType Type => SubBuffType.Debuff_HpLose;

        public override void PermanentApply()
        {
            base.PermanentApply();
            EventParameters parameters = new(target.GetComponent<IEventUser>(), actor)
            {
                hitData = new(){dmg = amount[0]}
            };

            actor.OnHit(parameters);
        }
        
    }
}