namespace Apis.BehaviourTreeTool
{
    public class SetAnimation : CommonActionNode
    {
        public enum TriggerType
        {
            Integer,
            Bool,
            Trigger,
            Float
        }

        public TriggerType type;
        public string paramName;
        public string paramValue;

        public override State OnUpdate()
        {
            switch (type)
            {
                case TriggerType.Integer:
                    if (int.TryParse(paramValue, out var integer))
                    {
                        _actor.animator.SetInteger(paramName, integer);
                        return State.Success;
                    }

                    return State.Failure;
                case TriggerType.Bool:
                    if (bool.TryParse(paramValue, out var boolValue))
                    {
                        _actor.animator.SetBool(paramName, boolValue);
                        return State.Success;
                    }

                    return State.Failure;
                case TriggerType.Float:
                    if (float.TryParse(paramValue, out var floatValue))
                    {
                        _actor.animator.SetFloat(paramName, floatValue);
                        return State.Success;
                    }

                    return State.Failure;
                case TriggerType.Trigger:
                    _actor.animator.SetTrigger(paramName);
                    break;
            }

            return State.Success;
        }
    }
}