using Apis;
using Apis.DataType;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis
{
    public class ActiveSkillItem: Item, IAttackItem
    {
        // protected new string name;
        // protected string flavourText;
        // protected string description;

        public int itemId { get; set; }

        public override int ItemId => itemId;
        // public override string Name => name;
        //
        // public override string FlavourText => flavourText;
        //
        // public override string Description => description;
        
        [ReadOnly][ShowInInspector]public ActiveSkill ActiveSkill { get; set; }

        private ActiveSkillItemDataType _data;
        
        public override void Init()
        {
            base.Init();
            if (Image == null && _data != null)
            {
                Image = Default.ResourceUtil.Load<Sprite>(_data.iconPath);
            }
            ActiveSkill.Init();
        }

        public override void Activate()
        {
        }

        public void BeforeAttack()
        {
            ActiveSkill.BeforeAttack();
        }

        public void UseAttack()
        {
            ActiveSkill.Use();
        }

        public bool TryAttack()
        {
            return ActiveSkill.TryUse();
        }
        public void WhenIconIsSet(UI_AtkItemIcon icon)
        {
            if (icon == null) return;
            
            icon.Skill = ActiveSkill;
        }

        public void EndAttack()
        {
            ActiveSkill?.DeActive();
        }

        protected override void OnEquip(IMonoBehaviour user)
        {
            base.OnEquip(user);
            ActiveSkill.Equip(user);
        }

        protected override void OnUnEquip()
        {
            base.OnUnEquip();
            ActiveSkill.UnEquip();
        }

        public override void Return()
        {
            base.Return();
            GameManager.Item.ActiveSkillItem.Return(this);
        }

        public AttackCategory Category => AttackCategory.One;
        
        public UI_AtkItemIcon Icon { get; set; }
        public int AtkSlotIndex { get; set; }
    }
}