using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Apis
{
    public class FireProjectileTrap : Trap, IAttackable
    {
        public string projectileName;
        public ProjectileInfo projectileInfo;
        public float atk;
        public Vector2 fireForce;

        public Transform buttonTrans;
        public float buttonActiveDelay = 1f;
        public Vector2 pressedLocalMove = new(0, -0.1f);

        public float buttonPressOnMoveSec = 0.1f;
        public float buttonPressOffMoveSec = 0.4f;


        private Vector2 _originPos, _pressedPos;

        public Vector3 TopPivot
        {
            get => transform.position;
            set => transform.position = value;
        }

        private void Awake()
        {
            _originPos = buttonTrans.position;
            _pressedPos = _originPos + pressedLocalMove;
        }

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public float Atk => atk;

        public void AttackOn()
        {
        }

        public void AttackOff()
        {
        }

        public EventParameters Attack(EventParameters eventParameters)
        {
            if (eventParameters?.target == null || eventParameters.target.IsInvincible) return null;
            eventParameters.atkData.dmg = eventParameters.atkData.atkStrategy.Calculate(eventParameters.target);

            eventParameters.hitData.isCritApplied = false;


            // if (eventParameters.knockBackData.knockBackForce > 0)
            // {
            //     eventParameters.atkData.isHitReaction = true;
            // }

            eventParameters.hitData.dmg = eventParameters.atkData.dmg;

            eventParameters.hitData.dmgReceived = eventParameters.target.OnHit(eventParameters);
            return eventParameters;
        }

        protected override void Active()
        {
            buttonTrans.DOMove(_pressedPos, buttonPressOnMoveSec);
            var p = GameManager.Factory.Get<Projectile>(FactoryManager.FactoryType.AttackObject, projectileName,
                transform.position);
            if (p != null)
            {
                p.Init(this, new AtkBase(this));
                if (projectileInfo != null) p.Init(projectileInfo);
            }

            p.firstVelocity = fireForce;
            p.Fire();
        }

        public override void Deactive()
        {
            if (!Activated) return;
            StartCoroutine(ButtonReset());
        }

        private IEnumerator ButtonReset()
        {
            yield return new WaitForSeconds(buttonActiveDelay);
            buttonTrans.DOMove(_originPos, buttonPressOffMoveSec);
            Activated = false;
        }
    }
}