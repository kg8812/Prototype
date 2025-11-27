using Default;
using Save.Schema;
using UnityEngine.UI;

public class KeySettingPopup : UI_Scene
{
    public Image keyImage;
    private UI_KeySetButton _gameKey;

    public void SetKey(UI_KeySetButton gameKey)
    {
        _gameKey = gameKey;
        SetKeyImage();
    }

    public override void TryActivated(bool force = false)
    {
        base.TryActivated(force);
    }

    public void SetKeyImage()
    {
        keyImage.sprite = DataAccess.Settings.Data.GetGameKeyImage(_gameKey.gameKey);
    }

    public override void KeyControl()
    {
        base.KeyControl();

        foreach (var key in DataAccess.Settings.Data.KeycodeImages.Keys)
            if (InputManager.GetKeyDown(key))
            {
                DataAccess.Settings.Data.SetGameKey(_gameKey.gameKey, key);
                GameManager.UI.CloseUI(this);
            }
    }
}