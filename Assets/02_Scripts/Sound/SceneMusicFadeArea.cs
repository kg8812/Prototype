using System;
using System.Collections;
using System.Collections.Generic;
using Apis.Components;
using UnityEngine;
using UnityEngine.Serialization;

public class SceneMusicFadeArea : MonoBehaviour
{
    public int number;
    public float fadeTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Sound.SwapSceneBGMWithIndex(number,fadeTime);
        }
    }
}
