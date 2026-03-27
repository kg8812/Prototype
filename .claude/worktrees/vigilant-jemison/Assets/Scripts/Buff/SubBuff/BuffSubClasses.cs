using System.Collections.Generic;
using UnityEngine;

namespace Apis
{
    public class SubBuffLifeCycleHandler
    {
        public void AfterSubBuffAdded(SubBuff sub)
        {
            sub?.OnAdd();
        }

        public void AfterSubBuffRemoved(SubBuff sub)
        {
            sub?.OnRemove();
        }

        public void AfterSubBuffMaxStackReached(SubBuff sub)
        {
            sub?.OnMaxStack();
        }
    }

    public class BuffBarrierProcessor
    {
        private readonly IBarrierUser _user;
        private readonly SubBuffManager _manager;

        public BuffBarrierProcessor(IBarrierUser user, SubBuffManager manager)
        {
            _user = user;
            _manager = manager;
        }

        public void Bind()
        {
            if (_user == null) return;

            _user.BarrierCalculator.BarrierAddEvent += AddBarrier;
            _user.BarrierCalculator.BarrierMinusEvent += MinusBarrier;
        }

        public void Unbind()
        {
            if (_user == null) return;

            _user.BarrierCalculator.BarrierAddEvent -= AddBarrier;
            _user.BarrierCalculator.BarrierMinusEvent -= MinusBarrier;
        }
        public float AddBarrier(float value)
        {
            _manager.Traverse(x =>
            {
                if (x is BarrierBase barrier) value += barrier.Barrier;
            });

            return value;
        }
        
        float MinusBarrier(float dmg)
        {
            List<BarrierBase> destroyed = new();

            _manager.Traverse(x =>
            {
                if (dmg <= 0) return;

                if (x is not BarrierBase barrier)
                    return;

                if (barrier.Barrier > dmg)
                {
                    barrier.Barrier -= dmg;
                    dmg = 0;
                }
                else
                {
                    dmg -= barrier.Barrier;
                    barrier.Barrier = 0;
                    destroyed.Add(barrier);
                }
            });

            foreach (var barrier in destroyed)
                barrier.onBarrierDestroy.Invoke();

            return dmg;
        }
    }
}