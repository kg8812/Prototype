using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis
{
    public class SoundWaveExtension : ProjectileExtension
    {
        [Title("음파 설정")] [LabelText("초기 크기")] public Vector2 size1;
        [LabelText("최종 크기")] public Vector2 size2;
        [LabelText("총 데미지 감소율(%)")] public float dmgReduce;

        [LabelText("감소 적용시간")] [Tooltip("이 시간에 거쳐서 수치가 서서히 감소함")]
        public float reduceTime;

        [LabelText("감소 Ease")] public Ease ease;

        private Tween tween1;
        private Tween tween2;

        private void Awake()
        {
            projectile.OnFire += Fire;
        }

        public void Fire()
        {
            transform.localScale = size1;
            transform.DOScale(size2, reduceTime);
            var finalDmg = projectile.DmgRatio * dmgReduce / 100;

            tween1 = DOTween.To(() => projectile.DmgRatio, x => projectile.DmgRatio = x, finalDmg, reduceTime)
                .SetEase(ease);
        }

        public override void Destroy()
        {
            base.Destroy();
            tween1?.Kill();
            tween2?.Kill();
        }
    }
}