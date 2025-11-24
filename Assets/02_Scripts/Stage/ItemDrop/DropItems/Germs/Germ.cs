using System;
using System.Collections;
using Apis.Managers;
using UnityEngine;

namespace Apis
{
    public class Germ: DropItem
    {
        public int germId;
        [SerializeField] private float germForce = 3f;
        [SerializeField] private float radius = 120f;
        [SerializeField] private Collider2D col;
        [SerializeField] private float followPower = 2f;
        
        public int Amount { get; set; }
        
        private const float germJoopDelay = 1f;
        
        private readonly WaitForSeconds _germJoopDelayWait = new WaitForSeconds(germJoopDelay);
        private AudioSource _audioSource;
        private Player _player;
        private bool _canJoop;

        protected void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public override void InvokeInteraction()
        {
            isInteractable = false;
            _audioSource.Play();
            ReturnObject(GameManager.Scene.CurSceneData);
        }

        public override void Dropping()
        {
            Init();
            isInteractable = true;
            _player = GameManager.instance.Player;
            float dAngle = UnityEngine.Random.Range(-radius / 2, radius / 2);
            rigid.velocity = (Quaternion.Euler(0f, 0f, dAngle) * Vector2.up) * germForce;
            _canJoop = false;
            if (!ReferenceEquals(_player, null))
            {
                StartCoroutine(JoopDelay());
            }
            else
            {
                InvokeInteraction();
            }
        }

        private void FixedUpdate()
        {
            Vector2 followVec = _player.Position - transform.position;
            Vector2 curVec = rigid.velocity;
            Vector2 toVec =
                Vector2.Lerp(curVec.normalized, followVec.normalized, followPower * Time.fixedDeltaTime).normalized *
                curVec.magnitude;

            rigid.velocity = toVec;
        }
        
        private IEnumerator JoopDelay()
        {
            yield return _germJoopDelayWait;
            _canJoop = true;
            col.enabled = true;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_canJoop && isInteractable && other.gameObject.CompareTag("Player"))
            {
                _canJoop = false;
                rigid.velocity = Vector2.zero;
                InvokeInteraction();
            }
        }
        
        protected override void ReturnObject(SceneData _)
        {
            GameManager.Item.Germ.Return(this);
        }
    }
}