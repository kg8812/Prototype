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
            Back,

            /// <summary>액터가 어느 쪽을 보든 상관없이 절대 거리로만 판정한다.</summary>
            Any
        }

        public UpOrDown distanceType;
        public Way Method;
        public float distance;
        public string objectName;

        private Transform target;

        public override void OnStart()
        {
            TryResolveTarget();
        }

        public override void OnStop()
        {
        }

        public override State OnUpdate()
        {
            if (child == null) return State.Failure;

            if (IsSatisfied()) return child.Update();

            // 조건을 벗어나도 이미 실행 중이던 자식은 끝까지 돌려준다
            if (child.state == State.Running) return child.Update();

            child.Abort();
            return State.Failure;
        }

        public override bool Check()
        {
            return IsSatisfied() && CheckChild;
        }

        /// <summary>
        ///     거리 조건 판정. OnUpdate와 Check가 갈라지지 않도록 반드시 이 함수만 사용할 것.
        /// </summary>
        private bool IsSatisfied()
        {
            if (!TryResolveTarget()) return false;

            var targetX = target.TryGetComponent(out Actor act) ? act.Position.x : target.position.x;
            var gap = targetX - _actor.Position.x;

            // 액터가 바라보는 방향을 +로 둔다. Left = -1, Right = 1 이므로 부호만 곱하면 된다
            switch (Method)
            {
                case Way.Front:
                    gap *= (int)_actor.Direction;
                    break;
                case Way.Back:
                    gap *= -(int)_actor.Direction;
                    break;
                case Way.Any:
                    // 방향을 보지 않으므로 몸을 돌려도 판정이 뒤집히지 않는다
                    gap = Mathf.Abs(gap);
                    break;
            }

            if (gap < 0) return false; // 지정한 쪽(앞/뒤)에 없음

            return distanceType == UpOrDown.Up ? gap >= distance : gap <= distance;
        }

        private bool TryResolveTarget()
        {
            if (target != null) return true;
            if (string.IsNullOrEmpty(objectName)) return false;

            // 파괴됐거나 아직 없는 경우 다음 프레임에 다시 시도한다
            var found = GameObject.Find(objectName);
            target = found == null ? null : found.transform;

            return target != null;
        }
    }
}
