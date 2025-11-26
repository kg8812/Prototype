using Default;
using GameStateSpace;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis
{
    public class Interaction : MonoBehaviour
    {
        // 상호작용 클래스
        // 상호작용이 필요한 오브젝트에 추가하고 오브젝트의 클래스에 IOnInteract 인터페이스를 추가하면 됩니다.
        // 자식오브젝트로 Text 프리팹도 추가하고 상호작용 텍스트 표시 할 위치에 놔주세요.

        SpriteRenderer text; // 상호작용 단축키 표시용 텍스트
        private IOnInteract _interact;

        [InfoBox("상호작용 불가 상태일시 해당 콜라이더가 꺼집니다. 상호작용용 콜라이더를 넣어주세요.")]
        [LabelText("상호작용 콜라이더")] public Collider2D col;
        
        private void Awake()
        {
            // 상호작용 텍스트 검색 및 참조
            
            text = transform.Find("FKey")?.GetComponent<SpriteRenderer>();
            
            if (text == null)
            {
                text = GameManager.Factory.Get<SpriteRenderer>(FactoryManager.FactoryType.Normal, "InteractionText");
                
                text.transform.SetParent(transform);
                if (TryGetComponent(out SpineRenderer actor))
                {
                    var boneFollower = SpineUtils.AddCustomBoneFollower(actor.Mecanim, "center", text.gameObject);
                    boneFollower.offset = actor.TopPivot;
                }
                else
                {
                    text.transform.localPosition = new Vector2(0, 1f);
                }
                text.name = "FKey";
            }
            
            text.gameObject.SetActive(false);
            text.transform.localScale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, 1);
        }

        private void Start()
        {
            _interact = Utils.GetComponentInParentAndChild<IOnInteract>(gameObject);
        }

        [ReadOnly] public bool isActive;

        public void OnActive()
        {
            isActive = true;
            text.gameObject.SetActive(true);
        }
        public void OffActive()
        {
            isActive = false;
            text.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (col == null || _interact == null) return;

            col.enabled = _interact.IsInteractable();

        }

        void OnDisable()
        {
            text.gameObject.SetActive(false);
        }
    }
}
