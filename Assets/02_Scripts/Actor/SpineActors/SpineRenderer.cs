using Apis;
using Default;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Apis
{
    public abstract class SpineRenderer : MonoBehaviour,IActorRenderer, IMecanimUser
    {
        private SkeletonMecanimRootMotion _rootMotion;

        public SkeletonMecanimRootMotion RootMotion =>
            _rootMotion ??= transform.GetComponentInParentAndChild<SkeletonMecanimRootMotion>();
        
        public SkeletonMecanim Mecanim { get; set; }
        public Transform SkeletonTrans { get; set; }

        private Bone centerBone;

        MeshRenderer _meshRenderer;
        public MeshRenderer MeshRenderer => _meshRenderer;
        
        protected virtual void Awake()
        {
            Mecanim = GetComponentInChildren<SkeletonMecanim>();
            _meshRenderer = null;

            if (Mecanim != null)
            {
                SkeletonTrans = Mecanim.transform;
                centerBone = Mecanim.Skeleton.FindBone("ctrl");
            }

            if (SkeletonTrans != null)
            {
                _meshRenderer = SkeletonTrans.GetComponent<MeshRenderer>();
            }
        }

        [Tooltip("캐릭터 가운데 위치 값")]
        [SerializeField] private Vector3 pivot;
        [LabelText("상단 위치")] [SerializeField] private Vector3 topPivot;

        public Vector3 Pivot => pivot;
        public Vector3 TopPivot => topPivot;
        
        public Vector3 GetPosition()
        {
            centerBone?.UpdateWorldTransform();
            return centerBone?.GetWorldPosition(SkeletonTrans) ?? transform.position + pivot;
        }

        public void SetPosition(Vector3 position)
        {
            if (centerBone != null)
            {
                pivot = centerBone.GetWorldPosition(Mecanim.transform) - transform.position;
            }
            transform.position = position - pivot;
        }

        public void UpdateRenderer()
        {
        }
        
        public void Hide()
        {
            if(!_meshRenderer.enabled) return;
            _meshRenderer.enabled = false;
        
        }

        public void Appear()
        {
            if(_meshRenderer.enabled) return;
            _meshRenderer.enabled = true;
        }
    }
}