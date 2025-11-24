using DG.Tweening;

namespace Apis
{
    public class Debuff_KnockBack : Debuff_CC
    {
        public Debuff_KnockBack(Buff effect) : base(effect)
        {
        }

        public override SubBuffType Type => SubBuffType.Debuff_KnockBack;


        public override void OnAdd()
        {
            base.OnAdd();
            if (actor is IMovable mover)
            {
                mover.MoveCCOn();
            }
            Push();
        }
        
        public override void OnRemove()
        {
            base.OnRemove();
            if (actor is IMovable mover)
            {
                mover.MoveCCOff();
            }
        }

        void Push()
        {
            float power = amount[0];

            if(target.transform.position.x > actor.Position.x)
            {
                power *= -1;
            }
            actor.Rb.DOMoveX(actor.transform.position.x + power, duration);

            
        }
    }
}