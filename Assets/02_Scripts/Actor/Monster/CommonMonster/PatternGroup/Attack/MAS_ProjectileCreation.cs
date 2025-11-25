using Apis;
using UnityEngine;

namespace Apis.CommonMonster2
{
    [CreateAssetMenu(fileName = "New MAS_ProjectileCreation",
        menuName = "Scriptable/Monster/Attack/MAS_ProjectileCreation")]
    [System.Serializable]
    public class MAS_ProjectileCreation : MAS_AttackObjectCreation
    {
        public ProjectileInitialVelocityType initVecType;
        public bool isFireImmediately;
        [HideInInspector] public Projectile _curProj;
        public bool storeInProjectileList;


        protected virtual void SetVelocityMonsterDirection(Projectile createdProj)
        {
            createdProj.firstVelocity =
                new Vector2(
                    createdProj.firstVelocity.x * (_monster.Direction == EActorDirection.Right ? 1 : -1),
                    createdProj.firstVelocity.y);
        }

        protected virtual void SetVelocityToPlayer(Projectile createdProj)
        {
            float initPower = createdProj.firstVelocity.magnitude;
            Debug.Log($"set vel to player{(GameManager.instance.ControllingEntity.Position - createdProj.transform.position).normalized * initPower}");
            createdProj.firstVelocity =
                (GameManager.instance.ControllingEntity.Position - createdProj.transform.position).normalized * initPower;
        }


        protected virtual void SetFirstVelocity(Projectile createdProj)
        {
            switch (initVecType)
            {
                case ProjectileInitialVelocityType.MonsterDirection:
                    SetVelocityMonsterDirection(createdProj);
                    break;

                case ProjectileInitialVelocityType.ToPlayer:
                    SetVelocityToPlayer(createdProj);
                    break;
            }
        }

        protected virtual void SetPosition(Projectile createdProj, AttackObjectData atkData)
        {
            if (atkData.transformName != "")
            {
                // TODO 미리 commonMonster에 position 리스트 만들어두고 id 방식으로 전환
                createdProj.transform.position = Default.Utils.FindChild<Transform>(_monster.gameObject, atkData.transformName, true).position;
            }
            else
            {
                createdProj.transform.position = _monster.Position +
                                                 new Vector3(atkData.isConsiderDirection
                                                     ? (atkData.offset.x *
                                                        (_monster.Direction == EActorDirection.Right ? 1 : -1))
                                                     : atkData.offset.x, atkData.offset.y, atkData.offset.z);
            }
        }

        protected override void CreateAttackObject(CommonMonster2 monster, AttackObjectData atk)
        {
            proj = GameManager.Factory.Get(FactoryManager.FactoryType.AttackObject, atk.obj.gameObject.name);
            _curProj = proj.GetComponent<Projectile>();
            _curProj.Init(_monster, new AtkBase(_monster));
            SetPosition(_curProj, atk);
            SetFirstVelocity(_curProj);
            if (storeInProjectileList)
            {
                if (_curProj.TryGetComponent<Rigidbody2D>(out var rigid))
                {
                    rigid.linearVelocity = Vector2.zero;
                }
                monster.Projectiles.Add(_curProj);
                void RemoveProjectile(EventParameters _)
                {
                    _cM.Projectiles.Remove(_curProj);
                    _curProj.RemoveEvent(EventType.OnDestroy, RemoveProjectile);
                }
                _curProj.AddEvent(EventType.OnDestroy, RemoveProjectile);
            }
            if (isFireImmediately)
            {
                _curProj.Fire();
            }
        }
    }
}