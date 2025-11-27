using System;
using Apis;
using Default;
using UnityEngine;

public class SingleUseInteraction : MonoBehaviour, IOnInteract
{
    private Collider2D _collider;

    private Interaction _interaction;

    private bool isUsed;
    public Interaction Interaction => _interaction ??= Utils.GetComponentInParentAndChild<Interaction>(gameObject);
    private Collider2D Collider => _collider ??= GetComponent<Collider2D>();

    private void Awake()
    {
        InteractCheckEvent += CheckUsed;
        isUsed = false;
    }

    public Func<bool> InteractCheckEvent { get; set; }

    public virtual void OnInteract()
    {
        isUsed = true;
        Collider.enabled = false;
        Interaction.OffActive();
    }

    private bool CheckUsed()
    {
        return !isUsed;
    }
}