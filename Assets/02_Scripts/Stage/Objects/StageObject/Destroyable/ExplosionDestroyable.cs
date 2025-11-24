using System;
using System.Collections;
using Apis;
using UnityEngine;
using Default;
using EventData;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Apis
{
    public enum ExplosionDestroyableType
    {
        Stun, Frozen, KnockBack
    }
    public class ExplosionDestroyable : DestroyableObject
    {
        public Collider2D activeCollider;
        public LayerMask layerMask;
        public bool isFlipXByPlayer;
        public ExplosionDestroyableType type;
        public float ccDuration;
        public float explosionDuration;
        [ShowIf("type",ExplosionDestroyableType.KnockBack)] public KnockBackData KnockBackData;

        private SpriteRenderer sp;

        protected override void Awake()
        {
            base.Awake();
            activeCollider.enabled = false;
            sp = GetComponent<SpriteRenderer>();
        }

        protected override void DestroyObj(EventParameters parameters)
        {
            base.DestroyObj(parameters);
            if (isFlipXByPlayer)
            {
                activeCollider.transform.localScale =
                    new Vector3(GameManager.instance.PlayerTrans.position.x > transform.position.x ? -1 : 1, 1, 1);
            }

            activeCollider.enabled = true;
            sp.enabled = false;
            StartCoroutine(TurnOffCollider());
        }

        private IEnumerator TurnOffCollider()
        {
            yield return new WaitForSeconds(explosionDuration);
            activeCollider.enabled = false;
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Utils.CheckLayer(layerMask, other.gameObject))
            {
                if (other.gameObject.TryGetComponent<Actor>(out var value))
                {
                    switch (type)
                    {
                        case ExplosionDestroyableType.Frozen:
                            value.SubBuffManager.AddCC(GameManager.instance.Player,SubBuffType.Debuff_Frozen,ccDuration);
                            break;
                        case ExplosionDestroyableType.Stun:
                            value.StartStun(GameManager.instance.Player, ccDuration);
                            break;
                        case ExplosionDestroyableType.KnockBack:
                            if (value is IMovable mover)
                            {
                                Debug.Log("KnockBack");
                                mover.KnockBack(transform.position,KnockBackData,null,null);
                            }
                            break;
                    }
                    
                }
            }
        }
    }
}