using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraShaker : MonoBehaviour
{
    [FormerlySerializedAs("amplify")] [LabelText("강도")] public float amplitude;
    [LabelText("주기")] public float frequency;
    [LabelText("지속시간")]public float duration;
    private void OnEnable()
    {
        GameManager.instance.StartCoroutine(CameraManager.instance.ShakePlayerCam(amplitude,frequency,duration));
    }
}
