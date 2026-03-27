using UnityEngine;

public class SetSceneMusicVolumeArea : MonoBehaviour
{
    public float volume;
    public float fadeTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) GameManager.Sound.SetSceneBGMVolume(volume, fadeTime);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) GameManager.Sound.SetSceneBGMVolume(1, fadeTime);
    }
}