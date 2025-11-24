using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectOnlyShader : MonoBehaviour
{
    [SerializeField]
    Image IconImage;
    [SerializeField]
    TextMeshProUGUI text;

    private void Awake() {
        Material mat = Instantiate(IconImage.material);
        IconImage.material = mat;
        IconImage.material.SetInt("_IsSelect", 0);
        text.color = Color.gray;

    }


    public void Selected(bool isSelected) {

        int booltoint;
        if (isSelected) {
            text.color = Color.white;
            booltoint = 1;
        }

        else {
            text.color = Color.gray;
            booltoint = 0;
        }
        IconImage.material.SetInt("_IsSelect", booltoint);

    }


}
