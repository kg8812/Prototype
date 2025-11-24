using System.Text.RegularExpressions;
using Apis.Managers;
using Directing;
using Managers;
using Save.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class UI_Dialogue: UI_Ingame, IController
    {
        enum RectTransforms
        {
            Info
        }

        enum Texts
        {
            Content
        }

        enum Images
        {
            SkipBtn
        }

        private const float autoFloatTime = 3f;
        private Vector2 offset = new Vector2(0, 300);

        private RectTransform rect;
        private DialogueDirector _dialogueDirector;

        private ScriptData curScriptData;
        private string[] scriptList;
        private int curInd;
        private bool isAppearing;

        [HideInInspector] public UnityAction whenDeactivated;

        private bool isShowingScript = false;
        
        
        public Febucci.UI.Core.TypewriterCore typewriter;


        private const string YesNoPopupReg = @"^~(.+)#(-?\d+)\^(-?\d+)$";
        private const string CheckPopupReg = @"^%(.+)$";
        private Match _match;

        public override void Init()
        {
            base.Init();
        
            Bind<RectTransform>(typeof(RectTransforms));
            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<Image>(typeof(Images));
            
            rect = Get<RectTransform>((int)RectTransforms.Info);
            
            typewriter.onTextShowed.AddListener(() =>
            {
                if (!isShowingScript) return;
                isAppearing = false;
                if (curScriptData.isAuto)
                {
                    Invoke("ShowNextScript", curScriptData.duration[curInd]);
                }
                else
                {
                    ToggleSkipBtn(true);
                }
            });
        }

        public void KeyControl()
        {
            if (curScriptData.isAuto) return;
            if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Skip)))
            {
                if (isAppearing)
                {
                    typewriter.SkipTypewriter();
                }
                else
                {
                    ToggleSkipBtn(false);
                    ShowNextScript();
                }
            }
        }

        public void GamePadControl()
        {
            if (curScriptData.isAuto) return;
            if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Skip)))
            {
                if (isAppearing)
                {
                    typewriter.SkipTypewriter();
                }
                else
                {
                    ToggleSkipBtn(false);
                    ShowNextScript();
                }
            }
        }

        public void SetInfo(Vector2 targetPos)
        {
            this.targetPos = targetPos;
        }
        public void SetInfo(Transform targetTrans, DialogueDirector dialogueDirector)
        {
            this.targetTrans = targetTrans;
            _dialogueDirector = dialogueDirector;
        }

        public void SetScripts(ScriptData scriptDataType)
        {
            curScriptData = scriptDataType;
            scriptList = LanguageManager.LanguageType == LanguageType.Korean ? scriptDataType.scriptsKor : scriptDataType.scriptsEng;
            curInd = 0;
            isShowingScript = true;
            CheckStringType(scriptList[curInd]);
        }

        private void CheckStringType(string content)
        {
            _match = Regex.Match(content, YesNoPopupReg);
            if (_match.Success)
            {
                SystemManager.SystemCheck(_match.Groups[1].Value, isYes =>
                {
                    int nextIndex = int.Parse(_match.Groups[isYes ? 2 : 3].Value);
                    _dialogueDirector.isFailed = !isYes;
                    if (nextIndex >= 0)
                    {
                        SetScripts(new ScriptData(_dialogueDirector.GetScriptDataType(nextIndex)));
                    }
                    else
                    {
                        isShowingScript = false;
                        GameManager.UI.CloseUI(this);
                    }
                });
            }
            else
            {
                _match = Regex.Match(content, CheckPopupReg);
                if (_match.Success)
                {
                    SystemManager.SystemAlert(_match.Groups[1].Value, ShowNextScript);
                }
                else
                {
                    ShowText(content);
                }
            }
        }
        
        
        public void ShowText(string content)
        {
            isAppearing = true;
            typewriter.ShowText(content);
        }

        public void ShowNextScript()
        {
            curInd++;
            if (curInd >= scriptList.Length)
            {
                isShowingScript = false;
                GameManager.UI.CloseUI(this);
            }
            else
            {
                CheckStringType(scriptList[curInd]);
            }
        }

        private void ToggleSkipBtn(bool isShow)
        {
            GetImage((int)Images.SkipBtn).enabled = isShow;
        }
        
        protected override void PositioningFollower()
        {
            rect.anchoredPosition = calcPos + offset;
        }


        protected override void Activated()
        {
            base.Activated();
            Director.DialogueController = this;
        }

        public override void TryDeactivated(bool force = false)
        {
            Director.DialogueController = null;
            base.TryDeactivated(force);
        }

        protected override void Deactivated()
        {
            base.Deactivated();
            isShowingScript = false;
            whenDeactivated?.Invoke();
            whenDeactivated = null;
        }
    }
}