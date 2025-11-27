using UnityEngine;
using UnityEngine.UI;

public class UI_HoveringOnlyShader : MonoBehaviour
{
    [HideInInspector] public int isHovering;
    private Image image;


    private void Start()
    {
        image = GetComponent<Image>();
        var mat = Instantiate(image.material);
        image.material = mat;
    }

    private void Update()
    {
        image.material.SetInt("_IsHovering", 1);
        image.material.SetFloat("_UnScaledTime", Time.unscaledTime);
    }
}