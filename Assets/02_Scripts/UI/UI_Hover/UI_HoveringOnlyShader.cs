using UnityEngine;
using UnityEngine.UI;

public class UI_HoveringOnlyShader : MonoBehaviour
{
    Image image;

    [HideInInspector] public int isHovering;


    private void Start() {
        image = GetComponent<Image>();
        Material mat = Instantiate(image.material);
        image.material = mat;

    }
    void Update()
    {

        image.material.SetInt("_IsHovering",1);
        image.material.SetFloat("_UnScaledTime",Time.unscaledTime);
            
    }

}
