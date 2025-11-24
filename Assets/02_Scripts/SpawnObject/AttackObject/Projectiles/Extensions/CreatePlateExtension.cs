using Apis;
using Default;
using Sirenix.OdinInspector;

namespace Apis
{
    public class CreatePlateExtension: ProjectileExtension
    {
        [TabGroup("생성설정")] [LabelText("장판 이름")]
        public string plateName;
        
        [TabGroup("생성설정")] [LabelText("장판 info")]
        public ProjectileInfo plateInfo;
        
        private void Awake()
        {
            projectile.AddEvent(EventType.OnDestroy, Destroy);
        }

        void Destroy(EventParameters _)
        {
            if (projectile == null) return;
            if (Utils.GetLowestPointByRay(projectile.Position, LayerMasks.GroundWallPlatform, out var pos))
            {
                AttackObject a = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.AttackObject, plateName, pos);
                a.Init(projectile._attacker, new FixedAmount(plateInfo.dmg),plateInfo.duration);
                if (plateInfo != null && a is Projectile p)
                {
                    p.Init(plateInfo);
                }
            }
        }
    }
}