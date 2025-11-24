using System;
using Apis;
using UnityEngine;
using UnityEngine.Serialization;

namespace Apis
{
    [RequireComponent(typeof(Actor))]
    public class ActorEffect: MonoBehaviour
    {
        private Actor _actor;

        [SerializeField] private EventType eventType;

        [SerializeField] private float size = 1f;
        
        public enum EffectRotationType
        {
            Fixed,          // 그냥 고정
            ActorRotate,    // actor 기준 방향 설정
            PlayerRotate    // player 기준 방향 설정
        }

        public EffectRotationType rotationType;
        public string effectName;

        private void Awake()
        {
            _actor = GetComponent<Actor>();
        }

        private void Start()
        {
            ChangeEventType(eventType);
        }

        public void ChangeEventType(EventType toType)
        {
            _actor.EventManager.RemoveEvent(eventType, EffectOn);
            eventType = toType;
            _actor.EventManager.AddEvent(eventType, EffectOn);
        }

        private void EffectOn(EventParameters _)
        {
            if (effectName != null)
            {
                var effect = _actor.EffectSpawner.Spawn(effectName, _actor.Position, false);
                bool isRight = true;
                switch (rotationType)
                {
                    case EffectRotationType.ActorRotate:
                        isRight = _actor.transform.localScale.x > 0;
                        break;
                    case EffectRotationType.PlayerRotate:
                        isRight = _actor.transform.position.x > GameManager.instance.ControllingEntity.Position.x;
                        break;
                }

                effect.transform.localScale = new Vector3(isRight ? size : -size, size, size);
            }
        }
    }
}