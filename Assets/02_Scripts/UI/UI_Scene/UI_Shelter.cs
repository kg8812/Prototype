using System.Collections;
using System.Collections.Generic;
using Apis;
using Apis.UI;
using Apis.UI.Focus;
using Default;
using Directing;
using Managers;
using Save.Schema;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Apis
{
    public class UI_Shelter : UI_Scene
    {
        #region Presets

        enum Buttons
        {
            SkillTree,
            OpenAtkItem,
            OpenAcc,
            Exit
        }

        enum FocusParents
        {
            Btns
        }

        #endregion

        private FocusParent _fc;
        

        public override void Init()
        {
            base.Init();

            Bind<UIAsset_Button>(typeof(Buttons));
            Bind<FocusParent>(typeof(FocusParents));

            _fc = Get<FocusParent>((int)FocusParents.Btns);

            #region 버튼 이벤트 등록

            Get<UIAsset_Button>((int)Buttons.SkillTree).OnClick.AddListener(() =>
            {
                if (DataAccess.TaskData.IsDone(102) && DataAccess.TaskData.IsDone(103))
                {
                    GameManager.UI.CreateUI("UI_SkillTree", UIType.Scene);
                }
                else
                {
                    SystemManager.SystemAlert("패시브 및 액티브 스킬을 활성화해주세요.",null);
                }
            });
            Get<UIAsset_Button>((int)Buttons.OpenAtkItem).OnClick.AddListener(() =>
            {
                OpenTabMenu(2);
            });
            Get<UIAsset_Button>((int)Buttons.OpenAcc).OnClick.AddListener(() =>
            {
                OpenTabMenu(3);
            });
            Get<UIAsset_Button>((int)Buttons.Exit).OnClick.AddListener(CloseOwn);

            #endregion
        }
        
        public override void TryDeactivated(bool force = false)
        {
            base.TryDeactivated(force);
            TargetGroupCamera.instance.AdjustTargetRadius(CameraManager.instance.fakePlayerTarget.transform,
                GameManager.instance.Player.camRadius);
            GameManager.instance.Player.ControlOn(); 
            GameManager.instance.SaveSlot();
        }
        
        
        // 무장/악세 장착
        private void OpenTabMenu(int index)
        {
            UI_TabMenu tabMenu = GameManager.UI.CreateUI("UI_TabMenu", UIType.Scene) as UI_TabMenu;
            if (tabMenu == null) return;
            UITab_Inventory.PreventMovingOrigin = false;
            tabMenu.MoveHeader(index);
            void CloseEvent()
            {
                UITab_Inventory.PreventMovingOrigin = true;
                tabMenu.OnDeactivated.RemoveListener(CloseEvent);
            }
            tabMenu.OnDeactivated.AddListener(CloseEvent);
        }

        
        // exit
        public override void CloseOwn()
        {
            FadeManager.instance.Fading(CloseOwnForce);
        }


        public override void KeyControl()
        {
            base.KeyControl();
            _fc?.KeyControl();
        }

        public override void GamePadControl()
        {
            base.GamePadControl();
            _fc?.GamePadControl();
        }
    }
}