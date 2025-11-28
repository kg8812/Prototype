using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_CutOutMask : Image
{
    public override Material materialForRendering
    {
        get
        {
            Material material = new(base.materialForRendering);
            material.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            return material;
        }
    }
}