using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public partial class Monster
    {
        
        private const float MonsterHitBlinkTime = 0.5f;
        private readonly WaitForSeconds _monsterHitBlinkTimeWait = new WaitForSeconds(MonsterHitBlinkTime);
        private const float MonsterDeathTime = 1f;
        
        // shader properties
        private static readonly int HitProperty = Shader.PropertyToID("_IsHit");
        private static readonly int HitDistancePowerProperty = Shader.PropertyToID("_HitEffectDistancePow");
        
        private static readonly int DeathProperty = Shader.PropertyToID("_IsDead");
        private static readonly int DeathProcessProperty = Shader.PropertyToID("_DeadEffectProcess");
        

        private bool _isShowShaderHitEff;
        private Coroutine _hitShaderCoroutine;

        private bool _isShowShaderDeathEff;
        private Coroutine _deathShaderCoroutine;
        
        protected virtual void ShaderOnHit()
        {
            if (_isShowShaderHitEff)
            {
                StopCoroutine(_hitShaderCoroutine);
            }
            _hitShaderCoroutine = StartCoroutine(ToggleShaderHit());
        }

        private IEnumerator ToggleShaderHit()
        {
            _isShowShaderHitEff = true;
            // Hit 활성화
            meshRenderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat(HitProperty, 1); // Hit On
            meshRenderer.SetPropertyBlock(propBlock);

            // yield return _monsterHitBlinkTimeWait;
            // Power 증가
            float power = 1f;
            meshRenderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat(HitDistancePowerProperty, power);
            meshRenderer.SetPropertyBlock(propBlock);
            // while (power < 1f)
            // {
            //     power += Time.deltaTime * 0.5f; // Power를 점진적으로 증가
            //     meshRenderer.GetPropertyBlock(propBlock);
            //     propBlock.SetFloat(HitDistancePowerProperty, power);
            //     meshRenderer.SetPropertyBlock(propBlock);
            //
            //     yield return null;
            // }
            //
            // // Power 감소
            while (power > 0.01f)
            {
                power -= Time.deltaTime * 4f; // Power를 점진적으로 감소
                meshRenderer.GetPropertyBlock(propBlock);
                propBlock.SetFloat(HitDistancePowerProperty, power);
                meshRenderer.SetPropertyBlock(propBlock);
            
                yield return null;
            }

            // Hit 비활성화
            meshRenderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat(HitProperty, 0); // Hit Off
            meshRenderer.SetPropertyBlock(propBlock);

            _hitShaderCoroutine = null;
            _isShowShaderHitEff = false;
        }

        public virtual void ShaderOnDeath(UnityAction endAction = null)
        {
            if (_isShowShaderDeathEff)
            {
                StopCoroutine(_deathShaderCoroutine);
            }
            _deathShaderCoroutine = StartCoroutine(ToggleShaderDeath(endAction));
        }

        private IEnumerator ToggleShaderDeath(UnityAction endAction)
        {
            _isShowShaderDeathEff = true;
            
            meshRenderer.GetPropertyBlock(propBlock);
            float power = 0;
            propBlock.SetFloat(DeathProcessProperty, power);
            propBlock.SetFloat(DeathProperty, 1);
            meshRenderer.SetPropertyBlock(propBlock);

            while (power < 1f)
            {
                power +=  Time.deltaTime / MonsterDeathTime;
                meshRenderer.GetPropertyBlock(propBlock);
                propBlock.SetFloat(DeathProcessProperty, power);
                meshRenderer.SetPropertyBlock(propBlock);
            
                yield return null;
            }


            _deathShaderCoroutine = null;
            _isShowShaderDeathEff = false;
            endAction?.Invoke();
            
            // 마지막 프레임 끊길 가능성 존재해서 넣음.
            yield return null;
            
            meshRenderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat(DeathProperty, 0);
            meshRenderer.SetPropertyBlock(propBlock);
        }

        protected virtual void ClearShaderOnDeath()
        {
            meshRenderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat(DeathProcessProperty, 0);
            propBlock.SetFloat(DeathProperty, 0);
            meshRenderer.SetPropertyBlock(propBlock);
        }
    }
}