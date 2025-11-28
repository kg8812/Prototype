using System.Collections;
using GameStateSpace;
using UnityEngine;

namespace Apis.CommonMonster2
{
    public partial class CommonMonster2
    {
        private const float MinRecognitionTime = 2f;
        private float _dist;
        private Coroutine _exitRecog;

        private bool _isActivated;

        private bool _isBlockByMap; // map = ground + wall

        public bool IsInRecognitionCircle { get; set; }

        public bool IsActivated
        {
            get => _isActivated;
            set
            {
                if (_isActivated == value) return;
                _isActivated = value;
                if (value)
                    OnActivated();
                else
                    OnDisActivated();
            }
        }


        public virtual void OnActivated()
        {
        }

        public virtual void OnDisActivated()
        {
        }

        public bool IsInVisible { get; set; }

        /// <summary>
        ///     인식 상태 체크 함수
        ///     - Idle, Patrol, Move 에서만 호출.
        ///     - 나머지 상태에선 인식 여부가 바뀌지 않음.
        ///     - 최적화를 위하여 playerDist를 반환함.
        /// </summary>
        public float CheckRecognition()
        {
            if (IsRecognized || IsInRecognitionCircle || IsInVisible) IsActivated = true;

            _dist = -1;
            if (GameManager.instance.playerDied ||
                GameManager.instance.CurGameStateType == GameStateType.InteractionState)
            {
                IsRecognized = false;
            }
            else if (IsRecognized || IsInRecognitionCircle)
            {
                _dist = ShotRayToPlayer();
                _isBlockByMap = _dist < 0;
                IsRecognized = !_isBlockByMap;
                if (!IsRecognized && _exitRecog != null)
                {
                    StopCoroutine(_exitRecog);
                    _exitRecog = null;
                }
            }

            if (IsRecognized && !IsInRecognitionCircle)
                if (_exitRecog == null)
                    _exitRecog = StartCoroutine(MinRecognitionTimer());

            if (!IsRecognized && !IsInVisible && !IsInRecognitionCircle) IsActivated = false;

            return _dist;
        }


        private IEnumerator MinRecognitionTimer()
        {
            yield return new WaitForSeconds(MinRecognitionTime);
            IsRecognized = false;
        }
    }
}