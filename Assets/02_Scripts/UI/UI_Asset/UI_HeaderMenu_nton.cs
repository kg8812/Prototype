using Apis.UI.Focus;
using UnityEngine;
using UnityEngine.Events;

namespace Apis.UI
{
    public class UI_HeaderMenu_nton: MonoBehaviour, IController
    {
        public FocusParent headerController;

        public UI_FocusContent[] contentControllers;

        public int curInd;
        public IController _curContentController;

        public UnityEvent<int> WillFocusChanged;
        public UnityEvent<int> FocusChanged;

        public void Init()
        {
            foreach (var curCont in contentControllers)
            {
                curCont.InitCheck();
            }
            headerController.InitCheck();
            headerController.FocusChanged.AddListener(FocusChange);
        }

        protected virtual void FocusChange(int id)
        {
            WillFocusChanged.Invoke(curInd);
            contentControllers[curInd].gameObject.SetActive(false);
            contentControllers[curInd].OnClose();
            curInd = id;
            contentControllers[curInd].OnOpen();
            contentControllers[curInd].gameObject.SetActive(true);
            _curContentController = contentControllers[curInd];
            FocusChanged.Invoke(curInd);
        }

        public void KeyControl()
        {
            _curContentController?.KeyControl();
            headerController.KeyControl();
        }

        public void GamePadControl()
        {
            _curContentController?.GamePadControl();
            headerController.GamePadControl();
        }

        public void Reset()
        {
            headerController.MoveTo(0);
        }
    }
}