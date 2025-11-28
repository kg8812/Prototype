using Apis.SkillTrees;

namespace Apis
{
    public abstract class PlayerPassiveSkill : PassiveSkill, IPlayerSkill
    {
        private PlayerSkillAttachment _attachment;
        public Player Player => GameManager.instance.Player;

        protected abstract float TagIncrement { get; }

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
    }
}