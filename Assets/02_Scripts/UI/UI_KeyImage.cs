using System;
using System.Collections;
using System.Collections.Generic;
using Save.Schema;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class UI_KeyImage : MonoBehaviour
{
    [Serializable]
    public enum KeyType
    {
        InGame,UI
    }

    public KeyType keyType = KeyType.InGame;
    
    [ShowIf("keyType", KeyType.InGame)]
    public Define.GameKey gameKey;
    [ShowIf("keyType", KeyType.UI)]
    public Define.UIKey uiKey;
    public Image image;
    public SpriteRenderer spriteRenderer;
    
    private void Start()
    {
        DataAccess.Settings.Data.OnKeyChange.RemoveListener(SetKeyImage);
        DataAccess.Settings.Data.OnKeyChange.AddListener(SetKeyImage);
        SetKeyImage();
    }

    void SetKeyImage()
    {
        if (image != null)
        {
            switch (keyType)
            {
                case KeyType.InGame:
                    image.sprite = DataAccess.Settings.Data.GetGameKeyImage(gameKey);
                    break;
                case KeyType.UI:
                    image.sprite = DataAccess.Settings.Data.GetUIKeyImage(uiKey);
                    break;
            }
            if (image.sprite == null)
            {
                image.gameObject.SetActive(false); 
                return;
            }
           
            image.gameObject.SetActive(true);
            Vector3 pos = image.transform.position;
            image.SetNativeSize();
            image.transform.position = pos;
            var rt = image.GetComponent<RectTransform>();
            var rtParent = image.GetComponent<RectTransform>().parent.GetComponent<RectTransform>();

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

        if (spriteRenderer != null)
        {
            switch (keyType)
            {
                case KeyType.InGame:
                    spriteRenderer.sprite = DataAccess.Settings.Data.GetGameKeyImage(gameKey);
                    break;
                case KeyType.UI:
                    spriteRenderer.sprite = DataAccess.Settings.Data.GetUIKeyImage(uiKey);
                    break;
            }
        }
    }
}
