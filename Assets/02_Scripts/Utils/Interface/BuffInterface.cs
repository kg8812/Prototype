using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Apis
{
    public interface IImmunity
    {
        public ImmunityController ImmunityController { get; }
    }

    public interface IBuffUser : IImmunity,IMonoBehaviour
    {
        public SubBuffManager SubBuffManager { get; }

        public void AddSubBuff(IEventUser user, Buff buff, SubBuff subBuff) // 버프 추가 함수 (효과로)
        {
            SubBuffManager.AddBuff(user,buff, subBuff);
        }
        public void AddSubBuff(IEventUser user, SubBuffType type) // 버프 타입으로 추가
        {
            SubBuffManager.AddSubBuff(type, user);
        }
        /// <summary>
    /// 액터에서 입력된 효과가 부여한 특정 버프를 제거합니다.
    /// </summary>
    /// <param name="buff">효과</param>
    /// <param name="subBuff">제거할 버프</param>
    public void RemoveSubBuff(Buff buff, SubBuff subBuff)
    {
        SubBuffManager.RemoveSubBuff(buff, subBuff);
    }

    /// <summary>
    /// 액터에서 입력된 효과가 부여한 버프들을 제거합니다.
    /// </summary>
    /// <param name="buff">효과</param>
    public void RemoveSubBuff(Buff buff)
    {
        SubBuffManager.RemoveSubBuff(buff);
    }

    /// <summary>
    /// 액터에서 효과를 제거합니다.
    /// </summary>
    /// <param name="buff">제거할 효과</param>
    public void RemoveBuff(Buff buff)
    {
        SubBuffManager.RemoveBuff(buff);
    }

    /// <summary>
    /// 특정 버프타입을 전부 제거합니다.
    /// </summary>
    /// <param name="type">버프 타입</param>
    public void RemoveType(SubBuffType type)
    {
        SubBuffManager.RemoveType(type);
    }

    /// <summary>
    /// 특정 버프타입을 입력된 개수만큼 제거합니다.
    /// </summary>
    /// <param name="type">버프 타입</param>
    /// <param name="stack">제거할 개수</param>
    public void RemoveType(SubBuffType type, int stack)
    {
        SubBuffManager.RemoveType(type, stack);
    }

    /// <summary>
    /// 모든 버프를 제거합니다.
    /// </summary>
    public void RemoveAllBuff()
    {
        SubBuffManager.Collector.Clear();
    }

    /// <summary>
    /// 특정 버프타입의 보유 여부를 반환합니다.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool Contains(SubBuffType type)
    {
        return SubBuffManager.Contains(type);
    }

    /// <summary>
    /// 특정 버프타입의 개수를 반환합니다.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int SubBuffCount(SubBuffType type)
    {
        return SubBuffManager.Count(type);
    }

    /// <summary>
    /// 특정 버프타입에 면역을 부여합니다.
    /// </summary>
    /// <param name="type"></param>
    public Guid AddSubBuffTypeImmune(SubBuffType type)
    {
        return SubBuffManager.AddImmune(type);
    }

    /// <summary>
    /// 특정 버프타입에 면역을 제거합니다.
    /// </summary>
    /// <param name="type">타입</param>
    /// <param name="guid">면역 부여할 때 반환된 guid</param>
    public void RemoveSubBuffTypeImmune(SubBuffType type,Guid guid)
    {
        SubBuffManager.RemoveImmune(type,guid);
    }
    }
}