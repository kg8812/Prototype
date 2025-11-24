using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCapturer : MonoBehaviour
{
    private Camera cam;
    public int size = 1;
    
    
    [Button(ButtonSizes.Large)]
    public void SetCamSize()
    {
        
        Screen.SetResolution(2000, 1024, true);
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 4f * size;
        cam.transform.position = new Vector3(Mathf.RoundToInt(cam.transform.position.x),
            Mathf.RoundToInt(cam.transform.position.y), cam.transform.position.z);
    }

    [Button(ButtonSizes.Large)]
    public void Capture()
    {
        string filePath = Path.Combine(Application.dataPath, $"Capture/{SceneManager.GetActiveScene().name}-{Default.FormatUtils.CurrentTimeToId()}.png");
        ScreenCapture.CaptureScreenshot(filePath, size);
    }
}
