using Spine.Unity;
using UnityEngine;

namespace Apis
{
    public interface IMecanimUser
    {
        public Transform SkeletonTrans { get; set; }
        public SkeletonMecanim Mecanim { get; set; }
    }
}