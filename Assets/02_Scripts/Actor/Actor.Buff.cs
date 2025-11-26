using System;
using Apis;

public partial class Actor : IBuffUser
{
    private SubBuffManager _subBuffManager; //버프 관리자
    public SubBuffManager SubBuffManager => _subBuffManager ??= new(this);

    
    public void AddSubBuff(IEventUser user, Buff buff, SubBuff subBuff) // 버프 추가 함수 (효과로)
    {
        if (IsDead) return;

        ((IBuffUser)this).AddSubBuff(user, buff, subBuff);
    }

    
    public void AddSubBuff(IEventUser user, SubBuffType type) // 버프 타입으로 추가
    {
        if (IsDead) return;
        
        ((IBuffUser)this).AddSubBuff(user, type);
    }

    /// <summary>
    /// 기절 시작 함수
    /// </summary>
    /// <param name="actor">기절을 거는 Actor</param>
    /// <param name="duration">기절 지속시간</param>
    public virtual void StartStun(IEventUser actor, float duration)
    {
    }

    public virtual void EndStun()
    {
    }
    
    /// <summary>
    /// 액터에서 입력된 효과가 부여한 특정 버프를 제거합니다.
    /// </summary>
    /// <param name="buff">효과</param>
    /// <param name="subBuff">제거할 버프</param>
    public void RemoveSubBuff(Buff buff, SubBuff subBuff)
    {
        ((IBuffUser)this).RemoveSubBuff(buff, subBuff);
    }

    /// <summary>
    /// 액터에서 입력된 효과가 부여한 버프들을 제거합니다.
    /// </summary>
    /// <param name="buff">효과</param>
    public void RemoveSubBuff(Buff buff)
    {
        ((IBuffUser)this).RemoveSubBuff(buff);
    }

    /// <summary>
    /// 액터에서 효과를 제거합니다.
    /// </summary>
    /// <param name="buff">제거할 효과</param>
    public void RemoveBuff(Buff buff)
    {
        ((IBuffUser)this).RemoveBuff(buff);
    }

    /// <summary>
    /// 특정 버프타입을 전부 제거합니다.
    /// </summary>
    /// <param name="type">버프 타입</param>
    public void RemoveType(SubBuffType type)
    {
        ((IBuffUser)this).RemoveType(type);
    }

    /// <summary>
    /// 특정 버프타입을 입력된 개수만큼 제거합니다.
    /// </summary>
    /// <param name="type">버프 타입</param>
    /// <param name="stack">제거할 개수</param>
    public void RemoveType(SubBuffType type, int stack)
    {
        ((IBuffUser)this).RemoveType(type, stack);
    }

    /// <summary>
    /// 모든 버프를 제거합니다.
    /// </summary>
    public void RemoveAllBuff()
    {
        ((IBuffUser)this).RemoveAllBuff();
    }

    /// <summary>
    /// 특정 버프타입의 보유 여부를 반환합니다.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool Contains(SubBuffType type)
    {
        return ((IBuffUser)this).Contains(type);
    }

    /// <summary>
    /// 특정 버프타입의 개수를 반환합니다.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int SubBuffCount(SubBuffType type)
    {
        return ((IBuffUser)this).SubBuffCount(type);
    }

    /// <summary>
    /// 특정 버프타입에 면역을 부여합니다.
    /// </summary>
    /// <param name="type"></param>
    public Guid AddSubBuffTypeImmune(SubBuffType type)
    {
        return ((IBuffUser)this).AddSubBuffTypeImmune(type);
    }

    /// <summary>
    /// 특정 버프타입에 면역을 제거합니다.
    /// </summary>
    /// <param name="type">타입</param>
    /// <param name="guid">면역 부여할 때 반환된 guid</param>
    public void RemoveSubBuffTypeImmune(SubBuffType type,Guid guid)
    {
        ((IBuffUser)this).RemoveSubBuffTypeImmune(type, guid);
    }
}