using Default;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Apis
{
    public abstract class SpineRenderer : MonoBehaviour, IActorRenderer, IMecanimUser
    {
        [Tooltip("캐릭터 가운데 위치 값")] [SerializeField]
        private Vector3 pivot;

        [LabelText("상단 위치")] [SerializeField] private Vector3 topPivot;

        private SkeletonMecanimRootMotion _rootMotion;

        private Bone centerBone;

        public SkeletonMecanimRootMotion RootMotion =>
            _rootMotion ??= transform.GetComponentInParentAndChild<SkeletonMecanimRootMotion>();

        protected virtual void Awake()
        {
            Mecanim = GetComponentInChildren<SkeletonMecanim>();
            MeshRenderer = null;

            if (Mecanim != null)
            {
                SkeletonTrans = Mecanim.transform;
                centerBone = Mecanim.Skeleton.FindBone("ctrl");
            }

            if (SkeletonTrans != null) MeshRenderer = SkeletonTrans.GetComponent<MeshRenderer>();
        }

        public MeshRenderer MeshRenderer { get; private set; }

        public Vector3 Pivot => pivot;
        public Vector3 TopPivot => topPivot;

        public Vector3 GetPosition()
        {
            centerBone?.UpdateWorldTransform();
            return centerBone?.GetWorldPosition(SkeletonTrans) ?? transform.position + pivot;
        }

        public void SetPosition(Vector3 position)
        {
            if (centerBone != null) pivot = centerBone.GetWorldPosition(Mecanim.transform) - transform.position;
            transform.position = position - pivot;
        }

        public void UpdateRenderer()
        {
        }

        public void Hide()
        {
            if (!MeshRenderer.enabled) return;
            MeshRenderer.enabled = false;
        }

        public void Appear()
        {
            if (MeshRenderer.enabled) return;
            MeshRenderer.enabled = true;
        }

        public SkeletonMecanim Mecanim { get; set; }
        public Transform SkeletonTrans { get; set; }
    }
}