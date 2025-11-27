using Managers;
using Unity.Cinemachine;
using UnityEngine;

namespace Directing
{
    [RequireComponent(typeof(CinemachineTargetGroup))]
    public class TargetGroupCamera : Singleton<TargetGroupCamera>
    {
        private CinemachineTargetGroup _citg;

        protected override void Awake()
        {
            base.Awake();
            _citg = GetComponent<CinemachineTargetGroup>();
        }


        public void RegisterTarget(Transform trans, float weight = 1, float radius = 1)
        {
            _citg.AddMember(trans, weight, radius);
            CameraManager.instance.UpdateConfinerMaxDistance();
        }

        public void RemoveTarget(Transform trans)
        {
            _citg.RemoveMember(trans);
            CameraManager.instance.UpdateConfinerMaxDistance();
        }

        public void ResetTargets()
        {
            foreach (var t in _citg.Targets) RemoveTarget(t.Object);
        }

        public void DoUpdate()
        {
            _citg.DoUpdate();
        }

        public void AdjustTargetRadius(Transform trans, float radius)
        {
            for (var i = 0; i < _citg.Targets.Count; i++)
                if (_citg.Targets[i].Object == trans)
                    _citg.Targets[i].Radius = radius;

            CameraManager.instance.UpdateConfinerMaxDistance();
        }
    }
}