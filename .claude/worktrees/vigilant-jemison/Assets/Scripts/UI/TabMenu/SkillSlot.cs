using Apis;
using Apis.UI;
using UnityEngine;
using UnityEngine.UI;

    public class SkillSlot: UIAsset_Toggle
    {
        [SerializeField] private Image itemImg;
        public Skill CurSkill { get; set; }
        
        public void UpdateSkill()
        {
            if (CurSkill == null)
            {
                itemImg.gameObject.SetActive(false);
                return;
            }
            
            itemImg.gameObject.SetActive(true);
            itemImg.sprite = CurSkill.SkillImage;
        }
    }
