using Apis;
using Apis.DataType;
using Default;
using System.Collections.Generic;
using Apis.Managers;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace UI
{
    public class UI_BuffIcon : UI_Base, ISubject<UI_BuffIcon>
    {
        interface IStrategy
        {
            int Count { get; }
            void Update();
            void ResetText();
            void Detach();
        }

        class SubBuffUpdate : IStrategy,IObserver<List<SubBuff>>
        {
            readonly SubBuffTypeList typeList;
            readonly UI_BuffIcon icon;
            private Sprite sp;
            public SubBuffUpdate(UI_BuffIcon icon, SubBuffTypeList typeList)
            {
                this.icon = icon;
                this.typeList = typeList;
                typeList.Attach(this);
                sp = ResourceUtil.Load<Sprite>(typeList.option.iconPath);
                if (sp != null)
                {
                    icon.image.sprite = sp;
                }
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
                    string desc = LanguageManager.Str(typeList.option.description);
                    float[] amount = new float[3];
                    
                    typeList.List.ForEach(x => { amount[0] += x.Amount[0]; amount[1] += x.Amount[1]; });
                    desc = desc.Replace("{amount1}", amount[0].ToString()).Replace("{amount2}", amount[1].ToString());
                    icon.description.descTmp.text = desc;
                }
            }

            public void Notify(List<SubBuff> value)
            {
                ResetText();
            }
            public void Detach()
            {
                typeList.Detach(this);
                if (sp != null)
                {
                    Addressables.Release(sp);
                }

                sp = null;
            }
        }
        class BuffUpdate : IStrategy,IObserver<List<SubBuff>>
        {
            readonly SubBuffList subList;
            readonly Buff buff;
            readonly UI_BuffIcon icon;


            public BuffUpdate(UI_BuffIcon icon, SubBuffList subList)
            {
                this.icon = icon;
                this.buff = subList.buff;
                this.subList = subList;
                subList.Attach(this);

                if (buff.Icon != null)
                {
                    icon.image.sprite = buff.Icon;
                }
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
                string desc = LanguageManager.Str(buff.BuffDesc).Replace("{amount1}", (buff.BuffPower[0] * subList.Count).ToString()).
                    Replace("{amount2}", (buff.BuffPower.Length > 1 ? buff.BuffPower[1] * subList.Count : 0).ToString());
                icon.description.descTmp.text = desc;
            }

            public void Notify(List<SubBuff> value)
            {
                ResetText();
            }
            public void Detach()
            {
                subList.Detach(this);
            }
        }

        class BuffGroup : IStrategy
        {
            public int Count => 0;
            readonly UI_BuffIcon icon;
            readonly BuffGroupDataType groupData;
            private Sprite sp;
            public BuffGroup(UI_BuffIcon icon, BuffGroupDataType groupData)
            {
                this.groupData = groupData;
                this.icon = icon;
                sp = ResourceUtil.Load<Sprite>(groupData.iconPath);
                if (sp != null)
                {
                    icon.image.sprite = sp;
                }
            }
            public void ResetText()
            {
                if (groupData != null)
                {
                    string desc = LanguageManager.Str(groupData.buffDesc);
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
        enum Images
        {
            CdImage,UI_BuffIcon
        }
        enum Texts
        {
            Stacks,
        }
        IStrategy strategy;

        Image cdImage;
        [HideInInspector] public UI_BuffDesc description;
        TextMeshProUGUI stackText;

        SubBuffList subList;
        SubBuffTypeList typeList;

        public SubBuffTypeList TypeList => typeList;
        public SubBuffList SubList => subList;

        private Image image;
        public int Count
        {
            get
            {
                if (strategy == null) return 0;
                return strategy.Count;
            }
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
                        Vector3 screenPoint = Input.mousePosition;
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
                    if (description != null)
                    {
                        description.TurnOff();
                    }
                }, Define.UIEvent.PointExit);
            }
        }

        public void Init(SubBuffTypeList buffList)
        {
            strategy = new SubBuffUpdate(this, buffList);
            typeList = buffList;
        }
        public void Init(SubBuffList buffList)
        {
            strategy = new BuffUpdate(this, buffList);
            subList = buffList;
        }
        public void Init(BuffGroupDataType groupData)
        {
            strategy = new BuffGroup(this, groupData);
        }

        protected override void Deactivated()
        {
            base.Deactivated();
            if (description.currentIcon == this)
            {
                description.TurnOff();
            }
            strategy?.Detach();
        }

        private void Update()
        {
            strategy?.Update();
            stackText.text = Count > 0 ? Count.ToString() : "";
        }

        readonly List<IObserver<UI_BuffIcon>> observers = new();
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
    }
}