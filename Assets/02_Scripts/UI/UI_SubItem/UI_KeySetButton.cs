using System.Collections;
using System.Collections.Generic;
using Apis;
using Apis.UI;
using Default;
using DG.Tweening;
using Save.Schema;
using UnityEngine;
using UnityEngine.UI;

public class UI_KeySetButton : UIAsset_Button
{
    public Define.GameKey gameKey;
    public Image keyImage;
    public Image changingImg;
    private bool _isChanging;
    public float changingImgDuration = 0.2f;
    
    public override void Init()
    {
        base.Init();
        DataAccess.Settings.Data.OnKeyChange.RemoveListener(SetKeyImage);
        DataAccess.Settings.Data.OnKeyChange.AddListener(SetKeyImage);
        SetKeyImage();
        _isChanging = false;
        OnClick.AddListener(() =>
        {
            _isChanging = true;
            changingImg.DOFade(1, changingImgDuration).SetUpdate(true);
        });
    }
    
    public override void KeyControl()
    {
        if (!_isChanging)
        {
            base.KeyControl();
        }
        else
        {
            // TODO: 근데 기존에 있는 키만 가능하면 딴 키 할당은 못하지 않나?
            foreach (KeyCode key in DataAccess.Settings.Data.KeycodeImages.Keys)
            {
                if (InputManager.GetKeyDown(key))
                {
                    DataAccess.Settings.Data.SetGameKey(gameKey,key);
                    _isChanging = false;
                    changingImg.DOFade(0, changingImgDuration).SetUpdate(true);
                    UI_Setting.IsDirty = true;
                }
            }
        }
    }

    public void SetKeyImage()
    {
        keyImage.sprite = DataAccess.Settings.Data.GetGameKeyImage(gameKey);
        // 2. 스프라이트가 null이면 에러 방지를 위해 종료합니다.
        if (keyImage.sprite == null)
        {
            keyImage.gameObject.SetActive(false); 
            return;
        }
        keyImage.gameObject.SetActive(true);
        Vector3 pos = keyImage.transform.position;
        keyImage.SetNativeSize();
        keyImage.transform.position = pos;
        var rt = keyImage.GetComponent<RectTransform>();
        var rtParent = keyImage.GetComponent<RectTransform>().parent.GetComponent<RectTransform>();

        Vector3[] rtCorners = new Vector3[4];  
        Vector3[] rtParentCorners = new Vector3[4];  
        rt.GetWorldCorners(rtCorners);
        rtParent.GetWorldCorners(rtParentCorners);

        var rtP1 = rtCorners[0];
        var rtP2 = rtCorners[2];
        
        var rtParentP1 = rtParentCorners[0];
        var rtParentP2 = rtParentCorners[2];

        rtP1 -= rtParentP1;
        rtP2 -= rtParentP1;
        rtParentP2 -= rtParentP1;
        rtParentP1 = Vector3.zero;

        var min = new Vector2(rtP1.x / rtParentP2.x, rtP1.y / rtParentP2.y);
        var max = new Vector2(rtP2.x / rtParentP2.x, rtP2.y / rtParentP2.y);

        rt.anchorMin = min;
        rt.anchorMax = max;
        
        rt.sizeDelta = Vector3.zero;
        rt.anchoredPosition = Vector2.zero;
    }
}
