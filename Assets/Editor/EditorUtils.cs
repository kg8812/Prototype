using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public static class UIExtensions
{
    // UI의 앵커를 현재 사이즈에 맞춰서 조절하는 기능입니다.
    [MenuItem ("CONTEXT/RectTransform/Set Anchor by corners")]
    static void SetAnchor (MenuCommand command) 
    {
        var rt = command.context.GetComponent<RectTransform>();
        var rtParent = command.context.GetComponent<RectTransform>().parent.GetComponent<RectTransform>();

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