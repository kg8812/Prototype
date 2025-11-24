using Apis;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis
{
    public class WhipEffect : Projectile // Tile sprite로 발사되는것처럼 보이는 형식
    {
        protected override bool IsCheckMaxDist => false;

        [LabelText("사정거리")] public float distance;
        [LabelText("y크기")] public float ySize;
        [LabelText("발사시간")] public float fireTime;
        [LabelText("발사Ease")] public Ease ease;

        public override void Fire(bool rotateWithPlayerX = true)
        {
            Collider.enabled = true;
            SpriteRenderer _renderer = GetComponent<SpriteRenderer>();

            transform.localScale = new Vector3(-(_direction != null ?(int)_direction.Direction : 1), 1, 1);
            _renderer.size = new Vector2(0, ySize);
            
            Tween tweener = DOTween.To(() => _renderer.size, x => _renderer.size = x,
                new Vector2(distance, _renderer.size.y),
                fireTime).SetUpdate(UpdateType.Fixed).SetEase(ease).SetAutoKill(true);
            GameObject col = transform.GetChild(0).gameObject;
            tweener.onUpdate += () => { col.transform.localPosition = new Vector2(-_renderer.size.x, 0); };
        }
    }
}