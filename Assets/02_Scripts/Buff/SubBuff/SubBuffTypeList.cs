using Apis.DataType;
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace Apis
{
    public class SubSingleStack : IBuffCollectionUpdate
    {
        public float Duration
        {
            get; set;
        }
        readonly SubBuffTypeList subBuffList;
        public float CurTime
        {
            get; set;
        }

        public SubSingleStack(SubBuffTypeList subBuffList)
        {
            this.subBuffList = subBuffList;
            Duration = subBuffList.Duration;
            CurTime = Duration;
        }
        public void Notify(List<SubBuff> value)
        {
        }

        public void ResetTime()
        {
            CurTime = Duration;
        }

        public void Update()
        {
            if (Duration < 0 || Mathf.Approximately(Duration,0)) return;

            if (CurTime > 0)
            {
                CurTime -= Time.deltaTime;
            }

            if (CurTime < 0)
            {
                subBuffList.RemoveSubBuff();
                CurTime = Duration;
            }
        }
    }
    public class SubBuffTypeList : ISubject<List<SubBuff>>
    {
        public CustomQueue<SubBuff> queue = new ();

        readonly IBuffCollectionUpdate _stackStrategy;
        readonly IBuffUpdate _buffUpdate;
        readonly float _duration;
        public float CurTime => _stackStrategy.CurTime;
        public float Duration => _duration;

        List<SubBuff> list;
        public List<SubBuff> List => list ??= new();
        readonly int maxStack;
        readonly Actor actor;
        public readonly SubBuffOptionDataType option;
        readonly SubBuffType _type;

        public int Count
        {
            get
            {
                try
                {
                    return queue.Count;
                }
                catch
                {
                    return 0;
                }
            }
        }
        
         List<IObserver<List<SubBuff>>> _observers;
         List<IObserver<List<SubBuff>>> Observers => _observers ??= new();
        public void Attach(IObserver<List<SubBuff>> observer)
        {
            if (!Observers.Contains(observer))
            {
                Observers.Add(observer);
            }
        }

        public void Detach(IObserver<List<SubBuff>> observer)
        {
            Observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            foreach (var x in Observers)
            {
                x.Notify(list);
            }
        }

        public readonly Buff dummyBuff;
        
        public SubBuffTypeList(SubBuffType type, Actor actor)
        {
            BuffDatabase.DataLoad.TryGetSubBuffIndex(type, out int index);
            BuffDatabase.DataLoad.TryGetSubBuffOption(index, out option);

            _type = type;
            maxStack = option.maxStack;
            _duration = option.duration;
            this.actor = actor;

            _stackStrategy = new SubSingleStack(this);           

            string str = "Apis." + type;
            Type tp = Type.GetType(str);

            if (tp != null && tp.IsSubclassOf(typeof(Debuff_DotDmg)))
            {
                _buffUpdate = new DotDmgUpdate(list, actor);
            }
            else
            {
                _buffUpdate = new BuffNoUpdate();
            }
            Attach(_stackStrategy);
            Attach(_buffUpdate);

            BuffDataType data = new(type)
            {
                buffPower = option.amount, buffDuration = _duration,buffDispellType = 1,stackDecrease = option.stackDecrease,
                showIcon = option.showIcon
            };
            
            dummyBuff = new(data, null);
        }

        void AddSub(SubBuff subBuff)
        {
            bool wasMaxStack = false;

            if (maxStack > 0 && Count >= maxStack)
            {
                wasMaxStack = true;
                SubBuff temp = queue.Dequeue();
                temp.OnRemove();
            }
            queue.Enqueue(subBuff);
            ResetList();
            subBuff.OnAdd();
            if (maxStack > 0 && Count >= maxStack && !wasMaxStack)
            {
                subBuff.OnMaxStack();
            }
        }
        public void Add(SubBuff subBuff)
        {
            AddSub(subBuff);
        }

        public SubBuff Add(GameObject target)
        {
            _stackStrategy.CurTime = _duration;
            SubBuff sub = SubBuffResources.Get(dummyBuff);
            if (sub == null) return null;
            sub.Actor = actor;
            sub.target = target;
            AddSub(sub);
            
            return sub;
        }
        
        public SubBuff RemoveSubBuff()
        {
            if (Count <= 0) return null;
            SubBuff subBuff = queue.Dequeue();
            switch (option.stackDecrease)
            {
                case 0:
                    ResetList();
                    subBuff.OnRemove();
                    return subBuff;
                case 1:
                    Clear();
                    return subBuff;
                default:
                    return null;
            }
        }

        public SubBuff RemoveSubBuff(Buff buff)
        {
            SubBuff sub = null;
            foreach (var x in queue)
            {
                if (x.buff == buff)
                {
                    sub = x;
                    break;
                }
            }

            queue.Remove(sub);
            ResetList();
            sub?.OnRemove();
            return sub;
        }
        public void RemoveBuff(Buff buff)
        {
            List<SubBuff> removeList = new();
            queue.ForEach(x =>
            {
                if (x.buff == buff)
                {
                    removeList.Add(x);
                }
            });
            if (removeList.Count == 0) return;
            
            removeList.ForEach(x =>
            {
                queue.Remove(x);
                x.OnRemove();
            });
        }
        public void Clear()
        {
            queue.Clear();

            var tempList = list?.ToList();
            ResetList();

            if (tempList != null)
            {
                foreach (var x in tempList)
                {
                    x.OnRemove();
                }
            }
           
        }
        void ResetList()
        {
            list = queue.ToList();
            if (list.Count == 0)
            {
                var temp = actor.SubBuffManager.Collector.subBuffs.ToDictionary(kv => kv.Key,kv => kv.Value);
                temp.Remove(_type);
                actor.SubBuffManager.Collector.subBuffs = temp;
            }
            NotifyObservers();
        }

        public void Update()
        {
            _stackStrategy.Update();
            _buffUpdate.Update();

            foreach (var x in list)
            {
                x.Update();
            }
        }
    }
}