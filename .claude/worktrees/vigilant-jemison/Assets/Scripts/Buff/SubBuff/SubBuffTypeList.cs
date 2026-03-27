using System;
using System.Collections.Generic;
using System.Linq;
using Apis.DataType;
using Sirenix.Utilities;
using UnityEngine;

namespace Apis
{
    public class SubSingleStack : IBuffCollectionUpdate
    {
        private readonly SubBuffTypeList subBuffList;

        public SubSingleStack(SubBuffTypeList subBuffList)
        {
            this.subBuffList = subBuffList;
            Duration = subBuffList.Duration;
            CurTime = Duration;
        }

        public float Duration { get; set; }

        public float CurTime { get; set; }

        public void Notify(List<SubBuff> value)
        {
        }

        public void ResetTime()
        {
            CurTime = Duration;
        }

        public void Update()
        {
            if (Duration < 0 || Mathf.Approximately(Duration, 0)) return;

            if (CurTime > 0) CurTime -= Time.deltaTime;

            if (CurTime < 0)
            {
                subBuffList.RemoveSubBuff();
                CurTime = Duration;
            }
        }
    }

    // 현재 책임 : SubBuffType 기준으로 서브버프를 관리하면서 생성/스택/시간/DB/상위 제거까지 처리
    // 목표 책임 : 특정 타입의 서브버프들을 스택 규칙에 따라 보관하는 타입별 컬렉션
    public class SubBuffTypeList : ISubject<List<SubBuff>>
    {
        private readonly IBuffUpdate _buffUpdate;

        private readonly IBuffCollectionUpdate _stackStrategy;
        private readonly SubBuffType _type;
        private readonly IBuffUser actor;

        public readonly Buff dummyBuff;
        private readonly int maxStack;
        public readonly SubBuffOptionDataType option;

        private List<IObserver<List<SubBuff>>> _observers;

        private List<SubBuff> list;
        public CustomQueue<SubBuff> queue = new();

        private SubBuffLifeCycleHandler _subBuffLifeCycleHandler;
        
        public SubBuffTypeList(SubBuffType type, IBuffUser actor)
        {
            // 데이터베이스 조회 및 데이터 가져오기 : 여기 책임 아님
            BuffDatabase.DataLoad.TryGetSubBuffIndex(type, out var index);
            BuffDatabase.DataLoad.TryGetSubBuffOption(index, out option);

            // 내부 필드 수정 : 생성자 책임
            _type = type;
            maxStack = option.maxStack;
            Duration = option.duration;
            this.actor = actor;

            // 스택 전략 판단 : 이곳 책임 아님
            _stackStrategy = new SubSingleStack(this);

            // 타입 및 이름 데이터 추출 : 이곳 책임 아님
            var str = "Apis." + type;
            var tp = Type.GetType(str);

            // 업데이트 전략 판단 : 이곳 책임 아님
            if (tp != null && tp.IsSubclassOf(typeof(Debuff_DotDmg)))
                _buffUpdate = new DotDmgUpdate(list, actor);
            else
                _buffUpdate = new BuffNoUpdate();
            
            // 옵저버 등록 : subject 생성자 책임
            Attach(_stackStrategy);
            Attach(_buffUpdate);

            // 더미 버프 데이터 생성 : 생성자 책임 애매
            BuffDataType data = new(type)
            {
                buffPower = option.amount, buffDuration = Duration, buffDispellType = 1,
                stackDecrease = option.stackDecrease,
                showIcon = option.showIcon
            };

            dummyBuff = new Buff(data, null);
        }

        public float CurTime => _stackStrategy.CurTime;
        public float Duration { get; }

        public List<SubBuff> List => list ??= new List<SubBuff>();

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

        private List<IObserver<List<SubBuff>>> Observers => _observers ??= new List<IObserver<List<SubBuff>>>();

        public void Attach(IObserver<List<SubBuff>> observer)
        {
            if (!Observers.Contains(observer)) Observers.Add(observer);
        }

        public void Detach(IObserver<List<SubBuff>> observer)
        {
            Observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            foreach (var x in Observers) x.Notify(list);
        }

        private void AddSub(SubBuff subBuff)
        {
            var wasMaxStack = false;

            if (maxStack > 0 && Count >= maxStack)
            {
                wasMaxStack = true;
                var temp = queue.Dequeue();
                _subBuffLifeCycleHandler.AfterSubBuffRemoved(subBuff);
                
            }

            queue.Enqueue(subBuff);
            ResetList();
            _subBuffLifeCycleHandler.AfterSubBuffAdded(subBuff);
            if (maxStack > 0 && Count >= maxStack && !wasMaxStack)
            {
                _subBuffLifeCycleHandler.AfterSubBuffMaxStackReached(subBuff);
            }
        }

        public void Add(SubBuff subBuff)
        {
            AddSub(subBuff);
        }

        public SubBuff Add(GameObject target)
        {
            // 지속시간 초기화 : 컨테이너 책임
            _stackStrategy.CurTime = Duration;
            
            var sub = SubBuffResources.Get(dummyBuff);
            if (sub == null) return null;
            sub.User = actor;
            sub.target = target;
            
            // 서브버프 추가 : 컨테이너 책임
            AddSub(sub);

            return sub;
        }

        public SubBuff RemoveSubBuff()
        {
            if (Count <= 0) return null;
            var subBuff = queue.Dequeue();
            switch (option.stackDecrease)
            {
                case 0:
                    ResetList();
                    _subBuffLifeCycleHandler.AfterSubBuffRemoved(subBuff);
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
                if (x.buff == buff)
                {
                    sub = x;
                    break;
                }

            queue.Remove(sub);
            ResetList();
            _subBuffLifeCycleHandler.AfterSubBuffRemoved(sub);
            return sub;
        }

        public void RemoveBuff(Buff buff)
        {
            List<SubBuff> removeList = new();
            queue.ForEach(x =>
            {
                if (x.buff == buff) removeList.Add(x);
            });
            if (removeList.Count == 0) return;

            removeList.ForEach(x =>
            {
                queue.Remove(x);
                _subBuffLifeCycleHandler.AfterSubBuffRemoved(x);
            });
        }

        public void Clear()
        {
            queue.Clear();

            var tempList = list?.ToList();
            ResetList();

            if (tempList != null)
                foreach (var x in tempList)
                {
                    _subBuffLifeCycleHandler.AfterSubBuffRemoved(x);
                }
        }

        private void ResetList()
        {
            // 기존 서브버프 조회 : 저장소 책임
            list = queue.ToList();

            // 내부 옵저버 변경 알림 : 상위 Subject 책임
            NotifyObservers();
        }

        public void Update()
        {
            _stackStrategy.Update();
            _buffUpdate.Update();

            foreach (var x in list) x.Update();
        }
    }
}