using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class IfXDistance : CommonDecoratorNode
    {
        public enum UpOrDown
        {
            Up,
            Down
        }

        public enum Way
        {
            Front,
            Back
        }

        public UpOrDown distanceType;
        public Way Method;
        public float distance;
        public string objectName;

        private Transform target;

        public override void OnStart()
        {
            target = GameObject.Find(objectName).transform;
        }

        public override void OnStop()
        {
        }

        public override State OnUpdate()
        {
            if (target == null) return State.Failure;

            var x = target.TryGetComponent(out Actor act) ? act.Position.x : target.position.x;

            switch (Method)
            {
                case Way.Front:
                    switch (distanceType)
                    {
                        case UpOrDown.Up:
                            if ((_actor.Direction == EActorDirection.Left && _actor.Position.x - x >= distance &&
                                 _actor.Position.x - x >= 0) ||
                                (_actor.Direction == EActorDirection.Right && x - _actor.Position.x >= distance &&
                                 x - _actor.Position.x >= 0))
                                return child.Update();
                            if (child.state == State.Running) return child.Update();
                            break;
                        case UpOrDown.Down:
                            if ((_actor.Direction == EActorDirection.Left && _actor.Position.x - x <= distance &&
                                 _actor.Position.x - x >= 0) ||
                                (_actor.Direction == EActorDirection.Right && x - _actor.Position.x <= distance &&
                                 x - _actor.Position.x >= 0))
                                return child.Update();

                            if (child.state == State.Running) return child.Update();
                            break;
                    }

                    child.state = State.Failure;
                    break;
                case Way.Back:
                    switch (distanceType)
                    {
                        case UpOrDown.Up:
                            if ((_actor.Direction == EActorDirection.Right && _actor.Position.x - x >= distance &&
                                 _actor.Position.x - x >= 0) ||
                                (_actor.Direction == EActorDirection.Left && x - _actor.Position.x >= distance &&
                                 x - _actor.Position.x >= 0))
                                return child.Update();

                            if (child.state == State.Running) return child.Update();
                            break;
                        case UpOrDown.Down:
                            if ((_actor.Direction == EActorDirection.Right && _actor.Position.x - x <= distance &&
                                 _actor.Position.x - x >= 0) ||
                                (_actor.Direction == EActorDirection.Left && x - _actor.Position.x <= distance &&
                                 x - _actor.Position.x >= 0))
                                return child.Update();

                            if (child.state == State.Running) return child.Update();
                            break;
                    }

                    child.state = State.Failure;
                    break;
            }

            return State.Failure;
        }

        public override bool Check()
        {
            target = GameObject.Find(objectName).transform;
            if (target == null) return false;
            var x = target.TryGetComponent(out Actor act) ? act.Position.x : target.position.x;

            switch (Method)
            {
                case Way.Front:
                    if ((_actor.Direction == EActorDirection.Left && _actor.Position.x - x <= distance &&
                         _actor.Position.x - x >= 0) ||
                        (_actor.Direction == EActorDirection.Right && x - _actor.Position.x <= distance &&
                         x - _actor.Position.x >= 0))
                        return CheckChild;
                    break;
                case Way.Back:
                    if ((_actor.Direction == EActorDirection.Right && _actor.Position.x - x <= distance &&
                         _actor.Position.x - x >= 0) ||
                        (_actor.Direction == EActorDirection.Left && x - _actor.Position.x <= distance &&
                         x - _actor.Position.x >= 0))
                        return CheckChild;
                    break;
            }

            return false;
        }
    }
}