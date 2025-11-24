using System;
using Apis;
using UnityEngine;

namespace Apis
{
    public class FallingObject : MonoBehaviour, IAttackable, IDirection
    {
        [SerializeField] private GameObject child;
        [SerializeField] private LayerMask target;


        private Rigidbody2D _childRigid;
        private Projectile _childProjectile;
        private Vector2 _childOrigin;

        private bool _activated;


        private void Start()
        {
            if (child == null)
                child = transform.GetChild(0).gameObject;
            _childRigid = child.GetComponent<Rigidbody2D>();
            _childProjectile = child.GetComponent<Projectile>();
            _childOrigin = child.transform.localPosition;
            if (child.TryGetComponent<AttackObject>(out var atkObj))
            {
                atkObj.Init(this, new AtkBase(this));
            }

            Reset();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_activated && target.Contains(other.gameObject.layer))
            {
                Active();
            }
        }

        public void Active()
        {
            _activated = true;
            _childRigid.bodyType = RigidbodyType2D.Dynamic;
            _childProjectile.Fire();
        }

        public void Reset()
        {
            _activated = false;
            _childRigid.gravityScale = 0;
            _childRigid.velocity = Vector2.zero;
            _childRigid.bodyType = RigidbodyType2D.Static;
            child.transform.localPosition = _childOrigin;
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

        public float Atk { get; }

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

            eventParameters.hitData.dmg = eventParameters.atkData.dmg;

            eventParameters.hitData.dmgReceived = eventParameters.target.OnHit(eventParameters);
            return eventParameters;
        }

        public EActorDirection Direction => EActorDirection.Right;
        public void SetDirection(EActorDirection dir)
        {
            
        }

        public int DirectionScale => 1;
    }
}