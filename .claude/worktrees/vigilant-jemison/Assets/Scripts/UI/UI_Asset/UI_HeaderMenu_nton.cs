using Apis.UI.Focus;
using UnityEngine;
using UnityEngine.Events;

namespace Apis.UI
{
    public class UI_HeaderMenu_nton: MonoBehaviour, IController
    {
        [Tooltip("UI의 탭 버튼 그룹을 제어하는 FocusParent입니다.")]
        public FocusParent headerController;

        [Tooltip("헤더의 각 탭 버튼에 연결된 실제 컨텐츠 UI들의 배열입니다.")]
        public UI_FocusContent[] contentControllers;

        [HideInInspector]public int curInd;
        public IController _curContentController;

        [Tooltip("탭(포커스)이 변경되기 직전에 호출되는 이벤트입니다.")]
        public UnityEvent<int> WillFocusChanged;
        [Tooltip("탭(포커스)이 변경된 직후에 호출되는 이벤트입니다.")]
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
            
            if (contentControllers[curInd].gameObject.activeSelf && curInd != id)
            {
                contentControllers[curInd].gameObject.SetActive(false);
                contentControllers[curInd].OnClose();
            }

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
            FocusChange(0);
            headerController.MoveTo(0,true);
        }
    }
}