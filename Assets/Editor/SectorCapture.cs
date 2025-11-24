using UnityEngine;
using UnityEditor;
using System.IO;
using Sirenix.OdinInspector;

public class SectorCapture : MonoBehaviour
{
    [SerializeField] private int pixelPerUnit = 32; // 1 Unit 당 픽셀 수 (기본값: 32px)

    [Button("Capture Sectors")]
    public void CaptureAllSectors()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;

        if (sceneView == null)
        {
            Debug.LogError("No active Scene View found!");
            return;
        }

        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            string sectorName = obj.name;
            if (!sectorName.StartsWith("Sector")) continue;

            // ID 추출 (Sector123 -> 123)
            string sectorId = sectorName.Substring(6);
            if (!int.TryParse(sectorId, out int id)) continue;

            // BoxCollider2D 크기 가져오기
            BoxCollider2D collider = obj.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                Debug.LogWarning($"No BoxCollider2D found on {sectorName}");
                continue;
            }

            Vector2 size = collider.size;
            Vector2 position = (Vector2)obj.transform.position + collider.offset;

            // Scene View 캡처 실행
            CaptureSceneView(id, position, size, sceneView);
            break;
        }
    }

    private void CaptureSceneView(int sectorId, Vector2 position, Vector2 size, SceneView sceneView)
    {
        Camera cam = sceneView.camera;
        if (cam == null)
        {
            Debug.LogError("Scene View camera not found!");
            return;
        }

        int width = Mathf.RoundToInt(size.x * pixelPerUnit);
        int height = Mathf.RoundToInt(size.y * pixelPerUnit);

        RenderTexture rt = new RenderTexture(width, height, 24);
        cam.targetTexture = rt;
        cam.Render();
        RenderTexture.active = rt;

        // 월드 좌표를 스크린 좌표로 변환
        Vector3 screenPos = cam.WorldToScreenPoint(position);
        int screenX = Mathf.RoundToInt(screenPos.x);
        int screenY = Mathf.RoundToInt(Screen.height - screenPos.y); // Y 좌표 변환

        // 렌더 텍스처 범위를 넘어가지 않도록 조정
        screenX = Mathf.Clamp(screenX, 0, rt.width - width);
        screenY = Mathf.Clamp(screenY, 0, rt.height - height);

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(screenX, screenY, width, height), 0, 0);
        texture.Apply();

        // 파일 저장
        string savePath = Path.Combine(Application.dataPath, $"Sector_{sectorId}.png");
        File.WriteAllBytes(savePath, texture.EncodeToPNG());

        Debug.Log($"Captured Sector {sectorId}: {savePath}");

        // 정리
        cam.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);
        DestroyImmediate(texture);
    }

}
