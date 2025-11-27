using Apis.Managers;
using NewNewInvenSpace;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public class ActiveSkill_PickUp : DropItem
    {
        // public string activeSkillName;
        public int activeSkillItemId;

        private UnityEvent<ActiveSkill_PickUp> _onCollect;

        private ActiveSkillItem item; // 획득할 아이템
        private SpriteRenderer render;
        public UnityEvent<ActiveSkill_PickUp> OnCollect => _onCollect ??= new UnityEvent<ActiveSkill_PickUp>();

        private void Awake()
        {
            isInteractable = true;
            render = GetComponentInChildren<SpriteRenderer>();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            OnCollect.RemoveAllListeners();
        }

        public override void InvokeInteraction()
        {
            if (item == null) item = GameManager.Item.GetActiveSkill(activeSkillItemId);
            var addInd = InvenManager.instance.AttackItem.Invens[InvenType.Storage].GetEmptySlot();
            if (addInd < 0) return;
            InvenManager.instance.AttackItem.Add(addInd, item, InvenType.Storage);

            // TODO: 마법도 codex 해금 요소 만들어야 함.
            // DataAccess.Codex.UnLock(CodexData.CodexType.Item,item.Index);
            OnCollect.Invoke(this);
            GameManager.Item.ActiveSkillPickUp.Return(this);
        }

        public void CreateNew(int skillItem)
        {
            item = GameManager.Item.ActiveSkillItem.CreateNew(skillItem);
            item.SetParent(transform);

            render.sprite = item.Image;
        }

        public void CreateExisting(ActiveSkillItem item)
        {
            this.item = item;
            this.item.SetParent(transform);
            render.sprite = item.Image;
        }

        protected override void ReturnObject(SceneData _)
        {
            GameManager.Item.ActiveSkillPickUp.Return(this);
        }
    }
}