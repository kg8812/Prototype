namespace Apis
{
    public class BossAttackCollider : AttackObject
    {
        protected BossMonster boss;

        protected override void Awake()
        {
            base.Awake();
            boss = GetComponentInParent<BossMonster>();
        }

        public override void Init(AttackObjectInfo atkObjectInfo)
        {
            base.Init(atkObjectInfo);

            if (_attacker is BossMonster b) boss = b;
        }
    }
}