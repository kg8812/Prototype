
namespace Apis
{
    public class Buff_BasicAtkEnhance : Buff_Base
    {
        public Buff_BasicAtkEnhance(Buff buff) : base(buff)
        {
        }

        public override void OnAdd()
        {
            base.OnAdd();
            actor.AddEvent(EventType.OnBasicAttack, TempApply);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            actor.RemoveEvent(EventType.OnBasicAttack, TempApply);
        }

        public override void PermanentApply()
        {
            base.PermanentApply();
            actor.RemoveEvent(EventType.OnBasicAttack, TempApply);
        }

        public override void TempApply(EventParameters parameters)
        {
            base.TempApply(parameters);
            switch (buff.ValueType)
            {
                case ValueType.Value:
                    parameters.atkData.dmg += amount[0];
                    break;
                case ValueType.Ratio:
                    parameters.statData.stat.AddValue(ActorStatType.ExtraDmg, amount[0]);
                    break;
            }
        }
        
        public override SubBuffType Type => SubBuffType.Buff_BasicAtkEnhance;
    }
}