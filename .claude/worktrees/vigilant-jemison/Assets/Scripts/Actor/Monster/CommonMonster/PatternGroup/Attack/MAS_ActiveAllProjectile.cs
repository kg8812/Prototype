using System;
using System.Collections;
using Default;
using UnityEngine;

namespace Apis.CommonMonster2
{
    [CreateAssetMenu(fileName = "New MAS_ActiveAllProjectile",
        menuName = "Scriptable/Monster/Attack/MAS_ActiveAllProjectile")]
    [Serializable]
    public class MAS_ActiveAllProjectile : MonsterAction
    {
        public TimeType timeType;
        public float delayTime;

        private Coroutine myCrt;
        private float realDelayTime;

        public override void Action(CommonMonster2 monster)
        {
            // Debug.Log("all projectile action");
            realDelayTime = Utils.CalculateDurationWithAtkSpeed(monster, delayTime);
            base.Action(monster);
            switch (timeType)
            {
                case TimeType.Immediately:
                    foreach (var proj in _cM.Projectiles) proj.Fire();
                    break;
                case TimeType.Periodically:
                    myCrt = GameManager.instance.StartCoroutineWrapper(FireProjectilesPeriodically());
                    break;
            }
        }

        public override void Update()
        {
        }

        public override void FixedUpdate()
        {
        }

        public override void OnCancel()
        {
            if (myCrt != null)
            {
                GameManager.instance.StopCoroutineWrapper(myCrt);
                foreach (var proj in _cM.Projectiles)
                {
                    // TODO: 투사체 해당 함수 만들어 주기
                    /*
                    if (!proj.fired)
                    {
                        proj.DeActivate();
                    }
                    */
                }
            }
        }

        private IEnumerator FireProjectilesPeriodically()
        {
            foreach (var proj in _cM.Projectiles)
            {
                proj.Fire();
                yield return new WaitForSeconds(realDelayTime);
            }
        }
    }
}