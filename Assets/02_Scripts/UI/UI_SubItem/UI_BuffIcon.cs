using System.Collections.Generic;
using Apis;
using Apis.DataType;
using Apis.Managers;
using Default;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace UI
{
    public class UI_BuffIcon : UI_Base, ISubject<UI_BuffIcon>
    {
        private IStrategy strategy;

        private Image cdImage;
        [HideInInspector] public UI_BuffDesc description;
        private TextMeshProUGUI stackText;

        private Image image;

        private readonly List<IObserver<UI_BuffIcon>> observers = new();

        public SubBuffTypeList TypeList { get; private set; }

        public SubBuffList SubList { get; private set; }

        public int Count
        {
            get
            {
                if (strategy == null) return 0;
                return strategy.Count;
            }
        }

        private void Update()
        {
            strategy?.Update();
            stackText.text = Count > 0 ? Count.ToString() : "";
        }

        public void Attach(IObserver<UI_BuffIcon> observer)
        {
            observers.Add(observer);
        }

        public void Detach(IObserver<UI_BuffIcon> observer)
        {
            observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            observers.ForEach(x => x.Notify(this));
        }

        public override void Init()
        {
            base.Init();
            Bind<Image>(typeof(Images));
            Bind<TextMeshProUGUI>(typeof(Texts));
            cdImage = Get<Image>((int)Images.CdImage);
            stackText = Get<TextMeshProUGUI>((int)Texts.Stacks);
            description = GetComponentInParent<UI_BuffCollector>().description;
            image = Get<Image>((int)Images.UI_BuffIcon);
            if (description != null)
            {
                AddUIEvent(gameObject, _ =>
                {
                    if (description != null)
                    {
                        var screenPoint = Input.mousePosition;
                        screenPoint.z = 10;
                        screenPoint = CameraManager.instance.UICam.ScreenToWorldPoint(screenPoint);
                        description.transform.position = screenPoint;
                        description.TurnOn();
                    }
                }, Define.UIEvent.PointStay);
                AddUIEvent(gameObject, _ =>
                {
                    if (description != null)
                    {
                        description.TurnOn();
                        description.currentIcon = this;

                        strategy.ResetText();
                    }
                }, Define.UIEvent.PointEnter);
                AddUIEvent(gameObject, _ =>
                {
                    if (description != null) description.TurnOff();
                }, Define.UIEvent.PointExit);
            }
        }

        public void Init(SubBuffTypeList buffList)
        {
            strategy = new SubBuffUpdate(this, buffList);
            TypeList = buffList;
        }

        public void Init(SubBuffList buffList)
        {
            strategy = new BuffUpdate(this, buffList);
            SubList = buffList;
        }

        public void Init(BuffGroupDataType groupData)
        {
            strategy = new BuffGroup(this, groupData);
        }

        protected override void Deactivated()
        {
            base.Deactivated();
            if (description.currentIcon == this) description.TurnOff();
            strategy?.Detach();
        }

        private interface IStrategy
        {
            int Count { get; }
            void Update();
            void ResetText();
            void Detach();
        }

        private class SubBuffUpdate : IStrategy, IObserver<List<SubBuff>>
        {
            private readonly UI_BuffIcon icon;
            private readonly SubBuffTypeList typeList;
            private Sprite sp;

            public SubBuffUpdate(UI_BuffIcon icon, SubBuffTypeList typeList)
            {
                this.icon = icon;
                this.typeList = typeList;
                typeList.Attach(this);
                sp = ResourceUtil.Load<Sprite>(typeList.option.iconPath);
                if (sp != null) icon.image.sprite = sp;
            }

            public void Notify(List<SubBuff> value)
            {
                ResetText();
            }

            public int Count
            {
                get
                {
                    if (typeList == null) return 0;
                    return typeList.Count;
                }
            }

            public void Update()
            {
                if (typeList == null || typeList.Count == 0)
                {
                    icon.NotifyObservers();
                    icon.Deactivated();
                    return;
                }

                icon.cdImage.fillAmount = 1 - typeList.CurTime / typeList.Duration;
            }

            public void ResetText()
            {
                if (icon.description.currentIcon == icon)
                {
                    var desc = LanguageManager.Str(typeList.option.description);
                    var amount = new float[3];

                    typeList.List.ForEach(x =>
                    {
                        amount[0] += x.Amount[0];
                        amount[1] += x.Amount[1];
                    });
                    desc = desc.Replace("{amount1}", amount[0].ToString()).Replace("{amount2}", amount[1].ToString());
                    icon.description.descTmp.text = desc;
                }
            }

            public void Detach()
            {
                typeList.Detach(this);
                if (sp != null) Addressables.Release(sp);

                sp = null;
            }
        }

        private class BuffUpdate : IStrategy, IObserver<List<SubBuff>>
        {
            private readonly Buff buff;
            private readonly UI_BuffIcon icon;
            private readonly SubBuffList subList;


            public BuffUpdate(UI_BuffIcon icon, SubBuffList subList)
            {
                this.icon = icon;
                buff = subList.buff;
                this.subList = subList;
                subList.Attach(this);

                if (buff.Icon != null) icon.image.sprite = buff.Icon;
            }

            public void Notify(List<SubBuff> value)
            {
                ResetText();
            }

            public int Count
            {
                get
                {
                    if (subList == null) return 0;
                    return subList.Count;
                }
            }

            public void Update()
            {
                if (subList == null || subList.Count == 0)
                {
                    icon.NotifyObservers();
                    icon.Deactivated();
                    return;
                }

                icon.cdImage.fillAmount = 1 - subList.CurTime / subList.Duration;
            }

            public void ResetText()
            {
                if (icon.description.currentIcon != icon || buff.BuffDesc != 0) return;
                var desc = LanguageManager.Str(buff.BuffDesc)
                    .Replace("{amount1}", (buff.BuffPower[0] * subList.Count).ToString()).Replace("{amount2}",
                        (buff.BuffPower.Length > 1 ? buff.BuffPower[1] * subList.Count : 0).ToString());
                icon.description.descTmp.text = desc;
            }

            public void Detach()
            {
                subList.Detach(this);
            }
        }

        private class BuffGroup : IStrategy
        {
            private readonly BuffGroupDataType groupData;
            private readonly UI_BuffIcon icon;
            private Sprite sp;

            public BuffGroup(UI_BuffIcon icon, BuffGroupDataType groupData)
            {
                this.groupData = groupData;
                this.icon = icon;
                sp = ResourceUtil.Load<Sprite>(groupData.iconPath);
                if (sp != null) icon.image.sprite = sp;
            }

            public int Count => 0;

            public void ResetText()
            {
                if (groupData != null)
                {
                    var desc = LanguageManager.Str(groupData.buffDesc);
                    icon.description.descTmp.text = desc;
                }
            }

            public void Update()
            {
            }

            public void Detach()
            {
                if (sp != null)
                {
                    Addressables.Release(sp);
                    sp = null;
                }
            }
        }

        private enum Images
        {
            CdImage,
            UI_BuffIcon
        }

        private enum Texts
        {
            Stacks
        }
    }
}