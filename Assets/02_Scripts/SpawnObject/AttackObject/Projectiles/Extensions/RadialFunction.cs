using System;
using Apis;
using Apis;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis
{
    public class RadialFunction : ProjectileExtension
    {
        [TabGroup("방사설정")] [LabelText("방사 크기")]
        public Vector2 size;

        [TabGroup("방사설정")] [LabelText("방사 지속시간")]
        public float duration2;

        [TabGroup("방사설정")] [LabelText("방사 데미지")]
        public float dmg2;

        [TabGroup("방사설정")] [LabelText("방사체 이름")]
        public string radialName;

        private void Awake()
        {
            projectile.AddEvent(EventType.OnDestroy, Destroy);
        }

        void Destroy(EventParameters _)
        {
            if (projectile == null) return;

            int dir = projectile.DirectionScale;

            AttackObject l = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.AttackObject, radialName,
                transform.position + Vector3.right * size.x / 2 * dir);
            l.transform.localScale = size;
            l.Init(projectile._attacker, new AtkBase(projectile._attacker, dmg2),
                duration2);
        }
    }
}