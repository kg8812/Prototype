using Apis.Managers;
using NewNewInvenSpace;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public class Weapon_PickUp : DropItem
    {
        [HideInInspector] public UnityEvent<Weapon_PickUp> OnCollect = new();

        // public string weaponName;
        public int weaponId;

        private SpriteRenderer render;
        // 무기 픽업 클래스

        // 획득할 무기
        public Weapon Weapon { get; private set; }

        private void Awake()
        {
            isInteractable = true;
            render = GetComponent<SpriteRenderer>();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            OnCollect.RemoveAllListeners();
        }

        public override void InvokeInteraction()
        {
            var addInd = InvenManager.instance.AttackItem.Invens[InvenType.Storage].GetEmptySlot();
            if (addInd < 0) return;

            if (Weapon == null) Weapon = GameManager.Item.GetWeapon(weaponId);
            InvenManager.instance.AttackItem.Add(addInd, Weapon, InvenType.Storage);
            OnCollect.Invoke(this);
            GameManager.Item.WeaponPickUp.Return(this);
        }

        // 획득할 무기 넣기
        public void CreateNew(Weapon weapon)
        {
            if (weapon == null) return;

            this.Weapon = GameManager.Item.Weapon.CreateNew(weapon.ItemId);
            if (this.Weapon == null) return;

            this.Weapon.SetParent(transform);
            render.sprite = this.Weapon.Image;
        }

        public void CreateAlready(Weapon weapon)
        {
            this.Weapon = weapon;
            if (weapon != null) weapon.SetParent(transform);
        }


        protected override void ReturnObject(SceneData _)
        {
            Weapon = null;
            GameManager.Item.WeaponPickUp.Return(this);
        }
    }
}