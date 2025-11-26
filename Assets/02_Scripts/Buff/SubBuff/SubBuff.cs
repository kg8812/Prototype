using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public abstract class SubBuff
    {
       
        protected readonly float duration;
        public float Duration => duration;
        protected Actor actor;
        public GameObject target;
        protected float[] amount;
        public Actor Actor { get => actor;
            set => actor = value;
        }
        public float[] Amount => amount;
        public abstract SubBuffType Type { get; }
        
        private BonusStat stat;
        public BonusStat Stat
        {
            get
            {
                return stat ??= new();
            }
        }

        public virtual void Update()
        {            
            
        }

        public Buff buff;
       
        public SubBuff(Buff buff)
        {
            duration = buff.BuffDuration;
            amount = buff.BuffPower;
            var dispelType = buff.BuffDispellType;
            this.buff = buff;
            actor = buff.subBuffActor;
            
            if (dispelType == 0) duration = 0;
        }
        
        public virtual void OnAdd() 
        {
            if (actor.SubBuffCount(Type) == 1)
            {
                OnTypeAdd();
            }
            OnBuffAdd.Invoke(this);
            buff.dispell.OnAdd(buff);
        }
        public virtual void OnRemove()
        {
            if (actor.SubBuffCount(Type) == 0)
            {
                OnTypeRemove();
            }
            
            OnBuffRemove.Invoke(this);
            buff.dispell.OnRemove(buff);
        }
        
        public virtual void TempApply(EventParameters parameters) { }
        public virtual void PermanentApply() { }

        public virtual void OnMaxStack()
        {
        }

        protected virtual void OnTypeAdd()
        {
        }

        protected virtual void OnTypeRemove()
        {
        }

        public readonly UnityEvent<SubBuff> OnBuffAdd = new();
        public readonly UnityEvent<SubBuff> OnBuffRemove = new();

        protected void SpawnEffect(string address, Vector2 offset)
        {
            GameObject effect = GameManager.Factory.Get(FactoryManager.FactoryType.Effect, address, actor.Position);
            
            effect.transform.SetParent(actor.transform);
            effect.gameObject.name = Type.ToString();
        }

        protected void RemoveEffect()
        {
            GameObject effect = actor.transform.Find(Type.ToString())?.gameObject;
            GameManager.Factory.Return(effect);
        }
    }
}
