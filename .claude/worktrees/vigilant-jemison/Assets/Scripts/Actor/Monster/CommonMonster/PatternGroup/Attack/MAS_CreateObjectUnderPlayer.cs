using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis.CommonMonster2
{
    [CreateAssetMenu(fileName = "New CreateObjectUnderPlayer",
        menuName = "Scriptable/Monster/Attack/CreateObjectUnderPlayer")]
    [Serializable]
    public class MAS_CreateObjectUnderPlayer : MonsterAction
    {
        public bool isUnder;
        [HideIf("isUnder")] public Vector2 abovePlayerOffset;
        public float maxDist;
        public TimeType creationType;
        public AttackObject attackObject;
        public float atkRatio;
        public bool isConsiderDirection;
        public bool isHitReaction;
        public float knockBack;
        public bool storeInProjectileList;
        private int randInd;

        private Vector2 startPos, originPos, dist;

        public override void Action(CommonMonster2 monster)
        {
            base.Action(monster);
            startPos = GameManager.instance.ControllingEntity.Position;
            if (isUnder)
            {
                var hit = Physics2D.Raycast(startPos, Vector2.down, 5, LayerMasks.GroundAndPlatform);
                Debug.DrawRay(startPos, new Vector2(0, -hit.distance), Color.yellow, 1f);
                if (hit.collider != null)
                {
                    originPos = monster.Position;
                    dist = hit.point - originPos;
                    if (dist.sqrMagnitude <= maxDist * maxDist) CreateAttackObject(monster, attackObject, hit.point);
                }
            }
            else
            {
                CreateAttackObject(monster, attackObject, startPos + abovePlayerOffset);
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
        }

        private void CheckCreationType(CommonMonster2 monster, Vector2 pos)
        {
        }

        private void CreateAttackObject(CommonMonster2 monster, AttackObject obj, Vector2 pos)
        {
            var atkObj = GameManager.Factory.Get(FactoryManager.FactoryType.AttackObject, obj.name)
                .GetComponent<AttackObject>();
            atkObj.transform.position = pos;
            if (isConsiderDirection)
                atkObj.transform.localScale = new Vector3(monster.Direction == EActorDirection.Right ? 1 : -1, 1, 1) *
                                              atkObj.transform.localScale.y;
            atkObj.Init(monster, new AtkBase(monster, atkRatio));
            if (atkObj.TryGetComponent<Animator>(out var anim)) anim.SetTrigger("Fire");

            if (storeInProjectileList)
            {
                if (atkObj.TryGetComponent<Rigidbody2D>(out var rigid)) rigid.linearVelocity = Vector2.zero;
                monster.Projectiles.Add(atkObj as Projectile);

                void RemoveProjectile(EventParameters _)
                {
                    _cM.Projectiles.Remove(atkObj as Projectile);
                    atkObj.RemoveEvent(EventType.OnDestroy, RemoveProjectile);
                }

                atkObj.AddEvent(EventType.OnDestroy, RemoveProjectile);
            }
        }
    }
}