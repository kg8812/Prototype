using DG.Tweening;
using UnityEngine;

namespace Apis
{
    public class GoseguFlask : BossAttackCollider
    {
        [HideInInspector] public Rigidbody2D rb;
        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody2D>();
            collid = GetComponent<Collider2D>();
        }

        Collider2D collid;
        private void Start()
        {
            AddEvent(EventType.OnTriggerEnter, info =>
            {
                Collider2D col = info?.collideData.collider;
                if (col != null && (col.CompareTag("Player") || col.gameObject.layer == LayerMask.NameToLayer("Map")))
                {
                    Destroy();
                    GameObject obj = GameManager.Factory.Get(FactoryManager.FactoryType.Normal, "FlaskEffect", transform.position);
                    obj.GetComponent<ParticleSystem>().Play();
                    GameManager.Factory.Return(obj, 1);
                }
            });

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            transform.rotation = Quaternion.identity;
            collid.enabled = false;
        }
        Sequence seq;

        public void CollideOn()
        {
            collid.enabled = true;
        }
        public Sequence Throw(float time, float distance, float height)
        {
            rb.DOKill();
            seq?.Kill();
            rb.AddTorque(-100 * (int)boss.Direction);
            Vector2 pos = new(boss.transform.position.x + (int)boss.Direction * distance, boss.transform.position.y);

            seq = rb.DOJump(pos, height, 1, time).SetUpdate(UpdateType.Fixed);
            collid.enabled = true;
            return seq;
        }

        public Sequence ThrowToTarget(float time, Vector2 target, float height)
        {
            rb.DOKill();
            seq?.Kill();
            rb.AddTorque(-100 * (int)boss.Direction);

            Vector2 lastPos;
            Vector2 vel;
            lastPos = rb.position;
            vel = Vector2.zero;

            seq = rb.DOJump(target, height, 1, time).OnUpdate(() =>
            {
                Vector2 dir = (rb.position - lastPos) / Time.fixedDeltaTime;
                if (dir.magnitude > 1) vel = dir;

                lastPos = rb.position;
            }).SetUpdate(UpdateType.Fixed).OnComplete(() =>
            {
                rb.velocity = vel;
            });
            collid.enabled = true;
            return seq;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            rb.DOKill();
            seq?.Kill();
            collid.enabled = false;
        }
    }
}