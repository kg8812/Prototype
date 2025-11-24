using System.Collections.Generic;
using System.Text;
using Apis;
using Apis;
using Apis.Managers;
using Apis.UI;
using Save.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _02_Scripts.UI.UI_Scene
{
    public class UI_Collection2 : UI_HeaderMenu_nto1
    {
        enum Texts
        {
            ListTitle,
            DescriptionText,
            NameText,
        }

        enum Images
        {
            Image
        }

        enum TitleList
        {
            Weapon = 0,
            Acc,
            Enemy,
            Story,
            Etc,
        }

        private TextMeshProUGUI collectionTitle;
        private TextMeshProUGUI descriptionText;
        private TextMeshProUGUI nameText;
        
        private GameObject imageObj;
        
        public ScrollRect scroll;
        
        
        
        private StringBuilder namestr = new StringBuilder();
        private StringBuilder description = new StringBuilder();
        
        
        private List<UIEffector> list = new();
        public Transform listParent;

        public override void Init()
        {
            base.Init();
        }

        protected override void FocusChanged(int id)
        {
            SetList((TitleList)id);
            base.FocusChanged(id);
        }

        private void SetList(TitleList type)
        {
            HashSet<int> set;
            scroll.verticalNormalizedPosition = 1;
            int i = 0;
            nameText.text = "";
            descriptionText.text = "";
            switch (type)
            {
                case TitleList.Weapon:
                    collectionTitle.text = "무기 리스트";
                    set = DataAccess.Codex.DataInt(CodexData.CodexType.Item);
                    break;
                case TitleList.Acc:
                    collectionTitle.text = "악세서리 리스트";
                    set = DataAccess.Codex.DataInt(CodexData.CodexType.Item);
                    break;
                case TitleList.Etc:
                case TitleList.Enemy:
                case TitleList.Story:
                default:
                    return;
            }

            foreach (var x in set)
            {
                namestr.Clear();
                description.Clear();
                
                switch (type)
                {
                    case TitleList.Weapon:
                        if (!WeaponData.DataLoad.TryGetWeaponData(x, out var data1)) continue;
                        namestr.Append(StrUtil.GetEquipmentName(data1.weaponId));
                        description.Append(StrUtil.GetEquipmentDesc(data1.weaponId));
                        break;
                    case TitleList.Acc:
                        if (!AccessoryData.DataLoad.TryGetData(x, out var data2)) continue;
                        namestr.Append(StrUtil.GetEquipmentName(data2.accId));
                        description.Append(StrUtil.GetEquipmentDesc(data2.accId));
                        break;
                }

                UIEffector uiEffector;
                if (i < list.Count)
                {
                    list[i].gameObject.SetActive(true);
                    uiEffector = list[i];
                }
                else
                {
                    uiEffector = GameManager.UI.MakeSubItem("CollectionButton", transform)
                        .GetComponent<UIEffector>();
                    uiEffector.transform.SetParent(listParent);
                    list.Add(uiEffector);
                }

                uiEffector.GetComponentInChildren<TextMeshProUGUI>().text = namestr.ToString();
                // uiElement.selectable..RemoveAllListeners();
                // uiElement.selectable.onClick.AddListener(() =>
                // {
                //     nameText.text = namestr.ToString();
                //     descriptionText.text = description.ToString();
                //     imageObj.SetActive(true);
                // });
                
                UI_Selectable s = uiEffector.GetComponent<UI_Selectable>();
                // s.OnSelected.RemoveAllListeners();
                // s.OnSelected.AddListener(() =>
                // {
                //     // if (a is AxisEventData)
                //     // {
                //     //     
                //     // }
                //     
                //     Vector2 pos = scroll.content.localPosition;
                //     // pos.y = scroll.GetSnapToPositionToBringChildIntoView(uiElement.myBtn.image.rectTransform).y;
                //     scroll.content.localPosition = pos;
                // });
                i++;
            }
        }
    }
}