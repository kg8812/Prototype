using UnityEngine;

namespace Apis
{
    public class GoseguRock : BossAttackCollider
    {
        [HideInInspector] public Rigidbody2D rb;

        protected override void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        private void Start()
        {
            AddEvent(EventType.OnTriggerEnter, info =>
            {
                Collider2D col = info?.collideData.collider;
                if (col != null && (col.CompareTag("Player") || col.gameObject.layer == LayerMask.NameToLayer("Map")))
                {
                    Destroy();
                    GameObject obj = GameManager.Factory.Get(FactoryManager.FactoryType.Normal,"GroundHitImpact", transform.position);
                    obj.GetComponent<ParticleSystem>().Play();
                    GameManager.Factory.Return(obj, 1);
                }
            });
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            transform.rotation = Quaternion.identity;
        }

    }
}