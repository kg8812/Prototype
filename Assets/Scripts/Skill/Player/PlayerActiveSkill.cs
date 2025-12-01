using Apis.SkillTrees;
using EventData;
using UI;

namespace Apis
{
    public abstract class PlayerActiveSkill : ActiveSkill, IPlayerSkill
    {
        private PlayerSkillAttachment _attachment;
        public Player Player => user as Player;
        protected virtual float TagIncrement => 0;

        public override UI_AtkItemIcon Icon => UI_MainHud.Instance.mainSkillIcon;

        // 고유트리 방랑자 적용 함수
        public void Accept(ISkillVisitor visitor, int level)
        {
            visitor.Activate(this, level);
        }

        public override bool TryUse()
        {
            return base.TryUse();
        }

        public override void Init()
        {
            base.Init();
            if (_attachment != null) RemoveAttachment(_attachment);
            _attachment = new PlayerSkillAttachment();
            AddAttachment(_attachment);
        }

        public override void Active()
        {
            base.Active();
            EventParameters parameters = new(eventUser);
            parameters.Set(new SkillEventData
            {
                usedSkill = this
            });
            eventUser?.EventManager.ExecuteEvent(EventType.OnSkill, parameters);
        }

        public override void EndMotion()
        {
            base.EndMotion();

            animator?.animator.SetTrigger("PlayerSkillEnd");
        }

        protected override void OnEquip(IMonoBehaviour owner)
        {
            base.OnEquip(owner);
            Icon.WhenItemIsSet();
            Icon.Skill = this;
        }

        public override void AfterDuration()
        {
            base.AfterDuration();
            eventUser?.EventManager.ExecuteEvent(EventType.OnSkillEnd, new EventParameters(eventUser));
        }

        public override void Cancel()
        {
            base.Cancel();
            animator?.animator.ResetTrigger("PlayerSkill");
        }

        public override void Decorate()
        {
            base.Decorate();
            activeUser?.ActiveAttachments?.ForEach(x => stats = new SkillDecorator(stats, x));
        }
    }
}