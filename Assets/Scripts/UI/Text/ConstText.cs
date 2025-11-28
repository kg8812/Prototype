using Apis.Managers;
using TMPro;
using UnityEngine;

namespace Apis
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ConstText : MonoBehaviour
    {
        public int textId;

        private TextMeshProUGUI _text;

        private void OnEnable()
        {
            _text ??= GetComponent<TextMeshProUGUI>();
            if (textId != 0) LanguageManager.RegisterText(_text, textId);
        }

        private void OnDisable()
        {
            if (!GameManager.IsQuitting && _text != null)
                LanguageManager.UnregisterText(_text);
        }

        public void ChangeId(int id)
        {
            textId = id;
            LanguageManager.RegisterText(_text, textId);
        }
    }
}