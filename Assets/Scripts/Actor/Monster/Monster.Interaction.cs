using System;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public partial class Monster : IOnInteract
    {
        //몬스터 상호작용 관련 코드입니다.
        // Static으로 되어있으며 한번 적용시키면 모든 몬스터에 적용됩니다.

        // 몬스터가 활성화될 때 실행되는 이벤트입니다. AddActivateEvent 함수를 통해 이벤트를 추가하셔야 됩니다.
        private static UnityEvent<Monster> _onActivate;

        // 이벤트를 추가할 때, 호출되는 이벤트입니다. 자세한 내용은 AddActivateEvent 함수를 확인하십시오.
        private static UnityEvent<UnityAction<Monster>> OnAdd;

        // 이벤트를 제거할 때, 호출되는 이벤트입니다.
        private static UnityEvent<UnityAction<Monster>> OnRemove;

        private static UnityEvent<Monster> _interactEvent;

        // 상호작용 on/off여부를 관리하는 컨트롤러입니다.
        private static ImmunityController _interactionController;

        private static float radius;
        private bool _isIntractable;

        private CircleCollider2D interactCollider;
        private static UnityEvent<Monster> OnActivate => _onActivate ??= new UnityEvent<Monster>();

        private static ImmunityController InteractionController => _interactionController ??= new ImmunityController();

        /// <summary>
        ///     상호작용시 호출되는 이벤트입니다. 상호작용시 실행할 함수들을 추가해주세요.
        /// </summary>
        public static UnityEvent<Monster> InteractEvent => _interactEvent ??= new UnityEvent<Monster>();

        // 상호작용 가능 여부입니다
        public Func<bool> InteractCheckEvent { get; set; }

        public void OnInteract() // 상호작용 함수
        {
            InteractEvent.Invoke(this);
        }

        private bool Check()
        {
            if (CheckInteractable != null) return CheckInteractable(this) && _isIntractable;

            return _isIntractable;
        }

        // 상호작용 가능 여부를 추가하는 이벤트입니다.
        public static event Func<Monster, bool> CheckInteractable;

        /// <summary>
        ///     몬스터 활성화 이벤트에 Action을 추가하는 함수입니다.
        ///     몬스터가 활성화 될 때 이벤트가 실행되고, 이미 활성화 된 몬스터는 Action을 바로 실행합니다.
        /// </summary>
        /// <param name="action">이벤트에 추가할 함수</param>
        public static void AddActivateEvent(UnityAction<Monster> action)
        {
            OnActivate.RemoveListener(action);
            OnActivate.AddListener(action);
            OnAdd ??= new UnityEvent<UnityAction<Monster>>();
            OnAdd.Invoke(action);
        }

        /// <summary>
        ///     활성화 이벤트에서 Action을 제거하는 함수입니다.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="removeAction"></param>
        public static void RemoveActivateEvent(UnityAction<Monster> action, UnityAction<Monster> removeAction)
        {
            OnActivate.RemoveListener(action);
            OnRemove ??= new UnityEvent<UnityAction<Monster>>();
            OnRemove.Invoke(removeAction);
        }

        /// <summary>
        ///     몬스터의 상호작용을 켭니다.
        /// </summary>
        /// <param name="_radius">상호작용 반경</param>
        public static void MakeInteractable(float _radius)
        {
            radius = _radius;

            AddActivateEvent(AddInteraction);
        }

        private static void AddInteraction(Monster monster)
        {
            if ((object)monster.interactCollider == null)
            {
                monster.interactCollider =
                    GameManager.Factory.Get<CircleCollider2D>(FactoryManager.FactoryType.Normal, "InteractCollider");
                monster.interactCollider.transform.SetParent(monster.transform);
                monster.interactCollider.radius = radius;
                monster.interactCollider.transform.position = monster.Position;
            }

            monster._isIntractable = true;
        }

        /// <summary>
        ///     몬스터의 상호작용을 끕니다.
        /// </summary>
        public static void RemoveInteractable()
        {
            RemoveActivateEvent(AddInteraction, RemoveInteraction);
        }

        private static void RemoveInteraction(Monster monster)
        {
            monster._isIntractable = false;
        }
    }
}