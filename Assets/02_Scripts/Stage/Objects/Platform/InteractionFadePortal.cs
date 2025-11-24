using System;
using System.Collections;
using Directing;
using Managers;
using UI;
using UnityEngine;

public class InteractionFadePortal : MonoBehaviour,IOnInteract
{
    [SerializeField] private Transform toPos;

    private bool portaled = false;

    bool CheckPortaled()
    {
        return !portaled;
    }

    public Func<bool> InteractCheckEvent { get; set; }

    private void Awake()
    {
        InteractCheckEvent += CheckPortaled;
    }

    public void OnInteract()
    {
        Portaled();
    }
    
    public void Portaled()
    {
        portaled = true;
        FadeManager.instance.Fading(() =>
        {
            GameManager.instance.ControllingEntity.transform.position = toPos.position;
            GameManager.instance.ControllingEntity.MoveToFloor();
            TargetGroupCamera.instance.DoUpdate();
            CameraManager.instance.SetPlayerCamConfinerBox2D(null);
            CameraManager.instance.ToggleCameraFix(false);
            CameraManager.instance.InitPlayerCamPosition();
        },null,0.2f);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !other.isTrigger)
        {
            portaled = false;
        }
    }
}
