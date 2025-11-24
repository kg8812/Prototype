using Apis;
using Default;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Apis
{
    public class UI_AtkItemIcon : UI_Base
    {
        enum Images
        {
            Front,
            SkillIcon,
            DurationImage,
            Frame,
        }

        enum Texts
        {
            SkillCdText
        }

        protected enum GameObjects
        {
            SkillCd,
            Deactive
        }

        protected Image skillIcon; // 아이콘
        private GameObject deactive; // 비활성화 아이콘
        protected Image skillCdFront; // 쿨타임 이미지
        protected Image durationImage; // 지속시간 테두리
        protected GameObject skillCd; // 지속시간 테두리
        protected TextMeshProUGUI skillCdText; // 쿨타임 텍스트
        [HideInInspector] public Image activatedFrame; // 액티브 프레임

        [HideInInspector] public bool showCDImage;

        public override void Init()
        {
            base.Init();
            Bind<Image>(typeof(Images));
            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<GameObject>(typeof(GameObjects));
            durationImage = Get<Image>((int)Images.DurationImage);
            skillCdText = Get<TextMeshProUGUI>((int)Texts.SkillCdText);
            skillCd = Get<GameObject>((int)GameObjects.SkillCd);
            durationImage.gameObject.SetActive(false);
            activatedFrame = GetImage((int)Images.Frame);
            activatedFrame?.gameObject.SetActive(false);
            skillCdFront = Get<Image>((int)Images.Front);
            skillCd.SetActive(false);
            skillCdText.text = "";
            showCDImage = true;
            skillIcon = Get<Image>((int)Images.SkillIcon);
            deactive = Get<GameObject>((int)GameObjects.Deactive);
            deactive.SetActive(true);
            SetItem(item);
        }

        private IAttackItem item;
        [ReadOnly] public ActiveSkill Skill;
        
        public interface IUpdate
        {
            public void Update();

            public void Change();
        }

        public class NoUpdate : IUpdate
        {
            private UI_AtkItemIcon _icon;
            public NoUpdate(UI_AtkItemIcon icon)
            {
                _icon = icon;
            }
            public void Update()
            {
            }

            public void Change()
            {
                _icon.durationImage.gameObject.SetActive(false);
                _icon.skillCdText.text = "";
                _icon.skillCd.gameObject.SetActive(false);
            }
        }
        public class NormalCdUpdate : IUpdate
        {
            private UI_AtkItemIcon _icon;

            public NormalCdUpdate(UI_AtkItemIcon icon)
            {
                _icon = icon;
            }

            public void Update()
            {
                if (Mathf.Approximately(_icon.Skill.CurCd, 0) || !_icon.showCDImage)
                {
                    if (_icon.skillCd.activeSelf)
                    {
                        _icon.skillCd.SetActive(false);
                    }
                }
                else
                {
                    if (!_icon.skillCd.activeSelf)
                    {
                        _icon.skillCd.SetActive(true);
                    }

                    _icon.skillCdFront.fillAmount = Mathf.Clamp01(_icon.Skill.CurCd / _icon.Skill.Cd);
                }
                //_icon.skillCdText.text = _icon.skillCd.fillAmount > 0 ? _icon.Skill.CurCd.ToString("0.0") : "";
            }

            public void Change()
            {
                _icon.durationImage.gameObject.SetActive(false);
                _icon.skillCdText.text = "";
            }
        }

        public class CastingUpdate : IUpdate
        {
            private UI_AtkItemIcon _icon;

            public CastingUpdate(UI_AtkItemIcon icon)
            {
                _icon = icon;
            }

            public void Update()
            {
                if (_icon.skillCd.activeSelf)
                {
                    _icon.skillCd.SetActive(false);
                }
                _icon.skillCdFront.fillAmount = 0;
                _icon.durationImage.fillAmount = Mathf.Clamp01(_icon.Skill.CurCastTime / _icon.Skill.CastTime);
                //_icon.skillCdText.text = _icon.Skill.CurCastTime > 0 ? _icon.Skill.CurCastTime.ToString("0.0") : "";
            }

            public void Change()
            {
                _icon.durationImage.gameObject.SetActive(true);
                _icon.skillCdText.text = "";
            }
        }

        public class ChargingUpdate : IUpdate
        {
            private UI_AtkItemIcon _icon;

            public ChargingUpdate(UI_AtkItemIcon icon)
            {
                _icon = icon;
            }

            public void Update()
            {
                if (_icon.skillCd.activeSelf)
                {
                    _icon.skillCd.SetActive(false);
                }
                _icon.skillCdFront.fillAmount = 0;
                _icon.durationImage.fillAmount = Mathf.Clamp01(_icon.Skill.CurChargeTime / _icon.Skill.ChargeTime);
                //_icon.skillCdText.text = _icon.Skill.CurChargeTime < _icon.Skill.ChargeTime ? _icon.Skill.CurChargeTime.ToString("0.0") : "";
            }

            public void Change()
            {
                _icon.durationImage.gameObject.SetActive(true);
                _icon.skillCdText.text = "";
            }
        }

        public class DurationUpdate : IUpdate
        {
            private UI_AtkItemIcon _icon;

            public DurationUpdate(UI_AtkItemIcon icon)
            {
                _icon = icon;
            }

            public void Update()
            {
                _icon.durationImage.fillAmount = Mathf.Clamp01(_icon.Skill.CurDuration / _icon.Skill.Duration);
                // _icon.skillCdText.text = _icon.Skill.CurDuration > 0 ? _icon.Skill.CurDuration.ToString("0.0") : "";
            }

            public void Change()
            {
                _icon.skillCd.gameObject.SetActive(false);
                _icon.durationImage.gameObject.SetActive(true);
                _icon.skillCdText.text = "";
            }
        }

        public class StackUpdate : IUpdate
        {
            private UI_AtkItemIcon _icon;

            public StackUpdate(UI_AtkItemIcon icon)
            {
                _icon = icon;
            }

            public void Update()
            {
                if (_icon.Skill.CurStack == _icon.Skill.MaxStack || Mathf.Approximately(_icon.Skill.Cd, 0) ||
                    Mathf.Approximately(_icon.Skill.CurCd, _icon.Skill.Cd) || !_icon.showCDImage)
                {
                    _icon.skillCdFront.fillAmount = 0;
                    if (_icon.skillCd.activeSelf)
                    {
                        _icon.skillCd.SetActive(false);
                    }
                }
                else
                {
                    _icon.skillCdFront.fillAmount = Mathf.Clamp01(_icon.Skill.CurCd / _icon.Skill.Cd);
                    if (!_icon.skillCd.activeSelf)
                    {
                        _icon.skillCd.SetActive(true);
                    }
                }

                _icon.skillCdText.text = _icon.Skill.MaxStack > 1 ? _icon.Skill.CurStack.ToString() : "";
            }

            public void Change()
            {
                _icon.durationImage.gameObject.SetActive(false);
            }
        }

        protected IUpdate updateType;

        public void ChangeType(IUpdate update)
        {
            updateType = update;
            update.Change();
        }

        public void SetItem(IAttackItem item)
        {
            if (item == null)
            {
                WhenItemIsNull();
                return;
            }
            this.item = item;
            WhenItemIsSet();
        }
        public void WhenItemIsNull()
        {
            ChangeType(new NoUpdate(this));
            durationImage.gameObject.SetActive(false);
            skillCd.SetActive(false);
            skillCdFront.fillAmount = 0;
            deactive.SetActive(true);
            skillCdText.text = "";
            Skill = null;
            activatedFrame.gameObject.SetActive(false);
        }

        public void WhenItemIsSet()
        {
            deactive.SetActive(false);
            activatedFrame.gameObject.SetActive(false);
        }
        public void Update()
        {
            if ((object)Skill != null)
            {
                updateType?.Update();
            }
        }
    }
}