using System;
using Managers;
using UI;
using UnityEngine;

public class InteractionFadePortal : MonoBehaviour, IOnInteract
{
    [SerializeField] private Transform toPos;

    private bool portaled;

    private void Awake()
    {
        InteractCheckEvent += CheckPortaled;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !other.isTrigger) portaled = false;
    }

    public Func<bool> InteractCheckEvent { get; set; }

    public void OnInteract()
    {
        Portaled();
    }

    private bool CheckPortaled()
    {
        return !portaled;
    }

    public void Portaled()
    {
        portaled = true;
        FadeManager.instance.Fading(() =>
        {
            GameManager.instance.ControllingEntity.transform.position = toPos.position;
            GameManager.instance.ControllingEntity.MoveToFloor();
        }, null, 0.2f);
    }
}