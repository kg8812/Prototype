using System.Collections.Generic;
using System.Linq;
using Apis;
using Default;
using UnityEngine;
using UnityEngine.UI;

namespace Apis.UI
{
    public class UI_BuffCollector : UI_Base, IObserver<UI_BuffIcon>
    {
        [HideInInspector] public List<UI_BuffIcon> icons = new();
        [HideInInspector] public UI_BuffDesc description;
        private IBuffUser _user;

        private readonly Dictionary<int, UI_BuffIcon> groupIcons = new();

        public void Notify(UI_BuffIcon value)
        {
            if (value.Count == 0)
            {
                RemoveSubItem(value);
                icons.Remove(value);
            }
        }

        public override void Init()
        {
            base.Init();
            description = GameManager.UI.MakeSubItem("UI_BuffDesc", transform).GetComponent<UI_BuffDesc>();
            Utils.GetOrAddComponent<LayoutElement>(description.gameObject).ignoreLayout = true;
            description.TurnOff();
        }

        public void Init(IBuffUser user)
        {
            if (user == null) return;
            
            _user = user;
            user.SubBuffManager.Collector.buffUIEvent.AddListener(Invoke);
            if (user.gameObject.TryGetComponent(out IEventUser eventUser))
            {
                eventUser.EventManager.AddEvent(EventType.OnBuffGroupAdd, SetBuffIcon);
                eventUser.EventManager.AddEvent(EventType.OnBuffGroupRemove, RemoveBuffIcon);
            }
            
        }

        protected override void Deactivated()
        {
            base.Deactivated();
            if (_user == null) return;
            _user.SubBuffManager.Collector.buffUIEvent.RemoveListener(Invoke);
            
            if (_user.gameObject.TryGetComponent(out IEventUser eventUser))
            {
                eventUser.EventManager.RemoveEvent(EventType.OnBuffGroupAdd, SetBuffIcon);
                eventUser.EventManager.RemoveEvent(EventType.OnBuffGroupRemove, RemoveBuffIcon);
            }
            
        }

        public void SetDescPivot(Vector2 pivot)
        {
            description.rect.pivot = pivot;
        }

        // 현재 subBuffCollector쪽에 서브버프 추가될때마다 아이콘 추가되도록 연결 되어있음

        public void Invoke(BuffInfo info)
        {
            if (!info.buff.ShowIcon || (info.typeList == null && info.subList == null)) return;

            if (info.subList != null)
            {
                if (info.subList.Count == 0) return;
                if (icons.All(x => x.SubList != info.subList))
                {
                    var icon = GameManager.UI.MakeSubItem("UI_BuffIcon", transform).GetComponent<UI_BuffIcon>();

                    icons.Add(icon);
                    icon.Init(info.subList);
                    icon.Attach(this);
                    description.transform.SetAsLastSibling();
                }
            }
            else
            {
                if (info.typeList == null || info.typeList.Count == 0) return;
                if (icons.All(x => x.TypeList != info.typeList))
                {
                    var icon = GameManager.UI.MakeSubItem("UI_BuffIcon", transform).GetComponent<UI_BuffIcon>();

                    icons.Add(icon);
                    icon.Init(info.typeList);
                    icon.Attach(this);
                    description.transform.SetAsLastSibling();
                }
            }
        }

        // 효과 아이콘 (buffGroup으로 효과 추가할 때)
        private void SetBuffIcon(EventParameters parameters)
        {
            if (parameters == null ||
                !BuffDatabase.DataLoad.TryGetBuffGroupData(parameters.Get<BuffEventData>().buffGroupId, out var group)) return;
            if (!group.showIcon) return;

            var icon = GameManager.UI.MakeSubItem("UI_BuffIcon", transform).GetComponent<UI_BuffIcon>();

            groupIcons.Add(parameters.Get<BuffEventData>().buffGroupId, icon);
            icon.Init(group);
            description.transform.SetAsLastSibling();
        }

        private void RemoveBuffIcon(EventParameters parameters)
        {
            if (parameters == null) return;

            if (groupIcons.ContainsKey(parameters.Get<BuffEventData>().buffGroupId))
            {
                RemoveSubItem(groupIcons[parameters.Get<BuffEventData>().buffGroupId]);
                groupIcons.Remove(parameters.Get<BuffEventData>().buffGroupId);
            }
        }
    }
}