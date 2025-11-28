using UnityEngine;

namespace Apis
{
    public class ProjectileExtension : MonoBehaviour
    {
        private Projectile _projectile;
        protected Projectile projectile => _projectile ??= GetComponent<Projectile>();

        public virtual void Init(ProjectileInfo info)
        {
        }

        public virtual void Destroy()
        {
        }
    }
}