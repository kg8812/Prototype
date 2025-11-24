using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class IfPlayerXDistance : BossDecoratorNode
    {
        public float distance;
        Actor ControllingEntity => GameManager.instance.ControllingEntity;

        public enum UpOrDown
        {
            Up, Down
        }
        public UpOrDown distanceType;

        public override void OnStart()
        {          
        }
    
        public override void OnStop()
        {
        }
    
        public override State OnUpdate()
        {
            float dist = Mathf.Abs(ControllingEntity.Position.x - boss.Position.x);

            switch (distanceType)
            {
                case UpOrDown.Up:

                    if (dist > distance)
                    {
                        return child.Update();
                    }
                    else if (child.state == State.Running)
                    {
                        return child.Update();
                    }
                    child.state = State.Failure;
                    break;
                case UpOrDown.Down:
                    if (dist < distance)
                    {
                        return child.Update();
                    }
                    else if (child.state == State.Running)
                    {
                        return child.Update();
                    }
                    child.state = State.Failure;
                    break;
            }
            return State.Failure;
        }
        public override bool Check()
        {
            float dist = Mathf.Abs(ControllingEntity.Position.x - boss.Position.x);

            switch (distanceType)
            {
                case UpOrDown.Up:

                    if (dist > distance)
                    {
                        return CheckChild;
                    }
                    break;
                case UpOrDown.Down:
                    if (dist < distance)
                    {
                        return CheckChild;
                    }

                    break;
            }
            return false;
        }
    }
}