using Apis;

namespace Apis.CommonMonster2
{
    public partial class CommonMonster2
    {
        public override void StartStun(IEventUser actor, float duration)
        {
            SubBuffManager.AddCC(actor, SubBuffType.Debuff_Stun, duration);
            TryChangeMonsterState(MonsterState.CC);
        }

        public override void EndStun()
        {
            base.EndStun();
            if(!IsDead)
                IdleOn();
        }
        
    }
}