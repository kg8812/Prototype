using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public class SubBuffCollector
    {
        private UnityEvent<BuffInfo> _buffUIEvent;
        public UnityEvent<BuffInfo> buffUIEvent => _buffUIEvent ??= new();

        //버프 목록
        public IDictionary<SubBuffType, BuffList> uniqueBuffs = new Dictionary<SubBuffType, BuffList>();
        public IDictionary<SubBuffType, SubBuffTypeList> subBuffs = new Dictionary<SubBuffType,SubBuffTypeList>();

        readonly SubBuffManager manager;

        public SubBuffCollector(SubBuffManager buffManager)
        {
            manager = buffManager;          
        }

        public int Count(SubBuffType type)
        {
            int count = 0;
            if (uniqueBuffs.TryGetValue(type, value: out var buff))
            {
                count += buff.Count;
            }
            if (subBuffs.TryGetValue(type, out var subBuff))
            {
                count += subBuff.Count;
            }
            return count;
        }
        public void AddBuff(Buff buff, SubBuff subBuff)
        {
            if (buff == null || subBuff == null) return;
            
            subBuff.Actor = manager.User;
            switch (buff.BuffCategory)
            {
                
                case 0:
                    if (subBuffs.ContainsKey(subBuff.Type))
                    {
                        subBuffs[subBuff.Type].Add(subBuff);
                    }
                    else
                    {
                        SubBuffTypeList list = new(subBuff.Type,manager.User);
                        BuffInfo info = new() { typeList = list , buff = buff};

                        var temp = subBuffs.ToDictionary(kv => kv.Key, kv => kv.Value);
                        temp.Add(subBuff.Type, list);
                        subBuffs = temp;
                        list.Add(subBuff);
                        buffUIEvent.Invoke(info);
                    }
                    break;
                case 1:
                    if (uniqueBuffs.ContainsKey(subBuff.Type))
                    {
                        uniqueBuffs[subBuff.Type].Add(buff, subBuff);
                    }
                    else
                    {
                        BuffList list = new(manager.User);
                        var temp = uniqueBuffs.ToDictionary(kv => kv.Key, kv => kv.Value);
                        temp.Add(subBuff.Type,list);
                        uniqueBuffs = temp;
                        
                        list.Add(buff, subBuff);
                    }
                    break;
            }
        }

        public SubBuff AddSubBuff(SubBuffType Type,GameObject target)
        {
            SubBuff sub;
            if (subBuffs.ContainsKey(Type))
            {
                sub = subBuffs[Type].Add(target);
            }
            else
            {
                SubBuffTypeList list = new(Type, manager.User);
                var temp = subBuffs.ToDictionary(kv => kv.Key, kv => kv.Value);

                temp.Add(Type, list);

                BuffInfo info = new() { typeList = list, buff = temp[Type].dummyBuff };

                subBuffs = temp;
                sub = list.Add(target);
                buffUIEvent.Invoke(info);
            }

            return sub;
        }

        // 버프 제거 함수 : 버프 타입 입력
        public void RemoveSubBuff(Buff buff, SubBuff subBuff) // 특정 효과내의 특정 버프 제거
        {
            if (buff == null || subBuff == null) return;
            
            if (uniqueBuffs.ContainsKey(subBuff.Type))
            {
                uniqueBuffs[subBuff.Type].RemoveSubBuff(buff, subBuff);
            }
            else if (subBuffs.ContainsKey(subBuff.Type))
            {
                subBuffs[subBuff.Type].RemoveSubBuff();
            }
            EventParameters parameters = new(manager.User)
            {
                buffData = new(){removedSubBuff = subBuff},
            };
            manager.User.ExecuteEvent(EventType.OnSubBuffRemove, parameters);
        }
        public void RemoveSubBuff(Buff buff)
        {
            if (buff == null) return;
            
            EventParameters parameters = new(manager.User);

            foreach (var x in uniqueBuffs.Keys)
            {
                if (uniqueBuffs[x].buffs.ContainsKey(buff))
                {                   
                    parameters.buffData.removedSubBuff = uniqueBuffs[x].RemoveSubBuff(buff);
                    manager.User.ExecuteEvent(EventType.OnSubBuffRemove, parameters);

                    return;
                }
            }

            foreach (var x in subBuffs.Keys)
            {
                parameters.buffData.removedSubBuff = subBuffs[x].RemoveSubBuff(buff);
                manager.User.ExecuteEvent(EventType.OnSubBuffRemove, parameters);

                return;
            }
        }
      
        public void RemoveBuff(Buff buff) // 특정 효과 제거
        {
            if (buff == null) return;
            
            foreach(var x in uniqueBuffs.Keys)
            {
                if (!uniqueBuffs[x].buffs.ContainsKey(buff)) continue;
                uniqueBuffs[x].RemoveBuff(buff);
                if (uniqueBuffs[x].Count > 0) continue;
                var temp = uniqueBuffs.ToDictionary(kv => kv.Key, kv => kv.Value);
                temp.Remove(x);
                uniqueBuffs = temp;
            }

            foreach(var x in subBuffs.Keys)
            {
                subBuffs[x].RemoveBuff(buff);

                if (subBuffs[x].Count == 0)
                {
                    var temp = subBuffs.ToDictionary(kv => kv.Key, kv => kv.Value);
                    temp.Remove(x);
                    subBuffs = temp;
                }
            }

            EventParameters parameters = new(manager?.User)
            {
                buffData = new(){removedSubBuff = buff?.ActivatedSubBuff}
            };

            manager?.User.ExecuteEvent(EventType.OnSubBuffRemove, parameters);
        }
        public void RemoveType(SubBuffType type)
        {
            if(uniqueBuffs.ContainsKey(type))
            {
                uniqueBuffs[type].Clear();
                var temp = uniqueBuffs.ToDictionary(kv => kv.Key, kv => kv.Value);
                temp.Remove(type);
                uniqueBuffs = temp;
            }
            if(subBuffs.ContainsKey(type))
            {
                subBuffs[type].Clear();
                var temp = subBuffs.ToDictionary(kv => kv.Key, kv => kv.Value);
                temp.Remove(type);
                subBuffs = temp;
            }
        }

        public void RemoveType(SubBuffType type, int stack)
        {
            var count = stack;
            if (uniqueBuffs.ContainsKey(type))
            {
                while (count > 0 && uniqueBuffs[type].Count > 0)
                {
                    if (uniqueBuffs[type].Remove()) count--;
                }
            }

            if (!subBuffs.ContainsKey(type)) return;
            while (count > 0 && subBuffs[type].Count > 0)
            {
                if (subBuffs[type].RemoveSubBuff() != null)
                {
                    count--;
                }
            }

        }
        public void Clear()
        {
            foreach(var x in uniqueBuffs.Values)
            {
                x.Clear();
            }
            {
                var temp = uniqueBuffs.ToDictionary(kv => kv.Key, kv => kv.Value);
                temp.Clear();
                uniqueBuffs = temp;
            }
            foreach(var x in subBuffs.Values)
            {
                x.Clear();
            }
            {
                var temp = subBuffs.ToDictionary(kv => kv.Key, kv => kv.Value);
                temp.Clear();
                subBuffs = temp;
            }
        }
        public bool Contains(SubBuffType type)
        {
            if (uniqueBuffs.ContainsKey(type) && uniqueBuffs[type].Count > 0)
            {
                return true;
            }
            if (subBuffs.ContainsKey(type) && subBuffs[type].Count > 0)
            {
                return true;
            }
            return false;
        }
        public void Update()
        {
            foreach (var x in uniqueBuffs.Values)
            {
                x.Update();
            }
            foreach(var x in subBuffs.Values)
            {
                x.Update();
            }
        }       
    }

    public class BuffInfo
    {
        public Buff buff;
        public SubBuffList subList;
        public SubBuffTypeList typeList;
    }
}