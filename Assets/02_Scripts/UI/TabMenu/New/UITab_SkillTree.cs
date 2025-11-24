using System.Collections.Generic;
using Apis.SkillTrees;
using Apis.Managers;
using Apis.UI.Focus;
using Default;
using Managers;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Apis
{
    public class UITab_SkillTree: UI_FocusContent
    {
        public TextMeshProUGUI[] categoryNames;
        public TextMeshProUGUI[] categoryDescs;

        public GameObject descriptionWindow;
        public TextMeshProUGUI treeName;
        public TextMeshProUGUI treeDesc;
        public TextMeshProUGUI treeTag;
    
        public Image[] trees;

    
        private Dictionary<int, SkillTrees> _treeDict = new();
        private Dictionary<int, SkillTrees> treeDict => _treeDict ??= new();

        public override void Init()
        {
            base.Init();
            for (int i = 0; i < trees.Length; i++)
            {
                treeDict.TryAdd(i,null);
            }

            for (int i = 0; i < trees.Length; i ++)
            {
                int temp = i;
                UI_Base.AddUIEvent(trees[i].gameObject, x =>
                {
                    SetDescription(treeDict[temp]);
                },Define.UIEvent.PointEnter);
                UI_Base.AddUIEvent(trees[i].gameObject, x =>
                {
                    Vector3 screenPoint = Input.mousePosition;
                    screenPoint.z = 10;
                    screenPoint = CameraManager.instance.UICam.ScreenToWorldPoint(screenPoint);
                    descriptionWindow.transform.position = screenPoint;
                },Define.UIEvent.PointStay);
                UI_Base.AddUIEvent(trees[i].gameObject, x =>
                {
                    descriptionWindow.SetActive(false);
                },Define.UIEvent.PointExit);
            }
        }

        public override void OnOpen()
        {
            base.OnOpen();
            Player player = GameManager.instance.Player;

            descriptionWindow.SetActive(false);
        
            categoryNames[0].text = StrUtil.GetTagName(101 + 3 * (int)player.playerType);
            categoryNames[1].text = StrUtil.GetTagName(102 + 3 * (int)player.playerType);
            categoryNames[2].text = StrUtil.GetTagName(103 + 3 * (int)player.playerType);
            
            categoryDescs[0].text = StrUtil.GetTagDesc(101 + 3 * (int)player.playerType);
            categoryDescs[1].text = StrUtil.GetTagDesc(102 + 3 * (int)player.playerType);
            categoryDescs[2].text = StrUtil.GetTagDesc(103 + 3 * (int)player.playerType);

            var list = SkillTreeDatas.GetSkillTreeList(player.playerType);
            var activated = SkillTreeDatas.GetActivatedSkillTrees();
        
            for (int i = 0; i < trees.Length; i++)
            {
                treeDict[i] = list[i];
                trees[i].color = activated.Contains(list[i]) ? Color.white : Color.black;
            }
        }
        
        public void SetDescription(SkillTrees tree)
        {
            descriptionWindow.SetActive(true);
            treeName.text = tree.Name;
            treeDesc.text = tree.Description;
            treeTag.text = "";
            tree.TagNames.ForEach(x =>
            {
                treeTag.text += x + ",";
            });
            treeTag.text = treeTag.text.Remove(treeTag.text.Length - 1, 1);
        
        }
    }
}