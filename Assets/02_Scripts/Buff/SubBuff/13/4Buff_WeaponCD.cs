
namespace Apis
{
    public class Buff_WeaponCD : SubBuff
    {
        public Buff_WeaponCD(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.Buff_WeaponCD;

        public override void OnAdd()
        {
            base.OnAdd();
            if (actor is Player player)
            {
                //player.WeaponCdAdd(amount[0]);
            }
        }

        public override void TempApply(EventParameters parameters)
        {
            base.TempApply(parameters);
            if (actor is Player player)
            {
                //player.WeaponCdAdd(amount[0]);
            }
        }

        public override void PermanentApply()
        {
            base.PermanentApply();
            if (actor is Player player)
            {
                //player.WeaponCdAdd(amount[0]);
            }
        }
    }
}