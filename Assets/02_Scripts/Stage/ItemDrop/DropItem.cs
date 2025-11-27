using System;
using Apis.Managers;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Apis
{
    public enum DropItemType
    {
        Accessory,
        ActiveSkill,
        Weapon
    }

    public abstract class DropItem : MonoBehaviour, IOnInteract
    {
        // public int dropItemId;
        public Rigidbody2D rigid;

        public UnityEvent InteractionEvent;

        protected bool isInteractable;


        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        public Func<bool> InteractCheckEvent { get; set; }

        public void OnInteract()
        {
            InteractionEvent?.Invoke();
            InvokeInteraction();
        }

        private bool Check()
        {
            return isInteractable;
        }

        public abstract void InvokeInteraction();

        protected void Init()
        {
            rigid = GetComponent<Rigidbody2D>();
            InteractCheckEvent -= Check;
            InteractCheckEvent += Check;
        }

        public virtual void Dropping()
        {
            Init();
            var vx = Random.Range(-2f, 2f);
            var vy = Random.Range(2f, 4f);
            rigid.linearVelocity = new Vector2(vx, vy);

            // 드롭아이템 통일 스폰 물리 효과
            Invoke(nameof(Droped), 1f);
        }

        //when 바닥에 닿았을 떄
        protected virtual void Droped()
        {
        }

        protected virtual void ReturnObject(SceneData _)
        {
        }
    }
}