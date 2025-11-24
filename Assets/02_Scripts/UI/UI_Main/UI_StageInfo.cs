using System.Collections;
using TMPro;
using UISpaces;
using UnityEngine;

namespace UI
{
    public class UI_StageInfo: UI_Main
    {
        [SerializeField] private float closeSec = 2f;
        enum Texts
        {
            StageName
        }

        public override void Init()
        {
            base.Init();
            Bind<TextMeshProUGUI>(typeof(Texts));
        }

        public override void ReShow()
        {
            base.ReShow();
        }

        public void Show(string str)
        {
            GetText((int)Texts.StageName).text = str;
            StopCoroutine(nameof(CloseInfo));
            StartCoroutine(CloseInfo());
        }

        private IEnumerator CloseInfo()
        {
            yield return new WaitForSeconds(closeSec);
            CloseOwn();
        }
    }
}