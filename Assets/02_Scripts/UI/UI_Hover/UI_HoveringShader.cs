using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_HoveringShader : MonoBehaviour
{
    [HideInInspector] public int isHovering;

    [FormerlySerializedAs("OnHoverOn")] public UnityEvent OnHoverEnter = new();
    public UnityEvent OnHoverExit = new();

    private Image image;
    private bool isSelected;

    private void Start()
    {
        image = GetComponent<Image>();
        var mat = Instantiate(image.material);
        image.material = mat;
    }

    private void Update()
    {
        if (isHovering == 1 || isSelected)
        {
            image.material.SetInt("_IsHovering", isHovering);
            image.material.SetFloat("_UnScaledTime", Time.unscaledTime);
        }
    }

    public void HoverEnter()
    {
        if (isSelected)
            isHovering = 0;
        else
            isHovering = 1;

        OnHoverEnter.Invoke();
    }

    public void HoverExit()
    {
        isHovering = 0;
        if (!isSelected) gameObject.SetActive(false);
        OnHoverExit.Invoke();
    }

    public void Select()
    {
        isHovering = 0;
        isSelected = true;
    }

    public void UnSelect()
    {
        isSelected = false;
    }
}