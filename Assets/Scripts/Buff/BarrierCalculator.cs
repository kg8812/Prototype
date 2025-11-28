using Apis;

public class BarrierCalculator
{
    private float _barrier;
    private readonly SubBuffManager buffManager;

    private readonly IEventManager user;

    public BarrierCalculator(IEventManager user, SubBuffManager buffManager)
    {
        this.user = user;
        this.buffManager = buffManager;
    }

    public float Barrier
    {
        get
        {
            float value = 0;

            buffManager.Traverse(x =>
            {
                if (x is BarrierBase barrier) value += barrier.Barrier;
            });
            return _barrier + value;
        }
    }

    public void Calculate(EventParameters parameters)
    {
        parameters.hitData.dmg = (int)MinusBarrier(parameters.hitData.dmg);
        if (_barrier >= parameters.hitData.dmg)
        {
            _barrier -= parameters.hitData.dmg;
            parameters.hitData.dmg = 0;
        }
        else
        {
            parameters.hitData.dmg -= (int)_barrier;
            _barrier = 0;
        }
    }

    public void AddBarrier(float value)
    {
        _barrier += value;
        user?.ExecuteEvent(EventType.OnBarrierChange, null);
    }

    private float MinusBarrier(float dmg)
    {
        buffManager.Traverse(x =>
        {
            if (dmg <= 0) return;

            if (x is BarrierBase barrier)
            {
                if (barrier.Barrier > dmg)
                {
                    barrier.Barrier -= dmg;
                    dmg = 0;
                }
                else
                {
                    dmg -= barrier.Barrier;
                    barrier.Barrier = 0;
                    barrier.onBarrierDestroy.Invoke();
                }
            }
        });

        if (dmg <= 0) return dmg;

        if (_barrier >= dmg)
        {
            _barrier -= dmg;
            dmg = 0;
        }
        else
        {
            dmg -= _barrier;
            _barrier = 0;
        }

        return dmg;
    }
}