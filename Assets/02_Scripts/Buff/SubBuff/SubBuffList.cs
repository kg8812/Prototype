using System.Collections.Generic;
using System.Linq;

namespace Apis
{
    public class SubBuffList : SubBuffCollection, ISubject<SubBuffList>
    {
        protected readonly List<IObserver<SubBuffList>> observers2 = new();

        public SubBuffList(Buff buff, Actor actor) : base(buff, actor)
        {
            list = new List<SubBuff>();
        }

        public void Attach(IObserver<SubBuffList> observer)
        {
            observers2.Add(observer);
        }

        public void Detach(IObserver<SubBuffList> observer)
        {
            observers2.Remove(observer);
        }

        public override void NotifyObservers()
        {
            base.NotifyObservers();
            observers2.ForEach(x => x.Notify(this));
        }

        public override void Add(SubBuff subBuff)
        {
            list.Add(subBuff);

            stackDecrease.ResetTime();
            NotifyObservers();
        }

        public override bool RemoveSubBuff(SubBuff subBuff)
        {
            if (list != null && list.Contains(subBuff))
            {
                var temp = list.ToList();

                temp.Remove(subBuff);
                list = temp;
                NotifyObservers();
                subBuff.OnRemove();

                return true;
            }

            return false;
        }

        public override SubBuff RemoveSubBuff()
        {
            if (list != null && list.Count > 0)
            {
                var subBuff = list[0];

                if (buff.StackDecrease == 0)
                {
                    var temp = list.ToList();
                    temp.RemoveAt(0);
                    list = temp;
                    NotifyObservers();

                    subBuff.OnRemove();
                }
                else if (buff.StackDecrease == 1)
                {
                    Clear();
                }

                return subBuff;
            }

            return null;
        }

        public override void Clear()
        {
            var a = list.ToList();
            var temp = list.ToList();
            a.Clear();
            list = a;
            NotifyObservers();

            foreach (var x in temp) x.OnRemove();
        }
    }
}