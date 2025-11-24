using System.Collections.Generic;
using System.Linq;

namespace Apis
{
    public class BuffList : ISubject<BuffList>,IObserver<SubBuffList>
    {
        readonly Actor actor;

        public Dictionary<Buff, SubBuffList> buffs = new Dictionary<Buff, SubBuffList>();

        public BuffList(Actor actor)
        {
            this.actor = actor;
        }

        public int Count // 서브버프 개수
        {
            get
            {
                int count = 0;

                foreach(var item in buffs.Values)
                {
                    count += item.Count;
                }
                return count;
            }
        }
        public List<SubBuff> this[Buff buff] => buffs[buff].List;

        void AddSub(Buff buff,SubBuff subBuff,Dictionary<Buff,SubBuffList> temp)
        {
            bool wasMaxStack = false;
            if (buff.BuffMaxStack > 0 && buff.BuffMaxStack <= temp[buff].Count)
            {
                var sub = temp[buff].List[0];
                temp[buff].List.RemoveAt(0);
                sub.OnRemove();
                wasMaxStack = true;
            }
            temp[buff].Add(subBuff);
            subBuff.OnAdd();

            if (buff.BuffMaxStack > 0 && buff.BuffMaxStack <= temp[buff].Count && !wasMaxStack)
            {
                subBuff.OnMaxStack();
            }
            
        }
        public void Add(Buff buff, SubBuff subBuff)
        {
            Buff b = buffs.Keys.FirstOrDefault(x => x.BuffIndex == buff.BuffIndex);

            if (b == null)
            {
                var temp = buffs.ToDictionary(kv => kv.Key, kv => kv.Value);
                temp.Add(buff, new SubBuffList(buff, actor));
                buffs = temp;

                temp[buff].CurTime = temp[buff].Duration;

                AddSub(buff,subBuff,temp);
                temp[buff].Attach(this);
                NotifyObservers();

                BuffInfo info = new() { subList = temp[buff], buff = buff };
                actor.SubBuffManager.Collector.buffUIEvent.Invoke(info);
            }
            else
            {
                buffs[b].CurTime = buffs[b].Duration;
                AddSub(b,subBuff,buffs);
                NotifyObservers();
            }

        }

        public bool RemoveSubBuff(Buff buff, SubBuff subBuff)
        {
            if (buffs.ContainsKey(buff))
            {
                bool success = buffs[buff].RemoveSubBuff(subBuff);
               
                return success;
            }

            return false;
        }

        public SubBuff RemoveSubBuff(Buff buff)
        {
            if (buffs.ContainsKey(buff))
            {
                SubBuff subBuff = buffs[buff].RemoveSubBuff();
                
                return subBuff;
            }

            return null;
        }

        public bool RemoveBuff(Buff buff)
        {
            if (buffs.ContainsKey(buff))
            {
                buffs[buff].Clear();
                return true;
            }

            return false;
        }
        public bool Remove()
        {
            foreach (var x in buffs.Keys)
            {
                if (buffs[x].Count <= 0) continue;
                buffs[x].RemoveSubBuff();
                return true;
            }

            return false;
        }
        public void Clear()
        {
            foreach (var x in buffs.Keys)
            {
                buffs[x].Clear();
            }
            buffs.Clear();
        }
        public void Update()
        {          
            foreach (var x in buffs.Keys)
            {
                buffs[x].Update();
            }
        }

        private List<IObserver<BuffList>> _observers;
        List<IObserver<BuffList>> observers => _observers ??= new();
        public void Attach(IObserver<BuffList> observer)
        {
            observers.Add(observer);
        }

        public void Detach(IObserver<BuffList> observer)
        {
            observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            observers.ForEach(x => x.Notify(this));
        }

        
        public void Notify(SubBuffList value)
        {
            if (value != null && value.Count == 0)
            {
                var temp = buffs.ToDictionary(kv => kv.Key, kv => kv.Value);
                temp.Remove(value.buff);
                buffs = temp;
            }

            NotifyObservers();
        }
    }
}