using System.Collections.Generic;
using UnityEngine.Events;

namespace Apis
{
    public interface IEventUser : IMonoBehaviour
    {
        // 이벤트 사용자
        public IEventManager EventManager { get; }
        List<IEventChild> EventChildren { get; }
        
    }

    public interface IEventManager
    {
        // 이벤트 매니저
        public void AddEvent(EventType type, UnityAction<EventParameters> action); // 이벤트 추가
        public void RemoveEvent(EventType type, UnityAction<EventParameters> action); // 이벤트 제거
        public void ExecuteEvent(EventType type, EventParameters parameters); // 이벤트 실행
        public UnityEvent<EventParameters> GetEvent(EventType type); // 이벤트 가져오기
    }
    public interface IEventChild
    {
        public void Init(IEventUser user); // 이벤트 사용자 할당
    }
}