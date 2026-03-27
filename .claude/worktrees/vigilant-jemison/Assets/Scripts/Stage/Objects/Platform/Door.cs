using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool isOriginClosed;
    [SerializeField] private Ease openFunc = Ease.Linear;
    [SerializeField] private Ease closeFunc = Ease.Linear;
    [SerializeField] private Vector2 openPos;

    [HideIf("isOriginClosed")] [SerializeField]
    private Vector2 closePos;

    [SerializeField] private float openDuration = 1f;
    [SerializeField] private float closeDuration = 1f;
    [SerializeField] private bool isClosed;
    [SerializeField] private Transform doorTrans;

    private Collider2D _collider2d;
    private Sequence openSequence, closeSequence;
    private Collider2D Collider2d => _collider2d ??= GetComponent<Collider2D>();


    protected virtual void Awake()
    {
        if (isOriginClosed) closePos = doorTrans.localPosition;
        openSequence = DOTween
            .Sequence()
            .Append(doorTrans.DOLocalMove(openPos, openDuration).SetEase(openFunc))
            .SetAutoKill(false)
            .Pause();

        closeSequence = DOTween
            .Sequence()
            .Append(doorTrans.DOLocalMove(closePos, closeDuration).SetEase(closeFunc))
            .SetAutoKill(false)
            .Pause();
        doorTrans.localPosition = isClosed ? closePos : openPos;
    }

    protected virtual void Start()
    {
    }

    protected void MoveDoor(bool isOpen)
    {
        if (isOpen)
        {
            closeSequence.Restart();
            Collider2d.enabled = true;
        }
        else
        {
            openSequence.Restart();
            Collider2d.enabled = false;
        }
    }
}