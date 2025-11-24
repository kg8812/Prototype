namespace Apis
{
    public class Buff_AirDashCount : SubBuff
    {
        public Buff_AirDashCount(Buff buff) : base(buff)
        {
        }

        public override void OnAdd()
        {
            base.OnAdd();
            if (actor is Player player)
            {
                player.MaxAirDash++;
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            if (actor is Player player)
            {
                player.MaxAirDash--;
            }
        }

        public override void TempApply(EventParameters parameters)
        {
            base.TempApply(parameters);
            if (actor is Player player)
            {
                player.MaxAirDash++;
            }
        }

        public override void PermanentApply()
        {
            base.PermanentApply();
            
            if (actor is Player player)
            {
                player.MaxAirDash++;
            }
        }

        public override SubBuffType Type => SubBuffType.Buff_AirDashCount;
    }
}