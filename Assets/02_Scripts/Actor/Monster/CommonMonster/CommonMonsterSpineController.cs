using Apis.CommonMonster2;
using UnityEngine;

namespace Apis.CommonMonster2
{
    public class CommonMonsterSpineController: SkeletonAttach
    {
        private PatternGroupController pGC;
        private CommonMonster2 _cM;
        protected override void Awake()
        {
            base.Awake();
            _cM = transform.parent.transform.GetComponent<CommonMonster2>();
            pGC = _cM.transform.GetComponent<PatternGroupController>();
            
        }
        public void PatternStarted()
        {
            pGC.PatternStarted();
        }

        public void PatternEnded()
        {
            pGC.PatternEnded();
        }

        public void Attack()
        {
            pGC.Attack();
        }
        
        public void Movement()
        {
            pGC.Movement();
        }

        public void IdleAnimStarted()
        {
            // _cM.IdleAnimationStated();
        }

        public void DeadAnimEnded()
        {
            Debug.Log("dead anim ended");
            _cM.ShaderOnDeath(_cM.ReturnToPool);
        }

        public void SpawnVfx(string address)
        {
            _cM.EffectSpawner.Spawn(address, Vector2.zero,true);
        }

        public void RemoveVfx(string address)
        {
            _cM.EffectSpawner.Remove(address);
        }
        
        private void OnBecameVisible()
        {
        }

        private void OnBecameInvisible()
        {
        }
    }
}