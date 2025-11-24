using System.Collections;
using Apis.Managers;
using Default;
using UnityEngine;

namespace Apis
{
    public class Gold_Drop : DropItem
    {
        public const int Gold1Id = 8001;
        public const int Gold3Id = 8002;
        public const int Gold30Id = 8003;
        public const int Gold300Id = 8004;
        
        public const int LobbyGold1Id = 8011;
        public const int LobbyGold5Id = 8012;
        public const int LobbyGold20Id = 8013;
        public const int LobbyGold50Id = 8014;
        
        private const float radius = 120f;
        private const float goldMinForce = 2f;
        private const float goldMaxForce = 4f;
        private const float lobbyGoldForce = 6f;
        
        private const float goldJoopDelay = 0.3f;
        private const float goldJoopSqrDist = 4;

        private readonly WaitForSeconds goldJoopDelayWait = new WaitForSeconds(goldJoopDelay);

        private AudioSource audioSource;
        private SpriteRenderer sp;
        
        public GoldType goldType;
        public int goldId;
        public int value;
        
        
        [SerializeField] private float lobbyGoldFollowPower = 10f;
        [SerializeField] private Collider2D col;
        
        private Player player;
        private Transform myTrans;
        private ParticleSystem ps;

        private bool lobbyGoldFired = false;




        private void Awake()
        {
            myTrans = transform;
            ps = Utils.GetComponentInParentAndChild<ParticleSystem>(gameObject, false);
            audioSource = GetComponent<AudioSource>();
            sp = GetComponent<SpriteRenderer>();
        }

        // 플레이어가 직접 oninteract 하는 것이 아닌 collider 안 붙이고 직접 함수 실행
        public override void InvokeInteraction()
        {
            // TODO: effect
            isInteractable = false;
            audioSource.Play();
            if (goldType == GoldType.Gold)
            {
                sp.enabled = false;
                GameManager.instance.Soul += value;
                ps.Play();
            }
            else
            {
                GameManager.instance.LobbySoul += value;
                ps.Stop();
            }
            StartCoroutine(DestroyGold());
        }

        private IEnumerator DestroyGold()
        {
            yield return new WaitForSeconds(2);
            GameManager.Item.Gold.Return(this);
        }
    
        // FixedUpdate에서 할지 코루틴으로 할지 나중에 결정
        protected override void Droped(){
            // OnInteract();
        }

        public override void Dropping()
        {
            if(goldType == GoldType.LobbyGold)
                col.enabled = false;
            Init();
            isInteractable = true;
            

            player = GameManager.instance.Player;
            float dAngle = Random.Range(-radius / 2, radius / 2);

            GameManager.SectorMag.CurEnteredSectorChanged += ForceGetBySectorChanged;
            
            if (goldType == GoldType.Gold)
            {
                sp.enabled = true;
                float dForce = Random.Range(goldMinForce, goldMaxForce);
                rigid.velocity = (Quaternion.Euler(0f, 0f, dAngle) * Vector2.up) * dForce;
                if (!ReferenceEquals(player, null))
                {
                    StartCoroutine(CheckPlayerPos());
                }
            }
            else
            {
                ps.Play();
                rigid.velocity = (Quaternion.Euler(0f, 0f, dAngle) * Vector2.up) * lobbyGoldForce;
                if (!ReferenceEquals(player, null))
                {
                    StartCoroutine(LobbyGoldDelay());
                }
                else
                {
                    InvokeInteraction();
                }
            }
        }

        private void ForceGetBySectorChanged(int _)
        {
            GameManager.SectorMag.CurEnteredSectorChanged -= ForceGetBySectorChanged;
            InvokeInteraction();
        }

        private void FixedUpdate()
        {
            if (goldType == GoldType.LobbyGold && lobbyGoldFired)
            {
                Vector2 followVec = player.Position - myTrans.position;
                Vector2 curVec = rigid.velocity;
                Vector2 toVec =
                    Vector2.Lerp(curVec.normalized, followVec.normalized, lobbyGoldFollowPower * Time.fixedDeltaTime).normalized *
                    curVec.magnitude;

                rigid.velocity = toVec;
            }
        }

        private IEnumerator CheckPlayerPos()
        {
            yield return goldJoopDelayWait;
            while (true)
            {
                if ((player.Position - myTrans.position).sqrMagnitude < goldJoopSqrDist)
                {
                    InvokeInteraction();
                    break;
                }
                yield return null;
            }
        }

        private IEnumerator LobbyGoldDelay()
        {
            yield return goldJoopDelayWait;
            lobbyGoldFired = true;
            col.enabled = true;
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isInteractable && goldType == GoldType.LobbyGold && other.gameObject.CompareTag("Player"))
            {
                lobbyGoldFired = false;
                rigid.velocity = Vector2.zero;
                InvokeInteraction();
            }
        }


        protected override void ReturnObject(SceneData _)
        {
            GameManager.SectorMag.CurEnteredSectorChanged -= ForceGetBySectorChanged;
            GameManager.Item.Gold.Return(this);
        }
    }
}
