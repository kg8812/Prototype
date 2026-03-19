using System;
using System.Collections.Generic;
using System.Linq;
using EventData;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{

    // 현재 책임 : 여러 버프 컬렉션을 통합 관리하고, 추가/제거/조회 요청을 분배하는 상위 저장소
    // 목표 책임 : 하위 컬렉션들을 조합해서 버프를 저장/조회/삭제만 하는 순수 저장소 허브
    
    public class SubBuffCollector
    {
        private readonly SubBuffManager manager;
        private IDictionary<SubBuffType, SubBuffTypeList> _subBuffs = new Dictionary<SubBuffType, SubBuffTypeList>();

        //버프 목록
        private IDictionary<SubBuffType, BuffList> _uniqueBuffs = new Dictionary<SubBuffType, BuffList>();

        public SubBuffCollector(SubBuffManager buffManager)
        {
            manager = buffManager;
        }


        public int Count(SubBuffType type)
        {
            var count = 0;
            if (_uniqueBuffs.TryGetValue(type, out var buff)) count += buff.Count;
            if (_subBuffs.TryGetValue(type, out var subBuff)) count += subBuff.Count;
            return count;
        }

        

        private bool HasEmptyTypeList()
        {
            foreach (var x in _subBuffs.Values)
            {
                if (x.Count == 0)
                    return true;
            }

            return false;
        }
        
        void CleanUpEmptyTypeLists()
        {
            var temp = _subBuffs
                .Where(kv => kv.Value.Count > 0)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            _subBuffs = temp;
        }
        
        public void Traverse(Action<SubBuff> action)
        {
            foreach (var x in _uniqueBuffs.Values)
            foreach (var y in x.buffs.Keys)
            foreach (var z in x[y])
                action(z);

            foreach (var x in _subBuffs.Values)
            foreach (var y in x.List)
                action(y);
        }
        
        public AddBuffResult AddBuff(Buff buff, SubBuff subBuff)
        {
            if (buff == null || subBuff == null) return default;

            subBuff.User = manager.User;
            switch (buff.BuffCategory)
            {
                case 0:
                    if (_subBuffs.ContainsKey(subBuff.Type))
                    {
                        _subBuffs[subBuff.Type].Add(subBuff);
                        return new AddBuffResult(buff);
                    }
                    else
                    {
                        SubBuffTypeList list = new(subBuff.Type, manager.User);

                        var temp = _subBuffs.ToDictionary(kv => kv.Key, kv => kv.Value);
                        temp.Add(subBuff.Type, list);
                        _subBuffs = temp;
                        list.Add(subBuff);
                        return new AddBuffResult(
                            buff,
                            createdTypeList: true,
                            typeList: list);
                    }

                case 1:
                    if (_uniqueBuffs.ContainsKey(subBuff.Type))
                    {
                        var createdList = _uniqueBuffs[subBuff.Type].Add(buff, subBuff);

                        if (createdList != null)
                        {
                            return new AddBuffResult(
                                buff,
                                createdUniqueList: true,
                                uniqueList: createdList);
                        }

                        return new AddBuffResult(buff);
                    }
                    else
                    {
                        BuffList list = new(manager.User);
                        var temp = _uniqueBuffs.ToDictionary(kv => kv.Key, kv => kv.Value);
                        temp.Add(subBuff.Type, list);
                        _uniqueBuffs = temp;

                        var createdList = list.Add(buff, subBuff);

                        return new AddBuffResult(
                            buff,
                            createdUniqueList: createdList != null,
                            uniqueList: createdList);
                    }
            }

            return default;
        }

        public AddTypeSubBuffResult AddSubBuff(SubBuffType Type, GameObject target)
        {
            
            if (_subBuffs.ContainsKey(Type))
            {
                var sub = _subBuffs[Type].Add(target);
                return new AddTypeSubBuffResult(sub);
            }

            SubBuffTypeList list = new(Type, manager.User);
            var temp = _subBuffs.ToDictionary(kv => kv.Key, kv => kv.Value);

            temp.Add(Type, list);

            SubBuff subBuff = list.Add(target);
            
            _subBuffs = temp;

            return new AddTypeSubBuffResult(
                subBuff,
                createdTypeList: true,
                typeList: list);
        }

        // 버프 제거 함수 : 버프 타입 입력
        public bool RemoveSubBuff(Buff buff, SubBuff subBuff) // 특정 효과내의 특정 버프 제거
        {
            if (buff == null || subBuff == null) return false;

            if (_uniqueBuffs.ContainsKey(subBuff.Type))
                _uniqueBuffs[subBuff.Type].RemoveSubBuff(buff, subBuff);
            else if (_subBuffs.ContainsKey(subBuff.Type)) _subBuffs[subBuff.Type].RemoveSubBuff();

            return true;
        }

        public SubBuff RemoveSubBuff(Buff buff)
        {
            if (buff == null) return null;


            foreach (var x in _uniqueBuffs.Keys)
            {
                if (_uniqueBuffs[x].buffs.ContainsKey(buff))
                {
                    return _uniqueBuffs[x].RemoveSubBuff(buff);
                }
            }

            foreach (var x in _subBuffs.Keys)
            {
                var removed = _subBuffs[x].RemoveSubBuff(buff);
                if (removed != null)
                    return removed;
            }

            return null;

        }

        public bool RemoveBuff(Buff buff) // 특정 효과 제거
        {
            if (buff == null) return false;

            foreach (var x in _uniqueBuffs.Keys)
            {
                if (!_uniqueBuffs[x].buffs.ContainsKey(buff)) continue;
                _uniqueBuffs[x].RemoveBuff(buff);
                if (_uniqueBuffs[x].Count > 0) continue;
                var temp = _uniqueBuffs.ToDictionary(kv => kv.Key, kv => kv.Value);
                temp.Remove(x);
                _uniqueBuffs = temp;
            }

            foreach (var x in _subBuffs.Keys)
            {
                _subBuffs[x].RemoveBuff(buff);

                if (_subBuffs[x].Count == 0)
                {
                    var temp = _subBuffs.ToDictionary(kv => kv.Key, kv => kv.Value);
                    temp.Remove(x);
                    _subBuffs = temp;
                }
            }

            return true;
        }

        public void RemoveType(SubBuffType type)
        {
            if (_uniqueBuffs.ContainsKey(type))
            {
                _uniqueBuffs[type].Clear();
                var temp = _uniqueBuffs.ToDictionary(kv => kv.Key, kv => kv.Value);
                temp.Remove(type);
                _uniqueBuffs = temp;
            }

            if (_subBuffs.ContainsKey(type))
            {
                _subBuffs[type].Clear();
                var temp = _subBuffs.ToDictionary(kv => kv.Key, kv => kv.Value);
                temp.Remove(type);
                _subBuffs = temp;
            }
        }

        public void RemoveType(SubBuffType type, int stack)
        {
            var count = stack;
            if (_uniqueBuffs.ContainsKey(type))
                while (count > 0 && _uniqueBuffs[type].Count > 0)
                    if (_uniqueBuffs[type].Remove())
                        count--;

            if (!_subBuffs.ContainsKey(type)) return;
            while (count > 0 && _subBuffs[type].Count > 0)
                if (_subBuffs[type].RemoveSubBuff() != null)
                    count--;
        }

        public void Clear()
        {
            foreach (var x in _uniqueBuffs.Values) x.Clear();
            {
                var temp = _uniqueBuffs.ToDictionary(kv => kv.Key, kv => kv.Value);
                temp.Clear();
                _uniqueBuffs = temp;
            }
            foreach (var x in _subBuffs.Values) x.Clear();
            {
                var temp = _subBuffs.ToDictionary(kv => kv.Key, kv => kv.Value);
                temp.Clear();
                _subBuffs = temp;
            }
        }

        public bool Contains(SubBuffType type)
        {
            if (_uniqueBuffs.ContainsKey(type) && _uniqueBuffs[type].Count > 0) return true;
            if (_subBuffs.ContainsKey(type) && _subBuffs[type].Count > 0) return true;
            return false;
        }

        public void Update()
        {
            foreach (var x in _uniqueBuffs.Values) x.Update();
            foreach (var x in _subBuffs.Values) x.Update();
        }

        public void PruneUpdate()
        {
            if (HasEmptyTypeList())
            {
                CleanUpEmptyTypeLists();
            }
        }
    }
    
    public readonly struct AddBuffResult
    {
        public Buff Buff { get; }
        public bool CreatedTypeList { get; }
        public bool CreatedUniqueList { get; }
        public SubBuffTypeList TypeList { get; }
        public SubBuffList UniqueList { get; }

        public AddBuffResult(
            Buff buff,
            bool createdTypeList = false,
            bool createdUniqueList = false,
            SubBuffTypeList typeList = null,
            SubBuffList uniqueList = null)
        {
            Buff = buff;
            CreatedTypeList = createdTypeList;
            CreatedUniqueList = createdUniqueList;
            TypeList = typeList;
            UniqueList = uniqueList;
        }
    }

    public readonly struct AddTypeSubBuffResult
    {
        public SubBuff CreatedSubBuff { get; }
        public bool CreatedTypeList { get; }
        public SubBuffTypeList TypeList { get; }

        public AddTypeSubBuffResult(
            SubBuff createdSubBuff,
            bool createdTypeList = false,
            SubBuffTypeList typeList = null)
        {
            CreatedSubBuff = createdSubBuff;
            CreatedTypeList = createdTypeList;
            TypeList = typeList;
        }
    }
}