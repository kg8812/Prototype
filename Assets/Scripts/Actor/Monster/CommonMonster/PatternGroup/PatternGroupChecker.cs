using System;
using System.Collections.Generic;
using UnityEngine;

namespace NewMonster
{
    [Serializable]
    public struct FixedRayCast
    {
        public Vector2 startPoint;
        public Vector2 directionVec;
    }

    [Serializable]
    public struct PatternGroupTrigger
    {
        public int triggerId;
        public float minDist;
        public float maxDist;
        public List<FixedRayCast> fixedRayCast;
    }

    public class PatternGroupChecker : MonoBehaviour
    {
        public LayerMask _layerMask;

        [SerializeField] private List<PatternGroupTrigger> _patternGroupTriggers;
        private Actor _actor;


        private void Awake()
        {
            _actor = GetComponent<Actor>();
        }


        // 현재 가능한(거리 판별, 고정캐스트) 패턴 그룹 id들 반환
        public List<int> GetCheckedPatternGroups(float distance)
        {
            List<int> availablePatternGroups = new();

            var origin = _actor.Position;

            foreach (var pG in _patternGroupTriggers)
            {
                // 거리 판별
                if (pG.minDist != 0 || pG.maxDist != 0)
                    if (pG.minDist <= distance && distance <= pG.maxDist)
                    {
                        availablePatternGroups.Add(pG.triggerId);
                        continue;
                    }

                // 고정 캐스트 판별

                foreach (var rayCast in pG.fixedRayCast)
                {
                    var xCheckedF = new Vector2(rayCast.directionVec.x * (int)_actor.Direction, rayCast.directionVec.y);
                    var startPos = new Vector2(origin.x + rayCast.startPoint.x, origin.y + rayCast.startPoint.y);
                    Debug.DrawRay(startPos, xCheckedF, Color.yellow, 0.1f);
                    var hit = Physics2D.Raycast(startPos, xCheckedF, xCheckedF.magnitude, _layerMask);
                    if (hit)
                    {
                        availablePatternGroups.Add(pG.triggerId);
                        break;
                    }
                }
            }

            return availablePatternGroups;
        }


        [ContextMenu("Show Fixed Raycast")]
        private void ShowFixedRay()
        {
            _actor = GetComponent<Actor>();
            var origin = _actor.Position;
            foreach (var pG in _patternGroupTriggers)
            foreach (var rayCast in pG.fixedRayCast)
            {
                var xCheckedF = new Vector2(rayCast.directionVec.x * (int)_actor.Direction, rayCast.directionVec.y);
                var startPos = new Vector2(origin.x + rayCast.startPoint.x, origin.y + rayCast.startPoint.y);
                Debug.DrawRay(startPos, xCheckedF, Color.yellow, 3f);
            }
        }
    }
}