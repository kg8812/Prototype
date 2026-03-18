using System;
using EventData;
using UnityEngine;

namespace Apis
{
    
    //버프를 유니티 컴포넌트 및 외부에 연결역할을 담당하는 클래스
    
    [RequireComponent(typeof(Actor))]
    public class BuffSystem : MonoBehaviour, IBuffUser
    {
        private SubBuffManager _subBuffManager; //버프 관리자
        public SubBuffManager SubBuffManager => _subBuffManager ??= new SubBuffManager(this);

        private Actor _actor;
        private Actor Actor => _actor ??= GetComponent<Actor>();

        private void Awake()
        {
            if (TryGetComponent(out IBarrierUser barrierUser))
            {
                barrierUser.BarrierCalculator.BarrierAddEvent += AddBarrier;
                barrierUser.BarrierCalculator.BarrierMinusEvent += MinusBarrier;
            }
        }

        private void Start()
        {
            Actor.BonusStatEvent += () => SubBuffManager.Stats;
            Actor.AddEvent(EventType.OnDeath , _ =>
            {
                RemoveAllBuff();
            });
        }

        private void Update()
        {
            SubBuffManager.Update();
        }

        float AddBarrier(float value)
        {
            SubBuffManager.Traverse(x =>
            {
                if (x is BarrierBase barrier) value += barrier.Barrier;
            });

            return value;
        }

        float MinusBarrier(float dmg)
        {
            SubBuffManager.Traverse(x =>
            {
                if (dmg <= 0) return;

                if (x is BarrierBase barrier)
                {
                    if (barrier.Barrier > dmg)
                    {
                        barrier.Barrier -= dmg;
                        dmg = 0;
                    }
                    else
                    {
                        dmg -= barrier.Barrier;
                        barrier.Barrier = 0;
                        barrier.onBarrierDestroy.Invoke();
                    }
                }
            });

            return dmg;
        }
        public void AddSubBuff(IBuffUser user, Buff buff, SubBuff subBuff) // 버프 추가 함수 (효과로)
        {
            if (Actor.IsDead) return;

            if (SubBuffManager.AddSubBuff(user, buff, subBuff))
            {
                BuffEventData buffData = new()
                {
                    activatedSubBuff = subBuff,
                    takenSubBuff = subBuff
                };
                
                if (user.gameObject.TryGetComponent(out IEventUser eventUser))
                {

                    EventParameters parameters = new(eventUser, SubBuffManager.User.gameObject.GetComponent<IOnHit>());
                    parameters.Set(buffData);

                    eventUser?.EventManager.ExecuteEvent(EventType.OnSubBuffApply, parameters);
                }

                if (SubBuffManager.User.gameObject.TryGetComponent(out IEventUser eventUser2))
                {
                    EventParameters parameters =
                        new EventParameters(eventUser2,
                            eventUser?.gameObject.GetComponent<IOnHit>());
                    parameters.Set(buffData);
                       
                    eventUser2.EventManager.ExecuteEvent(EventType.OnSubBuffTaken, parameters);
                }
            }
        }


        public void AddSubBuff(IBuffUser user, SubBuffType type) // 버프 타입으로 추가
        {
            if (Actor.IsDead) return;

            var sub = SubBuffManager.AddSubBuff(type,user);
            
            if (sub != null)
            {
                BuffEventData buffData = new()
                {
                    activatedSubBuff = sub,
                    takenSubBuff = sub
                };
                
                if (user.gameObject.TryGetComponent(out IEventUser eventUser))
                {
                    EventParameters parameters = new(eventUser, SubBuffManager.User.gameObject.GetComponent<IOnHit>());
                    parameters.Set(buffData);
                    eventUser?.EventManager.ExecuteEvent(EventType.OnSubBuffApply, parameters);
                }

                if (SubBuffManager.User.gameObject.TryGetComponent(out IEventUser eventUser2))
                {
                    EventParameters parameters =
                        new EventParameters(eventUser2,
                            eventUser?.gameObject.GetComponent<IOnHit>());
                    parameters.Set(buffData);
                    eventUser2.EventManager.ExecuteEvent(EventType.OnSubBuffTaken, parameters);
                }
            }
        }

        /// <summary>
        ///     액터에서 입력된 효과가 부여한 특정 버프를 제거합니다.
        /// </summary>
        /// <param name="buff">효과</param>
        /// <param name="subBuff">제거할 버프</param>
        public void RemoveSubBuff(Buff buff, SubBuff subBuff)
        {
            if (SubBuffManager.RemoveSubBuff(buff, subBuff))
            {
                if (SubBuffManager.User.gameObject.TryGetComponent(out IEventUser eventUser))
                {
                    EventParameters parameters = new(eventUser);
                    parameters.Set(new BuffEventData(){removedSubBuff = subBuff});
                    
                    eventUser.EventManager.ExecuteEvent(EventType.OnSubBuffRemove, parameters);
                }
                
            }
        }

        /// <summary>
        ///     액터에서 입력된 효과가 부여한 버프들을 제거합니다.
        /// </summary>
        /// <param name="buff">효과</param>
        public void RemoveSubBuff(Buff buff)
        {
            var sub = SubBuffManager.RemoveSubBuff(buff);

            if (sub != null)
            {
                if (SubBuffManager.User.gameObject.TryGetComponent(out IEventUser eventUser))
                {
                    EventParameters parameters = new(eventUser);
                    parameters.Get<BuffEventData>().removedSubBuff = sub;
                    eventUser.EventManager.ExecuteEvent(EventType.OnSubBuffRemove, parameters);
                }
            }
        }

        /// <summary>
        ///     액터에서 효과를 제거합니다.
        /// </summary>
        /// <param name="buff">제거할 효과</param>
        public void RemoveBuff(Buff buff)
        {
            SubBuffManager.RemoveBuff(buff);

            if (SubBuffManager.User.gameObject.TryGetComponent(out IEventUser eventUser))
            {
                EventParameters parameters = new(eventUser);
                parameters.Set(new BuffEventData { removedSubBuff = buff?.ActivatedSubBuff });

                eventUser.EventManager.ExecuteEvent(EventType.OnSubBuffRemove, parameters);
            }
           
        }

        /// <summary>
        ///     특정 버프타입을 전부 제거합니다.
        /// </summary>
        /// <param name="type">버프 타입</param>
        public void RemoveType(SubBuffType type)
        {
            SubBuffManager.RemoveType(type);
        }

        /// <summary>
        ///     특정 버프타입을 입력된 개수만큼 제거합니다.
        /// </summary>
        /// <param name="type">버프 타입</param>
        /// <param name="stack">제거할 개수</param>
        public void RemoveType(SubBuffType type, int stack)
        {
            SubBuffManager.RemoveType(type, stack);
        }

        /// <summary>
        ///     모든 버프를 제거합니다.
        /// </summary>
        public void RemoveAllBuff()
        {
            SubBuffManager.Collector.Clear(); 
        }

        /// <summary>
        ///     특정 버프타입의 보유 여부를 반환합니다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool Contains(SubBuffType type)
        {
            return SubBuffManager.Contains(type);
        }

        /// <summary>
        ///     특정 버프타입의 개수를 반환합니다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int SubBuffCount(SubBuffType type)
        {
            return SubBuffManager.Count(type);
        }

        /// <summary>
        ///     특정 버프타입에 면역을 부여합니다.
        /// </summary>
        /// <param name="type"></param>
        public Guid AddSubBuffTypeImmune(SubBuffType type)
        {
            return SubBuffManager.AddImmune(type);

        }

        /// <summary>
        ///     특정 버프타입에 면역을 제거합니다.
        /// </summary>
        /// <param name="type">타입</param>
        /// <param name="guid">면역 부여할 때 반환된 guid</param>
        public void RemoveSubBuffTypeImmune(SubBuffType type, Guid guid)
        {
            SubBuffManager.RemoveImmune(type, guid);
        }

        public Vector3 Position
        {
            get => _actor.Position;
            set => _actor.Position = value;
        }

        public ImmunityController ImmunityController => Actor.ImmunityController;
    }
}