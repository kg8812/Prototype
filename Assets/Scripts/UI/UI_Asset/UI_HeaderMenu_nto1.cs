using Apis.UI.Focus;
using Default;
using UnityEngine;

namespace Apis.UI
{
    public class UI_HeaderMenu_nto1 : UI_Scene
    {
        [SerializeField] protected FocusParent headerController;

        [SerializeField] protected UI_MultiFuncTab contentControllers;


        protected IController _curContentController;

        public void Reset()
        {
            headerController.MoveTo(0);
        }

        public override void Init()
        {
            headerController.InitCheck();
            contentControllers.Init();
            _curContentController = contentControllers;
            headerController.FocusChanged.AddListener(FocusChanged);
        }

        protected virtual void FocusChanged(int id)
        {
            contentControllers.Change(id);
        }

        public override void KeyControl()
        {
            headerController.KeyControl();
            _curContentController.KeyControl();
        }

        public override void GamePadControl()
        {
            base.GamePadControl();
            headerController.GamePadControl();
            _curContentController.GamePadControl();
        }
    }
}