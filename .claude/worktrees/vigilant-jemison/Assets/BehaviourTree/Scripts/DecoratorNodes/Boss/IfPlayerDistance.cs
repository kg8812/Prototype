using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class IfPlayerDistance : BossDecoratorNode
    {
        public enum UpOrDown
        {
            Up,
            Down
        }

        public int distance;
        public Color color;

        public UpOrDown distanceType;
        private Actor ControllingEntity => GameManager.instance.ControllingEntity;

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override State OnUpdate()
        {
            var dist = Vector2.Distance(ControllingEntity.Position, _actor.Position);

            switch (distanceType)
            {
                case UpOrDown.Up:

                    if (dist > distance) return child.Update();

                    if (child.state == State.Running) return child.Update();
                    child.state = State.Failure;
                    break;
                case UpOrDown.Down:
                    if (dist < distance) return child.Update();

                    if (child.state == State.Running) return child.Update();
                    child.state = State.Failure;
                    break;
            }

            return State.Failure;
        }

        public override bool Check()
        {
            var dist = Vector2.Distance(ControllingEntity.Position, _actor.Position);

            switch (distanceType)
            {
                case UpOrDown.Up:

                    if (dist > distance) return CheckChild;
                    break;
                case UpOrDown.Down:
                    if (dist < distance) return CheckChild;

                    break;
            }

            return false;
        }
    }
}