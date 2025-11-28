using UnityEngine;

namespace Apis
{
    /**
     * 개별적으로 공격 계수 설정이 가능한 어택 오브젝트.
     * 또한, onceObject의 성격을 지니지만 기준은 ColliderOn/Off이다.
     */
    public class IndividualCoefAttackCollider : AttackObject
    {
        [SerializeField] private Actor actor;
        public float atkCoef;


        protected override void Awake()
        {
            base.Awake();
            if (actor == null)
                actor = transform.parent.parent.parent.GetComponent<Actor>();
            Init(actor, new AtkBase(actor, atkCoef));
        }
    }
}