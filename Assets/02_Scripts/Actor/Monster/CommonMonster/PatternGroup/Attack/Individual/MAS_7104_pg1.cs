using Apis;
using UnityEngine;

namespace Apis.CommonMonster2
{
    [CreateAssetMenu(fileName = "New MAS_7104_pg1", menuName = "Scriptable/Monster/Attack/MAS_7104_pg1")]
    [System.Serializable]
    public class MAS_7104_pg1: MAS_ProjectileCreation
    {
        public float angleDiff;


        protected override void SetVelocityToPlayer(Projectile createdProj)
        {
            
            float angle = Vector2.SignedAngle(Vector2.right, GameManager.instance.ControllingEntity.Position - createdProj.transform.position);
            
            float randomAngle = Random.Range(angle - angleDiff * 0.5f, angle + angleDiff * 0.5f);

            createdProj.realInitVelocity = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad))*createdProj.firstVelocity.magnitude;
            Debug.DrawRay(createdProj.transform.position, createdProj.realInitVelocity, Color.blue, 1f);
        }
        
    }
}