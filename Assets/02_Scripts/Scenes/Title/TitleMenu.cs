using UnityEngine;

namespace Apis
{
    public class TitleMenu : MonoBehaviour
    {
        private void Start()
        {
            GameManager.instance.WhenReturnedToTitle.Invoke();
            GameManager.Sound.Play(Define.BGMList.MainThemeTitle, Define.Sound.BGM);
        }
    }
}