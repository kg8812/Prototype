using Managers;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private float depth;
    [SerializeField] private bool isChild;
    [SerializeField] private bool fixX, fixY;
    private Transform mainCam;
    private readonly float maxDepth = 100;

    // private Vector2 GetVector2ByRatio(Vector2 a, Vector2 b, float ratio, bool isInner)
    // { // ratio = m / n
    //     int inner = isInner ? 1 : -1;
    //     return (ratio * b + inner * a) / (ratio + inner * 1);
    // }

    private Vector3 pos;

    private Vector3 prePos;
    private float ratio;
    private SpriteRenderer sp;

    private Transform trans;

    private void Start()
    {
        mainCam = CameraManager.instance.MainCam.transform;
        trans = GetComponent<Transform>();
        ResetSorting();
        prePos = trans.position;
        ratio = depth / maxDepth;
    }

    private void LateUpdate()
    {
        if (depth != 0)
        {
            pos = (mainCam.position - prePos) * ratio;
            pos.z = 0;
            if (fixX) pos.x = 0;
            if (fixY) pos.y = 0;
            trans.position = pos + prePos;
        }
    }

    [ContextMenu("Setting SortingLayer")]
    private void ResetSorting()
    {
        trans = GetComponent<Transform>();
        if (isChild)
        {
            foreach (var cSp in trans.GetComponentsInChildren<SpriteRenderer>())
            {
                cSp.sortingLayerName = depth == 0 ? "Ground" : depth >= 0 ? "Bg" : "Fg";
                cSp.sortingOrder = -(int)depth;
            }
        }
        else
        {
            sp = GetComponent<SpriteRenderer>();
            sp.sortingLayerName = depth == 0 ? "Ground" : depth >= 0 ? "Bg" : "Fg";
            sp.sortingOrder = -(int)depth;
        }
    }
}