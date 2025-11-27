using UnityEngine;

public class SceneMusicFadeArea : MonoBehaviour
{
    public int number;
    public float fadeTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) GameManager.Sound.SwapSceneBGMWithIndex(number, fadeTime);
    }
}