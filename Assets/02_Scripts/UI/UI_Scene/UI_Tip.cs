using _02_Scripts.UI.UI_SubItem;
using Apis.DataType;
using Apis.UI;
using Default;
using Save.Schema;
using UnityEngine;

namespace Apis
{
    public class UI_Tip : UI_Scene
    {
        [SerializeField] private UI_TipContent content;

        #region 바인딩

        private enum Btns
        {
            okBtn
        }

        #endregion
        
        public override void Init()
        {
            base.Init();
            Bind<UIAsset_Button>(typeof(Btns));
            
            Get<UIAsset_Button>((int)Btns.okBtn).OnClick.AddListener(CloseOwn);
            subItems.Add(content);
            content.Init();
        }

        public void InitData(TipDataType tipData)
        {
            content.SetInfo(tipData);
            DataAccess.Codex.UnLock(CodexData.CodexType.Tip, tipData.index);
        }
    }
}