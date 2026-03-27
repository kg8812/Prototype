using System.Collections.Generic;
using System.Linq;

namespace Apis
{
    // 현재 책임 : 특정 Buff 기준으로 서브버프 리스트를 관리하고 스택/제거 규칙까지 처리
    // 목표 책임 : 특정 Buff에 속한 서브버프들을 단순히 추가/제거/보관하는 컬렉션
    
    public class SubBuffList : SubBuffCollection, ISubject<SubBuffList>
    {
        SubBuffLifeCycleHandler _subBuffLifeCycleHandler;
        protected readonly List<IObserver<SubBuffList>> observers2 = new();

        public SubBuffList(Buff buff, IBuffUser actor) : base(buff, actor)
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
            if (list != null && list.Contains(subBuff)) // 리스트 검색 및 판단 : 컨테이너 책임
            {
                var temp = list.ToList(); // 리스트 조회 : 컨테이너 책임

                temp.Remove(subBuff); // 서브버프 제거 : 컨테이너 책임
                list = temp; 
                NotifyObservers(); // 옵저버 변경 알림 : Subject 책임
                _subBuffLifeCycleHandler.AfterSubBuffRemoved(subBuff);

                return true;
            }

            return false;
        }

        public override SubBuff RemoveSubBuff()
        {
            if (list != null && list.Count > 0) // 리스트 검색 및 판단 : 컨테이너 책임
            {
                var subBuff = list[0]; // 리스트 조회 : 컨테이너 책임

                if (buff.StackDecrease == 0) // 스택 전략 판단 : 이곳 책임 X
                {
                    var temp = list.ToList();
                    temp.RemoveAt(0);
                    list = temp;
                    NotifyObservers();

                    _subBuffLifeCycleHandler.AfterSubBuffRemoved(subBuff);
                }
                else if (buff.StackDecrease == 1) // 스택 전략 판단 : 이곳 책임 X
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

            foreach (var x in temp)
            {
                _subBuffLifeCycleHandler.AfterSubBuffRemoved(x);
            }
        }
    }
}