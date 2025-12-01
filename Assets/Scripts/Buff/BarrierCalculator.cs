using Apis;

public class BarrierCalculator
{
    private float _barrier;

    private readonly IEventManager user;

    public delegate float FloatCalculateDelegate(float barrier);
    FloatCalculateDelegate _barrierAddEvent;
    FloatCalculateDelegate _barrierMinusEvent;
    
    public event FloatCalculateDelegate BarrierAddEvent
    {
        add
        {
            _barrierAddEvent -= value;
            _barrierAddEvent += value;
        }
        remove => _barrierAddEvent -= value;
    }
    
    public event FloatCalculateDelegate BarrierMinusEvent
    {
        add
        {
            _barrierMinusEvent -= value;
            _barrierMinusEvent += value;
        }
        remove => _barrierMinusEvent -= value;
    }
    public BarrierCalculator(IEventManager user)
    {
        this.user = user;
    }

    public float Barrier
    {
        get
        {
            float value = 0;

            if (_barrierAddEvent != null)
            {
                value = _barrierAddEvent.Invoke(value);
            }
            
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
        if (_barrierMinusEvent != null)
        {
            dmg = _barrierMinusEvent.Invoke(dmg);
        }

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