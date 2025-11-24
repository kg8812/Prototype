using UnityEngine;

namespace Apis
{
    public class pickMenu : MonoBehaviour
    {
        private void Start()
        {
            GameManager.UI.CreateUI("UI_Title", UIType.Scene);
        }
    }
}