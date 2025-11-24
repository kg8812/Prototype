using System;
using Apis;
using GameStateSpace;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public class ItemBox : MonoBehaviour, IOnInteract
    {
        public Animator _animator;
        StateMachine<ItemBox> states;

        public Func<bool> InteractCheckEvent { get; set; }

        class CloseState : IState<ItemBox>
        {
            public void FixedUpdate()
            {
            }

            public void OnEnter(ItemBox t)
            {
                t._animator.SetBool("locked", false);
                t._animator.SetBool("opened", false);
            }

            public void OnExit()
            {
            }

            public void Update()
            {
            }
        }

        class OpenState : IState<ItemBox>
        {
            public void FixedUpdate()
            {
            }

            public void OnEnter(ItemBox t)
            {
                if (!t.opened)
                {
                    t.opened = true;
                    t._dropper.Drop();
                    t.whenDropped.Invoke();
                }
                t.col.enabled = false;
                t._animator.SetBool("locked", false);
                t._animator.SetBool("opened", true);
            }

            public void OnExit()
            {
            }

            public void Update()
            {
            }
        }

        class LockedState : IState<ItemBox>
        {
            public void FixedUpdate()
            {
            }

            public void OnEnter(ItemBox t)
            {
                t._animator.SetBool("locked", true);
                t._animator.SetBool("opened", false);
            }

            public void OnExit()
            {
            }

            public void Update()
            {
            }
        }

        public void ChangeState(IState<ItemBox> state)
        {
            states.SetState(state);
        }

        [HideInInspector] public ItemDropper _dropper;
        [HideInInspector] public Collider2D col;
        [HideInInspector] public UnityEvent whenDropped;
        IState<ItemBox> openState;
        IState<ItemBox> closeState;
        IState<ItemBox> lockedState;

        [HideInInspector] public bool opened = false;

        bool Check()
        {
            return states.CurrentState is CloseState;
        }
        private void Awake()
        {
            _dropper = GetComponent<ItemDropper>();
            col = GetComponent<Collider2D>();
            openState = new OpenState();
            closeState = new CloseState();
            lockedState = new LockedState();
            whenDropped = new UnityEvent();
            states = new StateMachine<ItemBox>(this, closeState);
            GameManager.instance.GameStateChangedTo.AddListener(LockToggle);
            Init();
            InteractCheckEvent += Check;
        }

        public void Init()
        {
            opened = false;
            col.enabled = true;
            states.SetState(closeState);
        }

        private void Update()
        {
            states.Update();
        }

        private void FixedUpdate()
        {
            states.FixedUpdate();
        }

        public void OnInteract() // 상호작용 인터페이스 
        {
            // 랜덤 픽업 아이템 생성
            if (states.CurrentState is LockedState)
            {
                // 뭔가 잠겨있어서 안열리는 모션
            }
            else if (states.CurrentState is CloseState)
            {
                states.SetState(openState);
            }
        }

        private void LockToggle(GameStateType gameStateType)
        {
            if (!opened)
            {
                if (gameStateType == GameStateType.BattleState)
                {
                    states.SetState(lockedState);
                }

                if (gameStateType == GameStateType.NonBattleState)
                {
                    states.SetState(closeState);
                }
            }
        }
    }
}