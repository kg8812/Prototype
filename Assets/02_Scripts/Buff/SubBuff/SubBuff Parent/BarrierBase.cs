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

        private class MaxHpBase : IStrategy
        {
            public float Calculate(BarrierBase sub)
            {
                return sub.actor.MaxHp / 100 * sub.buff.BuffPower[0];
            }
        }

        private class FixedAmount : IStrategy
        {
            public float Calculate(BarrierBase sub)
            {
                return sub.amount[0];
            }
        }

        private class AtkBase : IStrategy
        {
            public float Calculate(BarrierBase sub)
            {
                return sub.buff.buffActor.Atk * (1 + sub.amount[0] / 100);
            }
        }

        private class RepairBase : IStrategy
        {
            public float Calculate(BarrierBase sub)
            {
                if (sub.buff.buffActor is Player player)
                {
                    return player.CalculateRepair() * sub.amount[0] / 100;
                }

                return 0;
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
                0 => new MaxHpBase(),
                1 => new FixedAmount(),
                2 => new AtkBase(),
                3 => new RepairBase(),
                _ => null
            };

            if (strategy != null) barrier = strategy.Calculate(this) * (1 + actor.ShieldRate / 100);
        }

        public override void OnAdd()
        {
            base.OnAdd();
            actor.ExecuteEvent(EventType.OnBarrierChange, null);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            actor.ExecuteEvent(EventType.OnBarrierChange, null);
        }
    }
}