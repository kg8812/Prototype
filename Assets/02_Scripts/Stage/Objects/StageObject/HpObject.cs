using System;
using Apis;
using UnityEngine;

namespace Apis
{
    public class HpObject : MonoBehaviour, IOnHit
    {
        [SerializeField] private float maxHp;

        private float _curHp;

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

        private void Awake()
        {
            _curHp = maxHp;
        }

        public float OnHit(EventParameters parameters)
        {
            CurHp -= parameters.atkData.dmg;
            return parameters.atkData.dmg;
        }

        public float MaxHp => maxHp;

        public float CurHp
        {
            get => _curHp;
            set
            {
                if (value <= 0)
                {
                    _curHp = 0;
                    DestroyObj();
                }
                else
                {
                    _curHp = value;
                }
            }
        }

        public float CritHit { get; }
        public bool IsDead { get; }
        public bool HitImmune { get; set; }
        public bool IsAffectedByCC => false;
        public bool IsInvincible { get; }

        public Guid AddInvincibility()
        {
            return Guid.Empty;
        }

        public void RemoveInvincibility(Guid guid)
        {
        }

        public Guid AddHitImmunity()
        {
            return Guid.Empty;
        }

        public void RemoveHitImmunity(Guid guid)
        {
        }

        public int Exp => 0;


        protected virtual void DestroyObj()
        {
        }
    }
}