using System.Collections.Generic;

namespace Apis
{
    public abstract class SubBuffCollection : ISubject<List<SubBuff>>
    {
        protected readonly IBuffUser _user;
        public readonly Buff buff;
        private readonly IBuffUpdate buffUpdate;
        protected readonly IBuffCollectionUpdate stackDecrease;
        private List<IObserver<List<SubBuff>>> _observers;

        protected List<SubBuff> list;

        public SubBuffCollection(Buff buff, IBuffUser user)
        {
            _user = user;
            this.buff = buff;

            stackDecrease = new SingleStackDecrease(this, buff.BuffDuration);

            var subBuff = SubBuffResources.Get(buff);

            if (subBuff is Debuff_DotDmg)
                buffUpdate = new DotDmgUpdate(list, user);
            else
                buffUpdate = new BuffNoUpdate();
            Attach(stackDecrease);
            Attach(buffUpdate);
        }

        protected List<IObserver<List<SubBuff>>> observers => _observers ??= new List<IObserver<List<SubBuff>>>();

        public List<SubBuff> List
        {
            get => list;
            protected set => list = value;
        }

        public int Count => list?.Count ?? 0;

        public float CurTime
        {
            get => stackDecrease.CurTime;
            set => stackDecrease.CurTime = value;
        }

        public float Duration => stackDecrease.Duration;

        public void Attach(IObserver<List<SubBuff>> observer)
        {
            if (!observers.Contains(observer)) observers.Add(observer);
        }

        public void Detach(IObserver<List<SubBuff>> observer)
        {
            observers.Remove(observer);
        }

        public virtual void NotifyObservers()
        {
            foreach (var x in observers) x.Notify(list);
        }

        public abstract void Add(SubBuff buff);
        public abstract bool RemoveSubBuff(SubBuff subBuff);
        public abstract SubBuff RemoveSubBuff();
        public abstract void Clear();

        public virtual void Update()
        {
            stackDecrease.Update();
            buffUpdate.Update();
            foreach (var x in list) x.Update();
        }
    }
}