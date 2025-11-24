using System.Collections;
using Sirenix.Utilities;
using UnityEngine;

#if UNITY_EDITOR
#endif
namespace Apis
{
    public class WeaponSprite : MonoBehaviour
    {
        SpriteRenderer _spriteRenderer;

        private Weapon _weapon;

        Weapon weapon
        {
            get
            {
                if (_weapon == null)
                {
                    _weapon = transform.root.GetComponent<Weapon>();
                }

                return _weapon;
            }
        }

        SpriteRenderer spriteRenderer
        {
            get
            {
                if (_spriteRenderer == null)
                {
                    _spriteRenderer = GetComponent<SpriteRenderer>();
                }

                return _spriteRenderer;
            }
        }

        private MeshRenderer _meshRenderer;

        MeshRenderer MeshRenderer
        {
            get
            {
                if (_meshRenderer == null)
                {
                    _meshRenderer = GetComponent<MeshRenderer>();
                }

                return _meshRenderer;
            }
        }

        public Renderer Renderer
        {
            get
            {
                return renderType switch
                {
                    RendererType.Sprite => spriteRenderer,
                    RendererType.Mesh => MeshRenderer,
                    _ => null
                };
            }
        }
        float CurrentTime ;

        private int _Condition ;

        public int Condition
        {
            get => _Condition;
            set
            {
                if (!weapon.IsFollow)
                {
                    Renderer.material.SetInt("_Condition", value);
                }

                _Condition = value;
            }
        }

        public AnimationCurve appareCurve;
        public float appareTime = 1;
        public AnimationCurve disappareCurve;
        public float disappareTime = 1;

        public enum RendererType
        {
            Sprite,Mesh
        }

        public RendererType renderType = RendererType.Sprite;

        public void ActiveRenderer(bool On)
        {
            switch (renderType)
            {
                case RendererType.Sprite:
                    spriteRenderer.enabled = On;
                    break;
                case RendererType.Mesh:
                    MeshRenderer.enabled = On;
                    break;
            }
        }

        private void Awake()
        {
            Condition = 1;
        }
       
        public void Appear()
        {
            if (!gameObject.activeInHierarchy || weapon.IsFollow) return;
            
            StartCoroutine(AppearCoroutine());
            
            Condition = 0;

        }

        public void Disappear()
        {
            if (!gameObject.activeInHierarchy || weapon.IsFollow) return;

            StartCoroutine(DisappearCoroutine());
            Condition = 2;

        }
        IEnumerator AppearCoroutine()
        {
            if (Renderer == null || Condition == 0) yield break;

            CurrentTime = 0;
            float value = 0;

            while (value < 1)
            {
                CurrentTime += Time.deltaTime;
                value = appareCurve.Evaluate(CurrentTime / appareTime);
                var value1 = value;
                Renderer?.material?.SetFloat("_ApparePow",value1);
                
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator DisappearCoroutine()
        {
            if (Renderer == null || Condition == 2) yield break;

            CurrentTime = 0;
            float value = 0;

            while (value < 1)
            {
                CurrentTime += Time.deltaTime;
                value = disappareCurve.Evaluate(CurrentTime / disappareTime);
                var value1 = value;
                
                Renderer?.material?.SetFloat("_DisapparePow",value1);

                yield return new WaitForEndOfFrame();
            }
            
            weapon.BoneFollower?.ForEach(x => x.enabled = true);
        }
    }
}