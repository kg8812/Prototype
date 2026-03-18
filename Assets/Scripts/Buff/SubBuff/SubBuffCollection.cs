using System.Collections.Generic;

namespace Apis
{
    // 현재 책임 : 서브버프 리스트와 업데이트/스택 감소/옵저버까지 함께 관리하는 베이스 클래스
    // 목표 책임 : 서브버프 컬렉션의 공통 구조(리스트 + 기본 흐름 + 옵저버)만 제공하는 베이스
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
            //내부 변수 초기화 : 생성자 책임
            _user = user;
            this.buff = buff;

            // 스택 전략 판단 : 여기 책임 아님
            stackDecrease = new SingleStackDecrease(this, buff.BuffDuration);

            // 서브버프 데이터 가져오기 : 여기 책임 아님
            var subBuff = SubBuffResources.Get(buff);

            // 도트 데미지 전략 판단 : 여기 책임 아님
            if (subBuff is Debuff_DotDmg)
                buffUpdate = new DotDmgUpdate(list, user);
            else
                buffUpdate = new BuffNoUpdate();
            
            // 옵저버 등록 : Subject 생성자 책임
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