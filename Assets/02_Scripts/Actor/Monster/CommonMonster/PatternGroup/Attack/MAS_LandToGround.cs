using DG.Tweening;
using UnityEngine;

namespace Apis.CommonMonster2
{
    [CreateAssetMenu(fileName = "New LandToGround", menuName = "Scriptable/Monster/Attack/LandToGround")]
    [System.Serializable]
    public class MAS_LandToGround: MonsterAction
    {
        private Vector2 startPos, originPos;

        private Tweener _tweener;

        public float landDuration = 0.3f;
        private float realLandDuration;
        public override void Action(CommonMonster2 monster)
        {
            realLandDuration = Default.Utils.CalculateDurationWithAtkSpeed(monster, landDuration);
            base.Action(monster);
            startPos = monster.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.down, 5, LayerMasks.Map | LayerMasks.Platform);
            Debug.DrawRay(startPos, new Vector2(0,-hit.distance), Color.yellow, 1f);
            if (hit.collider != null)
            {
                _tweener = monster.Rb.DOMove(new Vector2(0, -hit.distance), realLandDuration);
            }
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