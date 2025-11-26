using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public abstract class BarrierBase : Buff_Base
    {
        #region 배리어 적용 인터페이스

        private interface IStrategy
        {
            float Calculate(BarrierBase sub);
        }

        private class FixedAmount : IStrategy
        {
            public float Calculate(BarrierBase sub)
            {
                return sub.amount[0];
            }
        }

        #endregion
        
        protected float barrier;
        public readonly UnityEvent onBarrierDestroy = new();
        
        public float Barrier 
        { 
            get => barrier;
            set => barrier = value;
        }
        protected BarrierBase(Buff buff) : base(buff)
        {
            IStrategy strategy = buff.ApplyStrategy switch
            {
                0 => new FixedAmount(),
                _ => null
            };

            if (strategy != null) barrier = strategy.Calculate(this);
        }

        public override void OnAdd()
        {
            base.OnAdd();

            if (actor is IEventUser eventUser)
            {
                eventUser.EventManager.ExecuteEvent(EventType.OnBarrierChange, null);
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            if (actor is IEventUser eventUser)
            {
                eventUser.EventManager.ExecuteEvent(EventType.OnBarrierChange, null);
            }
        }
    }
}