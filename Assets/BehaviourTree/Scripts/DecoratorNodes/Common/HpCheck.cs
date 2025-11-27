namespace Apis.BehaviourTreeTool
{
    public class HpCheck : CommonDecoratorNode
    {
        public enum UpOrDown
        {
            Up,
            Down
        }

        public float hpPercent;
        public UpOrDown upOrDown;

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override State OnUpdate()
        {
            var hp = _actor.CurHp / _actor.MaxHp;

            switch (upOrDown)
            {
                case UpOrDown.Up:
                    if (hp > hpPercent / 100f) return child.Update();
                    break;
                case UpOrDown.Down:
                    if (hp < hpPercent / 100f) return child.Update();
                    break;
            }

            return State.Failure;
        }

        public override bool Check()
        {
            var hp = _actor.CurHp / _actor.MaxHp;

            switch (upOrDown)
            {
                case UpOrDown.Up:
                    if (hp > hpPercent / 100f) return CheckChild;
                    break;
                case UpOrDown.Down:
                    if (hp < hpPercent / 100f) return CheckChild;
                    break;
            }

            return false;
        }
    }
}