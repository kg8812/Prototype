using EventData;

namespace Apis
{
    public abstract class MagicSkill : ActiveSkill
    {
        protected Weapon _weapon;
        public virtual bool isActive => true;

        public int Index => index;

        public override bool Use()
        {
            if (!base.Use()) return false;

            eventUser?.EventManager.ExecuteEvent(EventType.OnWeaponSkillUse,
                new EventParameters(GameManager.instance.Player)
                {
                    skillData = new SkillEventData { usedSkill = this }
                });

            return true;
        }

        public override void Active()
        {
            base.Active();
            if (isMotion) SetSkillType();
        }

        protected virtual void SetSkillType()
        {
            animator?.animator.SetInteger("WeaponSkillType", 1);
        }

        public virtual void Init(Weapon weapon)
        {
            Init();
            _weapon = weapon;
            Item = weapon;
        }

        public override void EndMotion()
        {
            base.EndMotion();
            if (!isMotion) return;
            animator?.animator.SetTrigger("WeaponSkillEnd");
        }

        public override void Decorate()
        {
            base.Decorate();
            weaponSkillUser?.WeaponSkillAttachments?.ForEach(x => stats = new SkillDecorator(stats, x));
        }

        public override void BeforeAttack()
        {
            base.BeforeAttack();
            animator.animator.SetInteger("WeaponSkillIndex", Index);
        }

        protected void SetAttackObject(AttackObject obj, ProjectileInfo info, float duration = 0)
        {
            obj.Init(attacker, new AtkItemCalculation(attacker as Actor, this, info.dmg), duration);
            obj.Init(info);
        }
    }
}