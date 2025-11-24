#define DEBUG_LOG_PLAYER
using UnityEngine;
using TMPro;

public class PlayerStateMonitor : MonoBehaviour
{
    [SerializeField] private bool EnableMonitor = true;
    [SerializeField] private TextMeshPro text;
    // Start is called before the first frame update

    void Start()
    {
#if DEBUG_LOG_PLAYER 
        if(!EnableMonitor) gameObject.SetActive(false);
        SetText(EPlayerState.Idle);
#endif
    }

    public void SetText(EPlayerState state)
    {
#if DEBUG_LOG_PLAYER
        text.text = state.ToString();
#endif
    }

}
