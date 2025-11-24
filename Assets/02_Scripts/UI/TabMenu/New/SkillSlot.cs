using Apis;
using Apis.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Apis
{
    public class SkillSlot: UIAsset_Toggle
    {
        [SerializeField] private Image itemImg;
        [SerializeField] private Image backgroundImg;
        [SerializeField] private bool isActive;
        public Skill CurSkill { get; private set; }
        
        public void UpdateSkill()
        {
            if (GameManager.instance.Player == null)
            {
                Debug.LogError("플레이어가 없는데 player skill을 조회함.");
            }
            else
            {
                if (isActive)
                {
                    // TODO: active skill slot img and skill update
                }
                else
                {
                    // TODO: passive skill slot img and skill update
                }
            }
        }
    }
}