using System.Collections;
using Apis;
using DG.Tweening;
using UnityEngine;

namespace Apis
{
    public class GashiTrap : Trap, IAttackable
    {
        public Transform gashiTrans;
        public Vector2 activeLocalMove;
        public float onDuration, offDuration;
        public Ease onMoveFunc, offMoveFunc;
        public int dmg;
        public float delay = 1f;
        [Tooltip("onDuration보다 길어야 버그 안남")]public float gashiReturnDelay = 1f;

        private Vector2 _originPos, _activatedPos;
        private Sequence sq;

        // 0 = default, 1 = 나오는 도중, 2 = 들어가는 도중
        private int moveType = 0;


        private void Awake()
        {
            _originPos = gashiTrans.localPosition;
            _activatedPos = _originPos + activeLocalMove;
            if (gashiTrans.TryGetComponent<AttackObject>(out var atkObj))
            {
                atkObj.Init(this, new FixedAmount(dmg));
            }
        }

        protected override void Active()
        {
            if (moveType != 0)
            {
                return;
            }

            moveType = 1;
            if (sq != null && sq.IsPlaying()) return;
            
            sq = DOTween.Sequence()
                .AppendInterval(delay)
                .Append(gashiTrans.DOLocalMove(_activatedPos, onDuration).SetEase(onMoveFunc))
                .OnComplete(() => { moveType = 0; });
        }

        public override void Deactive()
        {
            if (!Activated) return;
            
            StartCoroutine(GashiReset());
        }

        private IEnumerator GashiReset()
        {
            yield return new WaitForSeconds(gashiReturnDelay);
            
            sq?.Kill();

            moveType = 2;
            sq = DOTween.Sequence().Append(gashiTrans.DOLocalMove(_originPos, offDuration).SetEase(offMoveFunc)).OnComplete(
                () => { moveType = 0;});
            Activated = false;
        }

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Vector3 TopPivot
        {
            get => transform.position;
            set => transform.position = value;
        }

        public float Atk => dmg;

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

            eventParameters.hitData.dmg = eventParameters.atkData.dmg;

            eventParameters.hitData.dmgReceived = eventParameters.target.OnHit(eventParameters);
            return eventParameters;
        }
    }
}