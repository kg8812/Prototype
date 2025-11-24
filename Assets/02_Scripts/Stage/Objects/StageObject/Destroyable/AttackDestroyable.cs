using Apis;
using EventData;
using UnityEngine;

namespace Apis.Destroyable
{
    public class AttackDestroyable: DestroyableObject, IAttackable
    {
        [SerializeField] private GameObject child;
        [SerializeField] private float dmg;
        public float Atk => dmg;

        public bool destroyWhenAttack;
        
        protected override void Awake()
        {
            base.Awake();
            if (child == null)
                child = transform.GetChild(0).gameObject;
            if (child.TryGetComponent<AttackObject>(out var atkObj))
            {
                atkObj.Init(this, new AtkBase(this));
            }
        }

        protected override void DestroyObj(EventParameters parameters)
        {
            base.DestroyObj(parameters);
            gameObject.SetActive(false);
        }

        public void AttackOn()
        {
            
        }

        public void AttackOff()
        {
            
        }

        public EventParameters Attack(EventParameters eventParameters)
        {
            if (eventParameters?.target == null || eventParameters.target.IsInvincible)
            {
                return null;
            }
            eventParameters.atkData.dmg = eventParameters.atkData.atkStrategy.Calculate(eventParameters.target);
            
            eventParameters.hitData.isCritApplied = false;

            // if (eventParameters.knockBackData.knockBackForce > 0)
            // {
            //     eventParameters.atkData.isHitReaction = true;
            // }

            
            // attack destroyable은 attacker가 없기 때문에 atkobj relative로 계산.
            if (eventParameters.knockBackData.directionType == KnockBackData.DirectionType.AttackerRelative)
            {
                eventParameters.knockBackData.directionType = KnockBackData.DirectionType.AktObjRelative;
            }
            if (eventParameters.groggyKnockBackData.directionType == KnockBackData.DirectionType.AttackerRelative)
            {
                eventParameters.groggyKnockBackData.directionType = KnockBackData.DirectionType.AktObjRelative;
            }
            

            eventParameters.hitData.dmg = eventParameters.atkData.dmg;

            eventParameters.hitData.dmgReceived = eventParameters.target.OnHit(eventParameters);
            if(destroyWhenAttack) DestroyObj(eventParameters);
            return eventParameters;
        }
    }
}