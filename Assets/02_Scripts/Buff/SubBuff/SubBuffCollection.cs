using System.Collections.Generic;

namespace Apis
{
    public abstract class SubBuffCollection : ISubject<List<SubBuff>>
    {
        private List<IObserver<List<SubBuff>>> _observers;
        protected List<IObserver<List<SubBuff>>> observers => _observers ??= new();

        protected List<SubBuff> list;

        public List<SubBuff> List
        {
            get => list;
            protected set => list = value;
        }
        protected readonly IBuffCollectionUpdate stackDecrease;
         readonly IBuffUpdate buffUpdate;

        protected readonly Actor actor;
        public readonly Buff buff;
        public int Count => list?.Count ?? 0;

        public float CurTime
        {
            get => stackDecrease.CurTime;
            set => stackDecrease.CurTime = value;
        }
        public float Duration => stackDecrease.Duration;

        public SubBuffCollection(Buff buff, Actor actor)
        {
            this.actor = actor;
            this.buff = buff;

            stackDecrease = new SingleStackDecrease(this, buff.BuffDuration);

            SubBuff subBuff = SubBuffResources.Get(buff);

            if (subBuff is Debuff_DotDmg)
            {
                buffUpdate = new DotDmgUpdate(list, actor);
            }
            else
            {
                buffUpdate = new BuffNoUpdate();
            }
            Attach(stackDecrease);
            Attach(buffUpdate);
        }

        public void Attach(IObserver<List<SubBuff>> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
        }

        public void Detach(IObserver<List<SubBuff>> observer)
        {
            observers.Remove(observer);
        }

        public virtual void NotifyObservers()
        {
            foreach (var x in observers)
            {
                x.Notify(list);
            }           
        }

        public abstract void Add(SubBuff buff);
        public abstract bool RemoveSubBuff(SubBuff subBuff);
        public abstract SubBuff RemoveSubBuff();
        public abstract void Clear();
        public virtual void Update()
        {
            stackDecrease.Update();
            buffUpdate.Update();
            foreach(var x in list)
            {
                x.Update();
            }
        }       
    }
}