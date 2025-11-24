using System;
using System.Collections.Generic;
using Apis.DataType;
using EventData;
using UnityEngine;

namespace Apis
{ public class SubBuffManager
    {
        //버프 관리 클래스

        public Actor User { get; }

        public SubBuffCollector Collector { get; private set; }
        BonusStat stat;
        
        public BonusStat Stats
        {
            get
            {
                stat ??= new();
                stat.Reset();

                Traverse(x => { stat += x.Stat;} );
                return stat;
            }
        }
        public SubBuffManager(Actor user)
        {
            User = user;
            Collector = new SubBuffCollector(this);
        }

        public void Traverse(Action<SubBuff> action)
        {
            foreach (var x in Collector.uniqueBuffs.Values)
            {
                foreach (var y in x.buffs.Keys)
                {
                    foreach (var z in x[y])
                    {
                        action(z);
                    }
                }
            }
            foreach (var x in Collector.subBuffs.Values)
            {
                foreach (var y in x.List)
                {
                    action(y);
                }
            }
        }
        /// <summary>
        /// 특정 효과로 액터에 버프를 추가합니다.
        /// 버프의 수치를(데미지,지속시간 등) 공용이 아닌 다른 수치로 사용하고 싶을 때 사용합니다.
        /// </summary>
        /// <param name="user"> 버프를 부여한 Actor (몬스터일 시, 웬만하면 플레이어)</param>
        /// <param name="buff"> 이 버프를 부여하는 효과</param>
        /// <param name="subBuff"> 추가할 버프</param>
        public void AddBuff(IEventUser user,Buff buff, SubBuff subBuff)
        {
            if (subBuff == null || buff == null) return;
            if (IsImmune(subBuff.Type)) return;

            BuffEventData buffData = new()
            {
                activatedSubBuff = subBuff,
                takenSubBuff = subBuff,
            };
            EventParameters parameters = new(user, User)
            {
                buffData = buffData
            };

            if (user != null)
            {
                subBuff.target = user.gameObject;
            }

            Collector.AddBuff(buff, subBuff);
        
            user?.EventManager.ExecuteEvent(EventType.OnSubBuffApply, parameters);

            parameters = new(User, user?.gameObject.GetComponent<IOnHit>())
            {
                buffData = buffData
            };
            User.ExecuteEvent(EventType.OnSubBuffTaken, parameters);
        }

        /// <summary>
        /// 액터에 타입으로 버프를 추가합니다.
        /// 수치는 SubBuffOptionTable에 입력된 공용 수치를 사용합니다.
        /// 공용 수치는 Type마다 공유합니다.
        /// </summary>
        /// <param name="user">버프를 부여한 Actor (몬스터일 시, 웬만하면 플레이어)</param>
        /// <param name="type">버프 타입</param>
        public void AddSubBuff(SubBuffType type,Actor target)
        {
            if (IsImmune(type)) return;
        
            SubBuff sub = Collector.AddSubBuff(type,target);

            BuffEventData buffData = new()
            {
                activatedSubBuff = sub,
                takenSubBuff = sub,
            };
            if (sub == null || target == null) return;

            EventParameters parameters = new(target, User)
            {
                buffData = buffData
            };
            
            target.EventManager.ExecuteEvent(EventType.OnSubBuffApply, parameters);

            parameters = new(User, target)
            {
                buffData = buffData
            };
            User.ExecuteEvent(EventType.OnSubBuffTaken, parameters);
        }
        
        /// <summary>
        /// CC를 추가하는 함수 (빙결 혹은 기절만 적용됩니다)
        /// 유닛에 기절을 부여할 땐 이 함수말고 StartStun 함수를 호출해주세요.
        /// </summary>
        /// <param name="actor"> 이 CC를 부여한 액터 </param>
        /// <param name="type"> CC타입 , Debuff_Frozen : 빙결, Debuff_Stun : 스턴 </param>
        /// <param name="duration"> 지속시간 </param>
        public void AddCC(IEventUser actor, SubBuffType type, float duration)
        {
            switch (type)
            {
                case SubBuffType.Debuff_Stun:
                case SubBuffType.Debuff_Frozen:
                    BuffDataType data = new(type)
                    {
                        buffDuration = duration, buffCategory = 1,
                        buffMaxStack = 1, buffDispellType = 1, showIcon = false
                    };
                    Buff buff = new(data, actor);

                    buff.AddSubBuff(User, null);
                
                    break;
                default:
                    Debug.LogError("CC기만 가능합니다 (빙결 혹은 스턴)");
                    return;
            }
        }
        public void RemoveSubBuff(Buff buff, SubBuff subBuff)
        {
            Collector.RemoveSubBuff(buff, subBuff);
        }
        public void RemoveSubBuff(Buff buff)
        {
            Collector.RemoveSubBuff(buff);
        }
        
        public void RemoveBuff(Buff buff)
        {
            Collector.RemoveBuff(buff);
        }      
        public void RemoveType(SubBuffType type)
        {
            Collector.RemoveType(type);
        }

        public void RemoveType(SubBuffType type, int stack)
        {
            Collector.RemoveType(type,stack);
        }
        public bool Contains(SubBuffType type)
        {
            return Collector.Contains(type);
        }

        public int Count(SubBuffType type)
        {
            return Collector.Count(type);
        }

        public void Update()
        {
            Collector.Update();
        }

        bool IsImmune(SubBuffType type)
        {
            return User.ImmunityController.IsImmune(type.ToString());
        }
        public Guid AddImmune(SubBuffType type)
        {
            string t = type.ToString();
            if (!User.ImmunityController.Contains(t))
            {
                User.ImmunityController.MakeNewType(t);
            }

            return User.ImmunityController.AddCount(t);
        }

        public void RemoveImmune(SubBuffType type, Guid guid)
        {
            User.ImmunityController.MinusCount(type.ToString(),guid);
        }
    }
}