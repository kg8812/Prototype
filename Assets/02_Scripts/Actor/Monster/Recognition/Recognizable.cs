using System.Collections;
using UnityEngine;

namespace Apis
{
    public class Recognizable : MonoBehaviour
    {
        [SerializeField] private IMonoBehaviour _iPosition;

        public bool IsInRecognitionCircle { get; set; }
        public bool IsInVisible { get; set; }
        private bool _isActivated;

        public bool IsActivated
        {
            get => _isActivated;
            set
            {
                // 함수 안에서 _isRecognized = value 해줌.
                if (!_isActivated && value)
                {
                    _isActivated = true;
                    OnActivated();
                }

                if (!value && _isActivated)
                {
                    _isActivated = false;
                    OnDisActivated();
                }
            }
        }

        private bool _isRecognized;

        public bool IsRecognized
        {
            get => _isRecognized;
            set
            {
                if (!_isRecognized && value)
                {
                    _isRecognized = true;
                    OnRecognized();
                }

                if (!value && _isRecognized)
                {
                    _isRecognized = false;
                    OnDisRecognized();
                }
            }
        }

        private bool _isBlockByMap; // map = ground + wall
        private Coroutine _exitRecog;

        protected virtual void OnActivated()
        {
            
        }

        private void OnDisActivated()
        {
            _isActivated = false;
        }


        private void OnRecognized()
        {
            _isActivated = true;
        }

        private void OnDisRecognized()
        {
            _isActivated = false;
        }


        public bool CheckRecognition(out float? dist)
        {
            if (IsRecognized || IsInRecognitionCircle || IsInVisible)
            {
                IsActivated = true;
            }

            dist = -1;
            if (IsRecognized || IsInRecognitionCircle)
            {
                dist = ShotRayToPlayer();
                _isBlockByMap = dist < 0;
                IsRecognized = !_isBlockByMap;
                if (!IsRecognized && _exitRecog != null)
                {
                    StopCoroutine(_exitRecog);
                    _exitRecog = null;
                }
            }

            if (IsRecognized && !IsInRecognitionCircle)
            {
                if (_exitRecog == null)
                {
                    StartCoroutine(MinRecognitionTimer());
                }
            }

            if (!IsRecognized && !IsInVisible && !IsInRecognitionCircle)
            {
                IsActivated = false;
            }

            return IsRecognized;
        }

        private IEnumerator MinRecognitionTimer()
        {
            yield return new WaitForSeconds(2f);
            IsRecognized = false;
        }

        public float ShotRayToPlayer()
        {
            Vector3 myRayPoint = _iPosition.Position;
            Vector2 dir = GameManager.instance.ControllingEntity.Position - myRayPoint;
            RaycastHit2D hit = Physics2D.Raycast(myRayPoint, dir, Mathf.Infinity,
                LayerMasks.Player | LayerMasks.GroundWall);
#if UNITY_EDITOR
            Debug.DrawRay(myRayPoint, dir, Color.yellow);
#endif
            if (hit && hit.collider.gameObject.CompareTag("Player"))
            {
                return hit.distance;
            }

            return -1;
        }
    }
}