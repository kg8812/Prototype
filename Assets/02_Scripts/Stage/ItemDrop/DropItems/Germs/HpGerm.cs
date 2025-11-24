namespace Apis
{
    public class HpGerm: Germ
    {
        public override void InvokeInteraction()
        {
            base.InvokeInteraction();
            GameManager.instance.Player.CurHp += Amount;
        }
    }
}