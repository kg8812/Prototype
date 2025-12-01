using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public abstract class SubBuff
    {
        protected readonly float duration;

        public readonly UnityEvent<SubBuff> OnBuffAdd = new();
        public readonly UnityEvent<SubBuff> OnBuffRemove = new();
        protected IBuffUser _user;
        protected float[] amount;

        public Buff buff;

        private BonusStat stat;
        public GameObject target;

        public SubBuff(Buff buff)
        {
            duration = buff.BuffDuration;
            amount = buff.BuffPower;
            var dispelType = buff.BuffDispellType;
            this.buff = buff;
            _user = buff.subBuffUser;

            if (dispelType == 0) duration = 0;
        }

        public float Duration => duration;

        public IBuffUser User
        {
            get => _user;
            set => _user = value;
        }

        public float[] Amount => amount;
        public abstract SubBuffType Type { get; }

        public BonusStat Stat
        {
            get { return stat ??= new BonusStat(); }
        }

        public virtual void Update()
        {
        }

        public virtual void OnAdd()
        {
            if (_user.SubBuffCount(Type) == 1) OnTypeAdd();
            OnBuffAdd.Invoke(this);
            buff.dispell.OnAdd(buff);
        }

        public virtual void OnRemove()
        {
            if (_user.SubBuffCount(Type) == 0) OnTypeRemove();

            OnBuffRemove.Invoke(this);
            buff.dispell.OnRemove(buff);
        }

        public virtual void TempApply(EventParameters parameters)
        {
        }

        public virtual void PermanentApply()
        {
        }

        public virtual void OnMaxStack()
        {
        }

        protected virtual void OnTypeAdd()
        {
        }

        protected virtual void OnTypeRemove()
        {
        }

        protected void SpawnEffect(string address, Vector2 offset)
        {
            var effect = GameManager.Factory.Get(FactoryManager.FactoryType.Effect, address, _user.Position);

            effect.transform.SetParent(_user.transform);
            effect.gameObject.name = Type.ToString();
        }

        protected void RemoveEffect()
        {
            var effect = _user.transform.Find(Type.ToString())?.gameObject;
            GameManager.Factory.Return(effect);
        }
    }
}