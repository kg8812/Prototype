using Apis.SkillTrees;
using Save.Schema;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;

namespace Apis
{
    public abstract class PlayerActiveSkill : ActiveSkill , IPlayerSkill
    {
        public Player Player => user as Player;
        protected virtual TagManager.SkillTreeTag Tag => TagManager.SkillTreeTag.None;
        protected virtual float TagIncrement => 0;

        private PlayerSkillAttachment _attachment;

        public override UI_AtkItemIcon Icon => UI_MainHud.Instance.mainSkillIcon;

        public override bool TryUse()
        {
            return base.TryUse();
        }

        public override void Init()
        {
            base.Init();
            if (_attachment != null)
            {
                RemoveAttachment(_attachment);
            }
            _attachment = new(Tag,TagIncrement);
            AddAttachment(_attachment);
        }

        // 고유트리 방랑자 적용 함수
        public void Accept(ISkillVisitor visitor,int level)
        {
            visitor.Activate(this, level);
        }

        public override void Active()
        {
            base.Active();
            eventUser?.EventManager.ExecuteEvent(EventType.OnSkill,new EventParameters(eventUser)
            {
                skillData = new()
                {
                    usedSkill = this
                }
            });
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