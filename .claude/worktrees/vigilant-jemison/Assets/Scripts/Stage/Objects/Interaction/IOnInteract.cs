using System;

public interface IOnInteract
{
    // 상호작용이 필요한 클래스에 추가합니다.

    public Func<bool> InteractCheckEvent { get; set; }
    void OnInteract(); // 상호작용 함수

    public bool IsInteractable()
    {
        return InteractCheckEvent == null || InteractCheckEvent();
    }
}