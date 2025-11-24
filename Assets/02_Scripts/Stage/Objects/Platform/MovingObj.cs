using System;
using System.Collections.Generic;
using System.Linq;
using Apis;
using Apis.BehaviourTreeTool;
using Default;
using Sirenix.Utilities;
using UnityEngine;

namespace Apis
{
    public class MovingObj: MonoBehaviour ,IPoolObject
    {
        [SerializeField] public LayerMask layers;
        /* 플레이어 root motion 사용 시 이동 보정용 */
        public Vector2 Velocity { get; private set; }
        private Vector3 previousPosition;

        private Collider2D myCollider;
        
        struct Datas
        {
            public Transform parent;
            public bool isDontDestroy;

            public Datas(Transform parent, bool isDontDestroy)
            {
                this.parent = parent;
                this.isDontDestroy = isDontDestroy;
            }
        }
        
        private Dictionary<GameObject,Datas> moves = new();
        private HashSet<GameObject> contacts = new();
        
        private void Awake()
        {
            moves = new();
            myCollider = GetComponent<Collider2D>();
            contacts ??= new();
            previousPosition = transform.position;
            
        }

        private bool IsOnTop(Transform other)
        {
            Bounds platformBounds = myCollider.bounds;
            Vector2 playerFoot = other.position;

            float platformTop = platformBounds.max.y;

            // x축 오버랩 확인
            float playerX = playerFoot.x;
            float platformMinX = platformBounds.min.x;
            float platformMaxX = platformBounds.max.x;

            bool isAbove = Mathf.Abs(playerFoot.y - platformTop) < 0.1f;
            bool isWithinX = playerX > platformMinX && playerX < platformMaxX;

            return isAbove && isWithinX;
        }

        private void FixedUpdate()
        {
            List<GameObject> toRemove = new List<GameObject>();
            List<GameObject> keys = moves.Keys.ToList(); // 반복 중 수정을 피하기 위해 키 목록 복사

            foreach (var key in keys)
            {
                if (!moves.ContainsKey(key)) continue; // 다른 곳에서 제거되었을 수 있음

                Transform objTransform = key.transform;
                Datas data = moves[key];

                bool shouldBeParented = IsOnTop(objTransform); // 플랫폼 위에 있는지 여부로 판단 (더 정교한 조건 필요할 수 있음)

                if (shouldBeParented)
                {
                    // 위에 있고, 아직 자식이 아니라면 자식으로 설정
                    if (objTransform.parent != transform)
                    {
                        objTransform.SetParent(transform);
                        
                        if(objTransform.TryGetComponent(out IMovable movable))
                            movable.MoveComponent.OnMovingObj = this;
                    }
                }
                else
                {
                    // 위에 있지 않고, 자식으로 설정되어 있다면 원래 부모로 되돌림
                    if (objTransform.parent == transform)
                    {
                        objTransform.SetParent(data.parent);

                        if(objTransform.TryGetComponent(out IMovable movable))
                            movable.MoveComponent.OnMovingObj = null;
                        // 원래 DontDestroyOnLoad 상태 복원
                        if (data.isDontDestroy)
                        {
                            DontDestroyOnLoad(key);
                        }
                    }

                    // 접촉도 완전히 끝났다면 관리 목록에서 제거 고려
                    if (!contacts.Contains(key))
                    {
                        toRemove.Add(key);
                    }
                }
            }
            toRemove.ForEach(Remove); // 루프 종료 후 제거

            UpdateDelta();
        }

    private void UpdateDelta()
    {
        Velocity = (transform.position - previousPosition) / Time.fixedDeltaTime;
        previousPosition = transform.position;
    }

        public void Remove(GameObject x)
        {
            if (!moves.TryGetValue(x, out var move)) return;
            
            if (x.transform.parent == transform)
            {
                x.transform.SetParent(move.parent);
                if (moves[x].isDontDestroy)
                {
                    DontDestroyOnLoad(x);
                }
            }
            
            moves.Remove(x);
        }

        public void RemoveAll()
        {
            var remover = moves.ToList();
            remover.ForEach(x =>
            {
                Remove(x.Key);
            });
            contacts.Clear();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (((1 << other.gameObject.layer) & layers) != 0)
            {
                contacts.Add(other.gameObject);
                if (!moves.ContainsKey(other.gameObject))
                {
                    bool isDontDestroy = other.gameObject.IsDontDestroy();

                    Datas data = new()
                    {
                        parent = other.transform.parent,
                        isDontDestroy = isDontDestroy
                    };

                    if (IsOnTop(other.transform))
                    {
                        moves.Add(other.gameObject, data);
                    }
                }
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (((1 << other.gameObject.layer) & layers) != 0)
            {
                contacts.Remove(other.gameObject);
            }
        }

        public void OnGet()
        {
            RemoveAll();
        }

        public void OnReturn()
        {
            RemoveAll();
        }
    }
}