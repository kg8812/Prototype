using System;
using Default;
using DG.Tweening;
using UnityEngine;

namespace Apis.CommonMonster2
{
    [CreateAssetMenu(fileName = "New LandToGround", menuName = "Scriptable/Monster/Attack/LandToGround")]
    [Serializable]
    public class MAS_LandToGround : MonsterAction
    {
        public float landDuration = 0.3f;

        private Tweener _tweener;
        private float realLandDuration;
        private Vector2 startPos, originPos;

        public override void Action(CommonMonster2 monster)
        {
            realLandDuration = Utils.CalculateDurationWithAtkSpeed(monster, landDuration);
            base.Action(monster);
            startPos = monster.transform.position;
            var hit = Physics2D.Raycast(startPos, Vector2.down, 5, LayerMasks.Map | LayerMasks.Platform);
            Debug.DrawRay(startPos, new Vector2(0, -hit.distance), Color.yellow, 1f);
            if (hit.collider != null) _tweener = monster.Rb.DOMove(new Vector2(0, -hit.distance), realLandDuration);
        }

        public override void Update()
        {
        }

        public override void FixedUpdate()
        {
        }

        public override void OnCancel()
        {
        }
    }
}