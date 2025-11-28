using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis.StageObj
{
    public class Lever : SerializedMonoBehaviour, IOnInteract
    {
        /**
         * 레버는 Triggered Object에게 다음과 같은 신호를 보냅니다.
         * - 0: left로 전환함.
         * - 1: right로 전환함.
         */
        private readonly int AnimBooleanR = Animator.StringToHash("toRight");

        private readonly int AnimBooleanL = Animator.StringToHash("toLeft");

        private readonly WaitForSeconds leverWait = new(1f);
        private bool isInteractable;

        [SerializeField] private bool isMultiLever;

        [SerializeField] private SpriteRenderer spr;

        [ShowIf("isMultiLever")] [SerializeField]
        private Lever multiLever;

        [SerializeField] private bool isInitialRight;
        [SerializeField] private bool canInteractOneSide;

        [ShowIf("canInteractOneSide")] [SerializeField]
        private bool interactInRight;

        private bool _curRight;
        private Animator _animator;
        private Collider2D _interactionCol;

        public List<TriggeredObj> triggeredObj;

        public bool IsInteractable
        {
            get => isInteractable;
            set
            {
                _interactionCol.enabled = value;
                isInteractable = value;
            }
        }


        protected virtual void Awake()
        {
            _interactionCol = GetComponent<Collider2D>();
            _animator = GetComponent<Animator>();
            IsInteractable = true;
            InteractCheckEvent += () => IsInteractable;
        }

        protected virtual void Start()
        {
            MoveToForce(isInitialRight);
        }

        public Func<bool> InteractCheckEvent { get; set; }

        public virtual void OnInteract()
        {
            if (IsInteractable) MoveLever();
        }

        public void MoveTo(bool isRight)
        {
            IsInteractable = false;
            _curRight = isRight;
            _animator.SetTrigger(_curRight ? AnimBooleanR : AnimBooleanL);
            if (!canInteractOneSide || _curRight == interactInRight) StartCoroutine(LeverMoveEnded());
        }

        public void MoveToForce(bool isRight)
        {
            _curRight = isRight;
            IsInteractable = !canInteractOneSide || _curRight == interactInRight;
        }

        protected void MoveLever()
        {
            MoveTo(!_curRight);

            foreach (var x in triggeredObj) x.ChangeTrigger(_curRight ? 1 : 0);

            if (isMultiLever) multiLever.MoveTo(_curRight);
        }

        /**
         * for animation
         */
        public IEnumerator LeverMoveEnded()
        {
            yield return leverWait;
            IsInteractable = true;
        }
    }
}