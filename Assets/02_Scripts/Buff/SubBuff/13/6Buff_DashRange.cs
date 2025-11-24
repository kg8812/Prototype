namespace Apis
{
    public class Buff_DashRange : SubBuff
    {
        public Buff_DashRange(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.Buff_DashRange;

        public override void OnAdd()
        {
            base.OnAdd();
            if (actor is Player player)
            {
                switch (buff.ValueType)
                {
                    case ValueType.Value:
                        player.DashSpeed += amount[0];
                        break;
                    case ValueType.Ratio:
                        player.DashSpeed *= 1 + amount[0] / 100f;
                        break;
                }
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            if (actor is Player player)
            {
                switch (buff.ValueType)
                {
                    case ValueType.Value:
                        player.DashSpeed -= amount[0];
                        break;
                    case ValueType.Ratio:
                        player.DashSpeed -= 1 + amount[0] / 100;
                        break;
                }
            }
        }

        public override void PermanentApply()
        {
            base.PermanentApply();
            if (actor is Player player)
            {
                switch (buff.ValueType)
                {
                    case ValueType.Value:
                        player.DashSpeed += amount[0];
                        break;
                    case ValueType.Ratio:
                        player.DashSpeed *= 1 + amount[0] / 100;
                        break;
                }
            }
        }
    }
}